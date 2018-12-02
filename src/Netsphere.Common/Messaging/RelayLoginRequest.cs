using System.Net;

namespace Netsphere.Common.Messaging
{
    public class RelayLoginRequest : MessageWithGuid
    {
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public IPAddress Address;
        public uint ServerId { get; set; }
        public uint ChannelId { get; set; }
        public uint RoomId { get; set; }
    }

    public class RelayLoginResponse : MessageWithGuid
    {
        public bool OK { get; set; }
        public Account Account { get; set; }

        public RelayLoginResponse()
        {
        }

        public RelayLoginResponse(bool ok, Account account)
        {
            OK = ok;
            Account = account;
        }
    }
}
