using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Game
{
    public class MakeRoomDto
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Name { get; set; }

        [Serialize(1)]
        public MatchKey MatchKey { get; set; }

        [Serialize(2)]
        public byte TimeLimit { get; set; }

        [Serialize(3)]
        public uint Unk1 { get; set; }

        [Serialize(4)]
        public ushort ScoreLimit { get; set; }

        [Serialize(5)]
        public uint Unk2 { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Password { get; set; }

        [Serialize(7)]
        public bool IsFriendly { get; set; }

        [Serialize(8)]
        public bool IsBalanced { get; set; }

        [Serialize(9)]
        public byte MinLevel { get; set; }

        [Serialize(10)]
        public byte MaxLevel { get; set; }

        [Serialize(11)]
        public byte EquipLimit { get; set; }

        [Serialize(12)]
        public bool IsNoIntrusion { get; set; }

        [Serialize(13)]
        public byte Unk3 { get; set; } // EnterRoomInfoDto->Value, RoomDto->Unk4

        public MakeRoomDto()
        {
            MatchKey = 0;
            Name = "";
            Password = "";
        }
    }
}
