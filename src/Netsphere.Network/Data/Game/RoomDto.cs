using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RoomDto
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }

        [BlubMember(1)]
        public byte ConnectingCount { get; set; }

        [BlubMember(2)]
        public byte PlayerCount { get; set; } // ToDo: Did it move or devs retarded?

        [BlubMember(3)]
        [BlubSerializer(typeof(EnumSerializer), typeof(byte))]
        public GameState State { get; set; }

        [BlubMember(4)]
        public byte Latency { get; set; }

        [BlubMember(5)]
        public MatchKey MatchKey { get; set; }

        [BlubMember(6)]
        public string Name { get; set; }

        [BlubMember(7)]
        public bool HasPassword { get; set; }

        [BlubMember(8)]
        public uint TimeLimit { get; set; }

        [BlubMember(9)]
        public uint Unk4 { get; set; } // EnterRoomInfoDto->Unk1

        [BlubMember(10)]
        public uint ScoreLimit { get; set; }

        [BlubMember(11)]
        public bool IsFriendly { get; set; }

        [BlubMember(12)]
        public bool IsBalanced { get; set; }

        [BlubMember(13)]
        public byte MinLevel { get; set; }

        [BlubMember(14)]
        public byte MaxLevel { get; set; }

        [BlubMember(15)]
        public EquipLimit EquipLimit { get; set; }

        [BlubMember(16)]
        public bool IsNoIntrusion { get; set; }

        [BlubMember(17)]
        public byte Unk5 { get; set; } // EnterRoomInfoDto->Value

        [BlubMember(18)]
        public string Unk6 { get; set; }

        [BlubMember(19)]
        public string Unk7 { get; set; }

        [BlubMember(20)]
        public string Unk8 { get; set; }

        [BlubMember(21)]
        public string Unk9 { get; set; }

        public RoomDto()
        {
            MatchKey = 0;
            Name = "";
            Unk6 = "";
            Unk7 = "";
            Unk8 = "";
            Unk9 = "";
        }
    }
}
