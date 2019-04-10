using System;
using System.Threading.Tasks;
using BlubLib;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class MiscHandler
        : IHandle<TimeSyncReqMessage>, IHandle<AdminShowWindowReqMessage>, IHandle<AdminActionReqMessage>
    {
        private readonly CommandService _commandService;

        public MiscHandler(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, TimeSyncReqMessage message)
        {
            context.Session.Send(new STimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Environment.TickCount
            });

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AdminShowWindowReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            session.Send(new SAdminShowWindowAckMessage(plr.Account.SecurityLevel <= SecurityLevel.User));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AdminActionReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            await _commandService.Execute(plr, message.Command.GetArgs());
            return true;
        }
    }
}
