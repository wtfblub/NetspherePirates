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
			//ToDo - get random room with desired gamerule
			//var plr = session.Player;
			//var gamerule = (GameRule) message.Unk;
			//var roomcount = plr.Channel.RoomManager.Count;

			//for(uint i = 0; i < roomcount; i++)
			//{
			//	var room = plr.Channel.RoomManager.Get(i);
			//	if (room.GameRuleManager.GameRule.GameRule == gamerule)
			//	{
			//		room.Join(plr);
			//		return;
			//	}
			//}

			session.Send(new SServerResultInfoAckMessage(ServerResult.QuickJoinFailed));
		}

		[MessageHandler(typeof(CTaskRequestReqMessage))]
		public void TaskRequestHandler(GameSession session, CTaskRequestReqMessage message)
		{
			//TODO
			session.Send(new SServerResultInfoAckMessage(ServerResult.FailedToRequestTask));
		}
	}
}
