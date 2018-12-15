using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Server.Game;
using Netsphere.Server.Game.GameRules;
using Netsphere.Server.Game.Plugins;
using ProudNet.Hosting.Services;

namespace ExamplePlugin
{
    public class ExamplePlugin : IPlugin
    {
        public void OnInitialize()
        {
        }

        public void OnConfigure(IServiceCollection services)
        {
            services
                .AddTransient<ExamplePluginGameRule>()
                .AddHostedServiceEx<ExamplePluginService>();
        }

        public void OnShutdown()
        {
        }
    }

    public class ExamplePluginService : IHostedService, IGameRuleResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DefaultGameRuleResolver _defaultGameRuleResolver;

        public ExamplePluginService(IServiceProvider serviceProvider, GameRuleManager gameRuleManager)
        {
            _serviceProvider = serviceProvider;
            _defaultGameRuleResolver = new DefaultGameRuleResolver(gameRuleManager);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RoomManager.RoomCreateHook += OnRoomCreateHook;
            RoomManager.RoomCreated += OnRoomCreated;
            Channel.JoinHook += OnChannelJoinHook;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool OnRoomCreateHook(RoomCreateHookEventArgs e)
        {
            e.Options.GameRuleResolver = this;
            return true;
        }

        private void OnRoomCreated(object sender, RoomEventArgs e)
        {
            e.Room.OptionsChanged += OnRoomOptionsChanged;
            e.Room.GameRule.CanStartGameHook += OnCanStartGameHook;
            e.Room.GameRule.HasEnoughPlayersHook += OnHasEnoughPlayersHook;
        }

        private bool OnChannelJoinHook(ChannelJoinHookEventArgs e)
        {
            if (e.Channel.Id == 4)
                return true;

            e.Error = ChannelJoinError.AlreadyInChannel;
            return false;
        }

        private void OnRoomOptionsChanged(object sender, RoomEventArgs e)
        {
            e.Room.GameRule.CanStartGameHook += OnCanStartGameHook;
            e.Room.GameRule.HasEnoughPlayersHook += OnHasEnoughPlayersHook;
        }

        private bool OnCanStartGameHook(CanStartGameHookEventArgs e)
        {
            if (e.GameRule.GameRule == GameRule.Deathmatch)
                e.Result = true;

            return true;
        }

        private bool OnHasEnoughPlayersHook(HasEnoughPlayersHookEventArgs e)
        {
            if (e.GameRule.GameRule == GameRule.Deathmatch)
                e.Result = true;

            return true;
        }

        public GameRuleBase Resolve(Room room)
        {
            return room.Options.MatchKey.GameRule == GameRule.Touchdown
                ? _serviceProvider.GetRequiredService<ExamplePluginGameRule>()
                : _defaultGameRuleResolver.Resolve(room);
        }
    }

    public class ExamplePluginGameRule : Touchdown
    {
        public ExamplePluginGameRule(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions,
            IOptions<TouchdownOptions> options, ISchedulerService schedulerService)
            : base(stateMachine, gameOptions, options, schedulerService)
        {
            stateMachine.ScheduleTriggerHook += ScheduleTriggerHook;
        }

        protected override bool CanStartGame()
        {
            return true;
        }

        protected override bool HasEnoughPlayers()
        {
            return true;
        }

        private bool ScheduleTriggerHook(ScheduleTriggerHookEventArgs e)
        {
            e.Cancel = true;
            return true;
        }
    }
}
