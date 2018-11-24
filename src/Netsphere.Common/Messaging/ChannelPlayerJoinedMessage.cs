namespace Netsphere.Common.Messaging
{
    public class ChannelPlayerJoinedMessage
    {
        public ulong AccountId { get; set; }
        public uint ChannelId { get; set; }

        public ChannelPlayerJoinedMessage()
        {
        }

        public ChannelPlayerJoinedMessage(ulong accountId, uint channelId)
        {
            AccountId = accountId;
            ChannelId = channelId;
        }
    }
}
