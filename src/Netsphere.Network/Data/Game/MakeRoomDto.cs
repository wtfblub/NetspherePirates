using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class MakeRoomDto
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(1)]
        public MatchKey MatchKey { get; set; }

        [BlubMember(2)]
        public byte TimeLimit { get; set; }

        [BlubMember(3)]
        public uint Unk1 { get; set; }

        [BlubMember(4)]
        public ushort ScoreLimit { get; set; }

        [BlubMember(5)]
        public uint Unk2 { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Password { get; set; }

        [BlubMember(7)]
        public bool IsFriendly { get; set; }

        [BlubMember(8)]
        public bool IsBalanced { get; set; }

        [BlubMember(9)]
        public byte MinLevel { get; set; }

        [BlubMember(10)]
        public byte MaxLevel { get; set; }

        [BlubMember(11)]
        public byte EquipLimit { get; set; }

        [BlubMember(12)]
        public bool IsNoIntrusion { get; set; }

        [BlubMember(13)]
        public byte Unk3 { get; set; } // EnterRoomInfoDto->Value, RoomDto->Unk4

        public MakeRoomDto()
        {
            MatchKey = 0;
            Name = "";
            Password = "";
        }
    }
}
