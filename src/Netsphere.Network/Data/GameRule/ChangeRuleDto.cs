using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ChangeRuleDto
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Password { get; set; }

        [BlubMember(2)]
        public MatchKey MatchKey { get; set; }

        [BlubMember(3, typeof(TimeSpanSerializer))]
        public TimeSpan TimeLimit { get; set; }

        [BlubMember(4)]
        public uint Unk1 { get; set; }

        [BlubMember(5)]
        public ushort ScoreLimit { get; set; }

        [BlubMember(6)]
        public int Unk2 { get; set; }

        [BlubMember(7)]
        public bool IsFriendly { get; set; }

        [BlubMember(8)]
        public bool IsBalanced { get; set; }

        [BlubMember(9)]
        public byte ItemLimit { get; set; }

        [BlubMember(10)]
        public bool IsNoIntrusion { get; set; }

        [BlubMember(11)]
        public byte Unk7 { get; set; } // EnterRoomInfoDto->Unk2 ?

        public ChangeRuleDto()
        {
            Name = "";
            Password = "";
            MatchKey = 0;
        }
    }
}
