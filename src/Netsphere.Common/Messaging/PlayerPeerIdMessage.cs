namespace Netsphere.Common.Messaging
{
    public class PlayerPeerIdMessage
    {
        public ulong AccountId { get; set; }
        public PeerId PeerId { get; set; }

        public PlayerPeerIdMessage()
        {
        }

        public PlayerPeerIdMessage(ulong accountId, PeerId peerId)
        {
            AccountId = accountId;
            PeerId = peerId;
        }
    }
}
