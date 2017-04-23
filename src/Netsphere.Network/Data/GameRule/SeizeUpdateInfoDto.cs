using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class SeizeUpdateInfoDto
    {
        [BlubMember(0)]
        public short Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        [BlubMember(3)]
        public byte Unk4 { get; set; }

        [BlubMember(4)]
        public short Unk5 { get; set; }

        [BlubMember(5)]
        public short Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        [BlubMember(9)]
        public long Unk10 { get; set; }

        [BlubMember(10, typeof(ArrayWithIntPrefixSerializer))]
        public long[] Unk11 { get; set; }

        [BlubMember(11)]
        public short Unk12 { get; set; }

        public SeizeUpdateInfoDto()
        {
            Unk11 = Array.Empty<long>();
        }
    }
}
