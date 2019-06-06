using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class Room2Dto
    {
        [BlubMember(0)]
        public byte RoomId { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(EnumSerializer), typeof(byte))]
        public GameState State { get; set; }

        [BlubMember(2)]
        public Netsphere.GameRule GameRule { get; set; }

        [BlubMember(3)]
        public byte Map { get; set; }

        [BlubMember(4)]
        public byte PlayerCount { get; set; }

        [BlubMember(5)]
        public byte PlayerLimit { get; set; }

        [BlubMember(6)]
        public uint ItemLimit { get; set; }

        [BlubMember(7)]
        public string Password { get; set; }

        [BlubMember(8)]
        public string Name { get; set; }

        [BlubMember(9)]
        public byte IsSpectatingEnabled { get; set; }

        [BlubMember(10)]
        public byte Unk2 { get; set; }

        [BlubMember(11)]
        public int Unk3 { get; set; }

        [BlubMember(12)]
        public RoomSettings Settings { get; set; }

        [BlubMember(13)]
        public int Unk5 { get; set; }

        [BlubMember(14)]
        public int Unk6 { get; set; }
    }
}
