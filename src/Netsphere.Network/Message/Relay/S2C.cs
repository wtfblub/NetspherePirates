using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Relay
{
    [BlubContract]
    public class SEnterLoginPlayerMessage : RelayMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; } // Not sure, but proudnet thing for sure

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Nickname { get; set; }

        public SEnterLoginPlayerMessage()
        {
            Nickname = "";
        }

        public SEnterLoginPlayerMessage(uint hostId, ulong accountId, string nickname)
        {
            HostId = hostId;
            AccountId = accountId;
            Nickname = nickname;
        }
    }

    [BlubContract]
    public class SNotifyLoginResultMessage : RelayMessage
    {
        [BlubMember(0)]
        public int Result { get; set; }

        public SNotifyLoginResultMessage()
        { }

        public SNotifyLoginResultMessage(int result)
        {
            Result = result;
        }
    }
}
