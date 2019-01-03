namespace Netsphere.Common.Messaging
{
    public class PlayerDisconnectedMessage
    {
        public ulong AccountId { get; set; }

        public PlayerDisconnectedMessage()
        {
        }

        public PlayerDisconnectedMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }
}
