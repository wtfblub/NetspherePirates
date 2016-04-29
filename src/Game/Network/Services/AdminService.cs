using System;
using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;

namespace Netsphere.Network.Services
{
    internal class AdminService : MessageHandler
    {
        [MessageHandler(typeof(CAdminShowWindowReqMessage))]
        public void ShowWindowHandler(GameSession session)
        {
            session.Send(new SAdminShowWindowAckMessage(session.Player.Account.SecurityLevel > SecurityLevel.User));
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
