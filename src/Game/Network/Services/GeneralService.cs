using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class GeneralService : ProudMessageHandler
    {
        [MessageHandler(typeof(TimeSyncReqMessage))]
        public void TimeSyncHandler(GameSession session, TimeSyncReqMessage message)
        {
            session.SendAsync(new TimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Program.AppTime.ElapsedMilliseconds
            });
        }
    }
}
