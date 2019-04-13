using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class UserDataHandler : IHandle<UserDataOneReqMessage>
    {
        [Firewall(typeof(MustBeInChannel))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, UserDataOneReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (plr.Account.Id == message.AccountId)
            {
                session.Send(new UserDataFourAckMessage(0, plr.Map<Player, UserDataDto>()));
                return true;
            }

            if (!plr.Channel.Players.TryGetValue(message.AccountId, out var target))
                return true;

            session.Send(new UserDataFourAckMessage(0, target.Map<Player, UserDataDto>()));
            return true;
        }
    }
}
