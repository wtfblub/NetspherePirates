using System.Net;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class EnterRoomInfoDto
    {
        [Serialize(0)]
        public uint RoomId { get; set; }

        [Serialize(1)]
        public MatchKey MatchKey { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
        public GameState State { get; set; }

        [Serialize(3, typeof(EnumSerializer))]
        public GameTimeState TimeState { get; set; }

        [Serialize(4)]
        public uint TimeLimit { get; set; }

        [Serialize(5)]
        public uint Unk1 { get; set; }

        [Serialize(6)]
        public uint TimeSync { get; set; }

        [Serialize(7)]
        public uint ScoreLimit { get; set; }

        [Serialize(8)]
        public bool IsFriendly { get; set; }

        [Serialize(9)]
        public bool IsBalanced { get; set; }

        [Serialize(10)]
        public byte MinLevel { get; set; }

        [Serialize(11)]
        public byte MaxLevel { get; set; }

        [Serialize(12)]
        public byte ItemLimit { get; set; }

        [Serialize(13)]
        public bool IsNoIntrusion { get; set; }

        [Serialize(14)]
        public byte Unk2 { get; set; }

        [Serialize(15, typeof(IPEndPointAddressStringSerializer))]
        public IPEndPoint RelayEndPoint { get; set; }

        [Serialize(16)]
        public bool CreatedRoom { get; set; }

        public EnterRoomInfoDto()
        {
            MatchKey = 0;
            RelayEndPoint = new IPEndPoint(0, 0);
            CreatedRoom = false;
        }
    }
}
