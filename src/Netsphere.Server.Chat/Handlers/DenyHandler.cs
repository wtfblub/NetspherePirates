using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class DenyHandler : IHandle<CDenyChatReqMessage>
    {
        private readonly PlayerManager _playerManager;

        public DenyHandler(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CDenyChatReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (message.Deny.AccountId == plr.Account.Id)
                return true;

            Deny deny;
            switch (message.Action)
            {
                case DenyAction.Add:
                    if (plr.Ignore.Contains(message.Deny.AccountId))
                        return true;

                    var target = _playerManager[message.Deny.AccountId];
                    if (target == null)
                        return true;

                    deny = plr.Ignore.Add(target.Account.Id, target.Account.Nickname);
                    session.Send(new SDenyChatAckMessage(0, DenyAction.Add, deny.Map<Deny, DenyDto>()));
                    break;

                case DenyAction.Remove:
                    deny = plr.Ignore[message.Deny.AccountId];
                    if (deny == null)
                        return true;

                    plr.Ignore.Remove(message.Deny.AccountId);
                    session.Send(new SDenyChatAckMessage(0, DenyAction.Remove, deny.Map<Deny, DenyDto>()));
                    break;
            }

            return true;
        }
    }
}
