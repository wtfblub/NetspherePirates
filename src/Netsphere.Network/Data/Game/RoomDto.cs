using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class RoomDto
    {
        [Serialize(0)]
        public uint RoomId { get; set; }

        [Serialize(1)]
        public byte ConnectingCount { get; set; }

        [Serialize(2)]
        public byte PlayerCount { get; set; } // ToDo: Did it move or devs retarded?

        [Serialize(3, typeof(EnumSerializer), typeof(byte))]
        public GameState State { get; set; }

        [Serialize(4)]
        public byte Latency { get; set; }

        [Serialize(5)]
        public MatchKey MatchKey { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Name { get; set; }

        [Serialize(7)]
        public bool HasPassword { get; set; }

        [Serialize(8)]
        public uint TimeLimit { get; set; }

        [Serialize(9)]
        public uint Unk4 { get; set; } // EnterRoomInfoDto->Unk1

        [Serialize(10)]
        public uint ScoreLimit { get; set; }

        [Serialize(11)]
        public bool IsFriendly { get; set; }

        [Serialize(12)]
        public bool IsBalanced { get; set; }

        [Serialize(13)]
        public byte MinLevel { get; set; }

        [Serialize(14)]
        public byte MaxLevel { get; set; }

        [Serialize(15)]
        public byte EquipLimit { get; set; }

        [Serialize(16)]
        public bool IsNoIntrusion { get; set; }

        [Serialize(17)]
        public byte Unk5 { get; set; } // EnterRoomInfoDto->Value

        [Serialize(18, typeof(StringSerializer))]
        public string Unk6 { get; set; }

        [Serialize(19, typeof(StringSerializer))]
        public string Unk7 { get; set; }

        [Serialize(20, typeof(StringSerializer))]
        public string Unk8 { get; set; }

        [Serialize(21, typeof(StringSerializer))]
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
