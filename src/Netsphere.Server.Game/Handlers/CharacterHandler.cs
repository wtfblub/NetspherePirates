using System.Threading.Tasks;
using Logging;
using Netsphere.Common;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class CharacterHandler
        : IHandle<CCreateCharacterReqMessage>, IHandle<CDeleteCharacterReqMessage>, IHandle<CSelectCharacterReqMessage>
    {
        private readonly ILogger _logger;

        public CharacterHandler(ILogger<CharacterHandler> logger)
        {
            _logger = logger;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CCreateCharacterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            logger.Information("Creating character {@Message}", message.ToJson());

            var (_, result) = plr.CharacterManager.Create(message.Slot, message.Style.Gender,
                message.Style.Hair, message.Style.Face, message.Style.Shirt, message.Style.Pants, 0, 0);

            if (result != CharacterCreateResult.Success)
            {
                logger.Information("Failed to create character result={Result}", result);
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.CreateCharacterFailed));
            }

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CDeleteCharacterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (!plr.CharacterManager.Remove(message.Slot))
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.DeleteCharacterFailed));

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CSelectCharacterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            // Prevents player from changing characters while playing
            if (plr.Room != null && plr.State != PlayerState.Lobby &&
                plr.Room.GameRule.StateMachine.TimeState != GameTimeState.HalfTime)
            {
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
                return true;
            }

            if (!plr.CharacterManager.Select(message.Slot))
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));

            return true;
        }
    }
}
