using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;

namespace Netsphere.Network.Services
{
    internal class GeneralService : Service
    {
        [MessageHandler(typeof(CTimeSyncReqMessage))]
        public void TimeSyncHandler(GameSession session, CTimeSyncReqMessage message)
        {
            session.Send(new STimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Program.AppTime.ElapsedMilliseconds
            });
        }
    }
}
