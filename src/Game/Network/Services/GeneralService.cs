using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class GeneralService : ProudMessageHandler
    {
        [MessageHandler(typeof(CTimeSyncReqMessage))]
        public void TimeSyncHandler(GameSession session, CTimeSyncReqMessage message)
        {
            session.SendAsync(new STimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Program.AppTime.ElapsedMilliseconds
            });
        }
    }
}
