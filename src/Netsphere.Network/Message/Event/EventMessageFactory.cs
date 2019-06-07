using ProudNet.Serialization;

namespace Netsphere.Network.Message.Event
{
    public interface IEventMessage
    {
    }

    public class EventMessageFactory : MessageFactory<EventOpCode, IEventMessage>
    {
        public EventMessageFactory()
        {
            Register<ChatMessage>(EventOpCode.Chat);
            Register<EventMessageMessage>(EventOpCode.EventMessage);
            Register<ChangeTargetMessage>(EventOpCode.ChangeTarget);
            Register<ArcadeSyncMessage>(EventOpCode.ArcadeSync);
            Register<ArcadeSyncReqMessage>(EventOpCode.ArcadeSyncReq);
            Register<WaitingRoomActionSyncMessage>(EventOpCode.WaitingRoomActionSync);
            Register<FootballBallSyncMessage>(EventOpCode.FootballBallSync);
            Register<PacketMessage>(EventOpCode.Packet);
        }
    }
}
