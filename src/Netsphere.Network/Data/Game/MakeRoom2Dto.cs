using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class MakeRoom2Dto
    {
        [BlubMember(0)]
        public Netsphere.GameRule GameRule { get; set; }

        [BlubMember(1)]
        public byte Map { get; set; }

        [BlubMember(2)]
        public byte PlayerLimit { get; set; }

        [BlubMember(3)]
        public ushort ScoreLimit { get; set; }

        [BlubMember(4)]
        [BlubSerializer(typeof(TimeLimitSerializer))]
        public TimeSpan TimeLimit { get; set; }

        [BlubMember(5)]
        public uint ItemLimit { get; set; }

        [BlubMember(6)]
        public string Name { get; set; }

        [BlubMember(7)]
        public string Password { get; set; }

        [BlubMember(8)]
        public bool IsSpectatingEnabled { get; set; }

        [BlubMember(9)]
        public byte SpectatorLimit { get; set; }

        [BlubMember(10)]
        public byte Unk3 { get; set; }

        [BlubMember(11)]
        public int Unk4 { get; set; }

        [BlubMember(12)]
        public int Unk5 { get; set; }

        [BlubMember(13)]
        public byte Unk6 { get; set; }

        [BlubMember(14)]
        public RoomRandomSettings RandomSettings { get; set; }

        [BlubMember(15)]
        public RoomSettings Settings { get; set; }

        public MakeRoom2Dto()
        {
            Name = "";
            Password = "";
        }
    }
}
