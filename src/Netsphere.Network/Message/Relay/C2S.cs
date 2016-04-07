using BlubLib.Serialization;
using Netsphere.Network.Data.Relay;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Relay
{
    public class CRequestLoginMessage : RelayMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2, typeof(RoomLocationSerializer))]
        public RoomLocation RoomLocation { get; set; }

        [Serialize(3)]
        public bool CreatedRoom { get; set; }
    }
}