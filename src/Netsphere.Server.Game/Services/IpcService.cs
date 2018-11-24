using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Game.Services
{
    public class IpcService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly ChannelService _channelService;
        private readonly CancellationTokenSource _cts;

        public IpcService(IMessageBus messageBus, PlayerManager playerManager, ChannelService channelService)
        {
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channelService = channelService;
            _cts = new CancellationTokenSource();

            _channelService.PlayerJoined += ChannelOnPlayerJoined;
            _channelService.PlayerLeft += ChannelOnPlayerLeft;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeToRequestAsync<ChatLoginRequest, ChatLoginResponse>(OnChatLogin, _cts.Token);
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
                return Task.FromResult(new ChatLoginResponse(false));

            return Task.FromResult(new ChatLoginResponse(true));
        }

        private void ChannelOnPlayerJoined(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerJoinedMessage(e.Player.Account.Id, e.Channel.Id));
        }

        private void ChannelOnPlayerLeft(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerLeftMessage(e.Player.Account.Id, e.Channel.Id));
        }
    }
}
