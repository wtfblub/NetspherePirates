using System;
using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class AdminService : Service
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		
        [MessageHandler(typeof(CEnableAccountStatusAckMessage))]
        public void EnableAccountStatusHandler(GameSession session, CEnableAccountStatusAckMessage message)
        {
			/* Not used yet */
            /*Logger.Info()
                .Account(session)
                .Message("CEnableAccountStatusAckMessage({0})", message.Unk)
                .Write();*/
        }
    }
}
