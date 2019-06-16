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
        public Netsphere.GameRule GameRule { get; set; }

        [BlubMember(2)]
        public byte MapId { get; set; }

        [BlubMember(3)]
        public byte PlayerLimit { get; set; }

        [BlubMember(4)]
        public GameTimeState TimeState { get; set; }

        [BlubMember(5)]
        public GameState State { get; set; }

        [BlubMember(6)]
        public uint TimeLimit { get; set; }

        [BlubMember(7)]
        public int Unk1 { get; set; }

        [BlubMember(8)]
        public uint TimeSync { get; set; }

        [BlubMember(9)]
        public uint ScoreLimit { get; set; }

        [BlubMember(10)]
        public byte Unk2 { get; set; }

        [BlubMember(11)]
        [BlubSerializer(typeof(IPEndPointAddressStringSerializer))]
        public IPEndPoint RelayEndPoint { get; set; }

        [BlubMember(13)]
        public bool CreatedRoom { get; set; }

        [BlubMember(14)]
        public int Unk4 { get; set; }

        public EnterRoomInfoDto()
        {
            RelayEndPoint = new IPEndPoint(0, 0);
        }
    }
}
