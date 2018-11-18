using System;
using System.Threading.Tasks;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class MiscHandler : IHandle<CTimeSyncReqMessage>, IHandle<CAdminShowWindowReqMessage>
    {
        public async Task<bool> OnHandle(MessageContext context, CTimeSyncReqMessage message)
        {
            await context.Session.SendAsync(new STimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Environment.TickCount
            });

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CAdminShowWindowReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            await session.SendAsync(new SAdminShowWindowAckMessage(plr.Account.SecurityLevel <= SecurityLevel.User));
            return true;
        }
    }
}
