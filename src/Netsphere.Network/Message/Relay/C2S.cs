using BlubLib.Serialization;
using Netsphere.Network.Data.Relay;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Relay
{
    [BlubContract]
    public class CRequestLoginMessage : RelayMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(2, typeof(RoomLocationSerializer))]
        public RoomLocation RoomLocation { get; set; }

        [BlubMember(3)]
        public bool CreatedRoom { get; set; }
    }
}