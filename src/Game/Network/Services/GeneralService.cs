using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;

namespace Netsphere.Network.Services
{
    internal class GeneralService : MessageHandler
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

        [MessageHandler(typeof(CQuickStartReqMessage))]
        public void QuickStartHandler(GameSession session, CQuickStartReqMessage message)
        {
            //ToDo - Logic
            session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }

        [MessageHandler(typeof(CTaskRequestReqMessage))]
        public void TaskRequestHandler(GameSession session, CTaskRequestReqMessage message)
        {
            //ToDo - Logic
            session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
        }
    }
}
