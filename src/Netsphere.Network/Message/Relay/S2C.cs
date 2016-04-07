using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Relay
{
    public class SEnterLoginPlayerMessage : RelayMessage
    {
        [Serialize(0)]
        public uint HostId { get; set; } // Not sure, but proudnet thing for sure

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(2, typeof(StringSerializer))]
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

    public class SNotifyLoginResultMessage : RelayMessage
    {
        [Serialize(0)]
        public int Result { get; set; }

        public SNotifyLoginResultMessage()
        { }

        public SNotifyLoginResultMessage(int result)
        {
            Result = result;
        }
    }
}
