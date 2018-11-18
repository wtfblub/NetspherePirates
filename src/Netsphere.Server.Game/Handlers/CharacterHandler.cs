using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CCreateCharacterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            plr.Logger.LogInformation("Creating character {@Message}", message.ToJson());

            var (_, result) = plr.CharacterManager.Create(message.Slot, message.Style.Gender,
                message.Style.Hair, message.Style.Face, message.Style.Shirt, message.Style.Pants, 0, 0);

            if (result != CharacterCreateResult.Success)
            {
                plr.Logger.LogInformation("Failed to create character result={Result}", result);
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

            // TODO Need room implementation first
            // Prevents player from changing characters while playing
            /*if (plr.Room != null && plr.RoomInfo.State != PlayerState.Lobby &&
                !plr.Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.HalfTime))
            {
                session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));
                return;
            }*/

            if (!plr.CharacterManager.Select(message.Slot))
                await session.SendAsync(new SServerResultInfoAckMessage(ServerResult.SelectCharacterFailed));

            return true;
        }
    }
}
