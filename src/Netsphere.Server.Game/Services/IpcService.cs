using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Game.Services
{
    public class IpcService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly ChannelService _channelService;
        private readonly ServerListOptions _serverListOptions;
        private readonly CancellationTokenSource _cts;

        public IpcService(IMessageBus messageBus, PlayerManager playerManager, ChannelService channelService,
            IOptions<ServerListOptions> serverListOptions)
        {
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channelService = channelService;
            _serverListOptions = serverListOptions.Value;
            _cts = new CancellationTokenSource();

            _channelService.PlayerJoined += ChannelOnPlayerJoined;
            _channelService.PlayerLeft += ChannelOnPlayerLeft;
            _playerManager.PlayerDisconnected += OnPlayerDisconnected;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeToRequestAsync<ChatLoginRequest, ChatLoginResponse>(OnChatLogin, _cts.Token);
            await _messageBus.SubscribeToRequestAsync<RelayLoginRequest, RelayLoginResponse>(OnRelayLogin, _cts.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private Task<ChatLoginResponse> OnChatLogin(ChatLoginRequest request)
        {
            var plr = _playerManager[request.AccountId];
            if (plr == null || plr.Session.SessionId != request.SessionId)
                return Task.FromResult(new ChatLoginResponse(false, null, 0));

            return Task.FromResult(new ChatLoginResponse(true, plr.Account, plr.TotalExperience));
        }

        private Task<RelayLoginResponse> OnRelayLogin(RelayLoginRequest request)
        {
            var plr = _playerManager[request.AccountId];
            if (plr == null || plr.Account.Nickname != request.Nickname ||
                request.ServerId != _serverListOptions.Id ||
                plr.Channel == null || plr.Room == null ||
                plr.Channel.Id != request.ChannelId || plr.Room.Id != request.RoomId)
            {
                return Task.FromResult(new RelayLoginResponse(false, null));
            }

            return Task.FromResult(new RelayLoginResponse(true, plr.Account));
        }

        private void ChannelOnPlayerJoined(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerJoinedMessage(e.Player.Account.Id, e.Channel.Id));
        }

        private void ChannelOnPlayerLeft(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerLeftMessage(e.Player.Account.Id, e.Channel.Id));
        }

        private void OnPlayerDisconnected(object sender, PlayerEventArgs e)
        {
            _messageBus.PublishAsync(new PlayerDisconnectedMessage(e.Player.Account.Id));
        }
    }
}
