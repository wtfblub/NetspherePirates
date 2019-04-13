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
        : IHandle<CharacterCreateReqMessage>, IHandle<CharacterDeleteReqMessage>, IHandle<CharacterSelectReqMessage>
    {
        private readonly ILogger _logger;
        private readonly EquipValidator _equipValidator;

        public CharacterHandler(ILogger<CharacterHandler> logger, EquipValidator equipValidator)
        {
            _logger = logger;
            _equipValidator = equipValidator;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CharacterCreateReqMessage message)
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
                session.Send(new ServerResultAckMessage(ServerResult.CreateCharacterFailed));
            }

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CharacterDeleteReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (!plr.CharacterManager.Remove(message.Slot))
                session.Send(new ServerResultAckMessage(ServerResult.DeleteCharacterFailed));

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CharacterSelectReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            // Prevents player from changing characters while playing
            if (plr.Room != null && plr.State != PlayerState.Lobby &&
                plr.Room.GameRule.StateMachine.TimeState != GameTimeState.HalfTime)
            {
                session.Send(new ServerResultAckMessage(ServerResult.SelectCharacterFailed));
                return true;
            }

            // Cant switch characters when ready
            if (plr.Room != null && plr.IsReady)
            {
                session.Send(new ServerResultAckMessage(ServerResult.SelectCharacterFailed));
                return true;
            }

            // Validate equip
            if (plr.Room != null && plr.State != PlayerState.Lobby &&
                plr.Room.GameRule.StateMachine.TimeState == GameTimeState.HalfTime)
            {
                var character = plr.CharacterManager[message.Slot];
                if (character != null && !_equipValidator.IsValid(character))
                {
                    session.Send(new ServerResultAckMessage(ServerResult.WearingUnusableItem));
                    return true;
                }
            }

            if (!plr.CharacterManager.Select(message.Slot))
                session.Send(new ServerResultAckMessage(ServerResult.SelectCharacterFailed));

            return true;
        }
    }
}
