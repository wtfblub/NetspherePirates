namespace Netsphere.Common.Messaging
{
    public class ChannelPlayerLeftMessage
    {
        public ulong AccountId { get; set; }
        public uint ChannelId { get; set; }

        public ChannelPlayerLeftMessage()
        {
        }

        public ChannelPlayerLeftMessage(ulong accountId, uint channelId)
        {
            AccountId = accountId;
            ChannelId = channelId;
        }
    }
}
