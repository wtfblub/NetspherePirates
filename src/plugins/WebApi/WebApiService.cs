using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpressMapper;
using ExpressMapper.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Server.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Serilog;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Swan;
using WebApi.Models;
using Constants = Serilog.Core.Constants;

namespace WebApi
{
    public class WebApiService : IHostedService
    {
        private readonly GameDataService _gameDataService;
        private readonly WebServer _webServer;

        public WebApiService(IServiceProvider serviceProvider, IOptions<WebApiOptions> options, GameDataService gameDataService)
        {
            _gameDataService = gameDataService;
            Terminal.Settings.DisplayLoggingMessageType = LogMessageType.None;
            Terminal.OnLogMessageReceived += (_, e) =>
            {
                var logger = Log.ForContext(Constants.SourceContextPropertyName, $"WebApi-{e.Source}");
                switch (e.MessageType)
                {
                    case LogMessageType.Info:
                        logger.Information(e.Exception, e.Message);
                        break;

                    case LogMessageType.Debug:
                        logger.Debug(e.Exception, e.Message);
                        break;

                    case LogMessageType.Trace:
                        logger.Verbose(e.Exception, e.Message);
                        break;

                    case LogMessageType.Error:
                        logger.Error(e.Exception, e.Message);
                        break;

                    case LogMessageType.Warning:
                        logger.Warning(e.Exception, e.Message);
                        break;

                    case LogMessageType.Fatal:
                        logger.Fatal(e.Exception, e.Message);
                        break;
                }
            };

            _webServer = new WebServer($"http://{options.Value.Listener}/");
            _webServer.RegisterModule(new WebApiModule());

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(WebApiController)))
                {
                    var parameters = new List<object>
                    {
                        null
                    };

                    var ctor = type.GetConstructors().First();
                    parameters.AddRange(ctor.GetParameters()
                        .Skip(1)
                        .Select(x => serviceProvider
                            .GetRequiredService(x.ParameterType)));

                    var arr = parameters.ToArray();
                    _webServer
                        .Module<WebApiModule>()
                        .RegisterController(type, ctx =>
                        {
                            arr[0] = ctx;
                            return Activator.CreateInstance(type, arr);
                        });
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Mapper.Register<MapInfo, MapDto>();

            Mapper.Register<ItemInfo, ItemDto>()
                .Member(dest => dest.Id, src => src.ItemNumber.Id);

            Mapper.Register<DefaultItem, DefaultItemDto>()
                .Function(dest => dest.Item, src =>
                {
                    var itemInfo = _gameDataService.Items[src.ItemNumber];
                    return itemInfo.Map<ItemInfo, ItemDto>();
                });

            Mapper.Register<Channel, ChannelDto>()
                .Member(dest => dest.PlayersOnline, src => src.Players.Count);

            Mapper.Register<Player, PlayerDto>()
                .Member(dest => dest.Id, src => src.Account.Id)
                .Member(dest => dest.Username, src => src.Account.Username)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Function(dest => dest.ActiveCharacter, src => src.CharacterManager.CurrentCharacter?.Slot ?? 0)
                .Function(dest => dest.Characters,
                    src => src.CharacterManager.Select(x => x.Map<Character, CharacterDto>()).ToArray())
                .Function(dest => dest.Inventory,
                    src => src.Inventory.Select(x => x.Map<PlayerItem, PlayerItemDto>()).ToArray())
                .Function(dest => dest.ChannelId, src => src.Channel?.Id)
                .Function(dest => dest.RoomId, src => src.Room?.Id);

            Mapper.Register<Character, CharacterDto>()
                .Function(dest => dest.Weapons, src => src.Weapons.GetItems().Select(x => x?.Id ?? 0).ToArray())
                .Function(dest => dest.Skills, src => src.Skills.GetItems().Select(x => x?.Id ?? 0).ToArray())
                .Function(dest => dest.Costumes, src => src.Costumes.GetItems().Select(x => x?.Id ?? 0).ToArray());

            Mapper.Register<PlayerItem, PlayerItemDto>()
                .Function(dest => dest.Item, src =>
                {
                    var itemInfo = _gameDataService.Items[src.ItemNumber];
                    return itemInfo.Map<ItemInfo, ItemDto>();
                })
                .Function(dest => dest.PurchaseTimestamp, src => src.PurchaseDate.ToUnixTimeSeconds());

            Mapper.Register<Room, RoomDto>()
                .Member(dest => dest.Name, src => src.Options.Name)
                .Member(dest => dest.CreationTimestamp, src => src.TimeCreated.ToUnixEpochDate())
                .Member(dest => dest.MasterId, src => src.Master.Account.Id)
                .Member(dest => dest.HostId, src => src.Host.Account.Id)
                .Member(dest => dest.GameRule, src => src.GameRule.GameRule)
                .Function(dest => dest.State, src => src.GameRule.StateMachine.GameState)
                .Function(dest => dest.TimeState, src => src.GameRule.StateMachine.TimeState)
                .Member(dest => dest.PlayerLimit, src => src.Options.MatchKey.PlayerLimit)
                .Member(dest => dest.SpectatorLimit, src => src.Options.MatchKey.SpectatorLimit)
                .Member(dest => dest.Password, src => src.Options.Password)
                .Member(dest => dest.TimeLimit, src => src.Options.TimeLimit.TotalMinutes)
                .Member(dest => dest.ScoreLimit, src => src.Options.ScoreLimit)
                .Member(dest => dest.IsFriendly, src => src.Options.IsFriendly)
                .Member(dest => dest.IsBalanced, src => src.Options.IsBalanced)
                .Member(dest => dest.MinLevel, src => src.Options.MinLevel)
                .Member(dest => dest.MaxLevel, src => src.Options.MaxLevel)
                .Member(dest => dest.ItemLimit, src => src.Options.ItemLimit)
                .Member(dest => dest.IsNoIntrusion, src => src.Options.IsNoIntrusion)
                .Function(dest => dest.Players,
                    src => src.Players.Values.Select(x => x.Map<Player, RoomPlayerDto>()).ToArray());

            Mapper.Register<Player, RoomPlayerDto>()
                .Member(dest => dest.TeamId, src => src.Team.Id)
                .Member(dest => dest.Id, src => src.Account.Id)
                .Member(dest => dest.Username, src => src.Account.Username)
                .Member(dest => dest.Nickname, src => src.Account.Nickname)
                .Function(dest => dest.ActiveCharacter, src => src.CharacterManager.CurrentCharacter?.Slot ?? 0)
                .Function(dest => dest.Characters,
                    src => src.CharacterManager.Select(x => x.Map<Character, CharacterDto>()).ToArray())
                .Function(dest => dest.Inventory,
                    src => src.Inventory.Select(x => x.Map<PlayerItem, PlayerItemDto>()).ToArray());

            // ReSharper disable once MethodSupportsCancellation
            var _ = _webServer.RunAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _webServer.Dispose();
            return Task.CompletedTask;
        }
    }
}
