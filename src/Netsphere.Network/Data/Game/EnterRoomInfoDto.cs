using System.Net;
using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class EnterRoomInfoDto
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }

        [BlubMember(1)]
        public MatchKey MatchKey { get; set; }

        [BlubMember(2)]
        public GameState State { get; set; }

        [BlubMember(3)]
        public GameTimeState TimeState { get; set; }

        [BlubMember(4)]
        public uint TimeLimit { get; set; }

        [BlubMember(5)]
        public uint Unk1 { get; set; }

        [BlubMember(6)]
        public uint TimeSync { get; set; }

        [BlubMember(7)]
        public uint ScoreLimit { get; set; }

        [BlubMember(8)]
        public bool IsFriendly { get; set; }

        [BlubMember(9)]
        public bool IsBalanced { get; set; }

        [BlubMember(10)]
        public byte MinLevel { get; set; }

        [BlubMember(11)]
        public byte MaxLevel { get; set; }

        [BlubMember(12)]
        public byte ItemLimit { get; set; }

        [BlubMember(13)]
        public bool IsNoIntrusion { get; set; }

        [BlubMember(14)]
        public byte Unk2 { get; set; }

        [BlubMember(15, typeof(IPEndPointAddressStringSerializer))]
        public IPEndPoint RelayEndPoint { get; set; }

        [BlubMember(16)]
        public bool CreatedRoom { get; set; }

        public EnterRoomInfoDto()
        {
            MatchKey = 0;
            RelayEndPoint = new IPEndPoint(0, 0);
            CreatedRoom = false;
        }
    }
}
