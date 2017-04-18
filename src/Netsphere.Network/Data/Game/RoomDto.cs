using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RoomDto
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }

        [BlubMember(1, typeof(EnumSerializer), typeof(byte))]
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
        public uint WeaponLimit { get; set; }

        [BlubMember(7, typeof(StringSerializer))]
        public string Password { get; set; }

        [BlubMember(8, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(9)]
        public byte Unk1 { get; set; }

        [BlubMember(10)]
        public byte Unk2 { get; set; }

        public RoomDto()
        {
            Name = "";
            Password = "";
        }
    }
}
