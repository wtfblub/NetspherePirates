using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    public class ChangeRuleDto
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Name { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Password { get; set; }

        [Serialize(2)]
        public MatchKey MatchKey { get; set; }

        [Serialize(3, typeof(TimeSpanSerializer))]
        public TimeSpan TimeLimit { get; set; }

        [Serialize(4)]
        public uint Unk1 { get; set; }

        [Serialize(5)]
        public ushort ScoreLimit { get; set; }

        [Serialize(6)]
        public int Unk2 { get; set; }

        [Serialize(7)]
        public bool IsFriendly { get; set; }

        [Serialize(8)]
        public bool IsBalanced { get; set; }

        [Serialize(9)]
        public byte ItemLimit { get; set; }

        [Serialize(10)]
        public bool IsNoIntrusion { get; set; }

        [Serialize(11)]
        public byte Unk7 { get; set; } // EnterRoomInfoDto->Unk2 ?

        public ChangeRuleDto()
        {
            Name = "";
            Password = "";
            MatchKey = 0;
        }
    }
}
