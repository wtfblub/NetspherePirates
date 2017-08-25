using System;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class AdminService : ProudMessageHandler
    {
        [MessageHandler(typeof(CAdminShowWindowReqMessage))]
        public Task ShowWindowHandler(GameSession session)
        {
            return session.SendAsync(new SAdminShowWindowAckMessage(session.Player.Account.SecurityLevel <= SecurityLevel.User));
        }

        [MessageHandler(typeof(CAdminActionReqMessage))]
        public void AdminActionHandler(GameServer server, GameSession session, CAdminActionReqMessage message)
        {
            var args = message.Command.GetArgs();
            if (!server.CommandManager.Execute(session.Player, args))
                session.Player.SendConsoleMessage(S4Color.Red + "Unknown command");
        }
    }
}
