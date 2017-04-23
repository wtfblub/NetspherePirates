using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ItemDropAckDto
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        // if Unk2 != 4 -- 6 bytes
        [BlubMember(4, typeof(FixedArraySerializer), 6)]
        public byte[] Unk5 { get; set; }

        // if Unk3 != 1
        [BlubMember(5)]
        public long Unk6 { get; set; }

        // else
        [BlubMember(6)]
        public long Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public int Unk9 { get; set; }

        [BlubMember(9)]
        public int Unk10 { get; set; }

        [BlubMember(10, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk11 { get; set; }

        [BlubMember(11)]
        public short Unk12 { get; set; }

        [BlubMember(12)]
        public short Unk13 { get; set; }

        public bool ShouldSerializeUnk5()
        {
            return Unk2 != 4;
        }

        public bool ShouldSerializeUnk6()
        {
            return Unk3 != 1;
        }

        public bool ShouldSerializeUnk7()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk8()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk9()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk10()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk11()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk12()
        {
            return Unk3 == 1;
        }

        public bool ShouldSerializeUnk13()
        {
            return Unk3 == 1;
        }
    }
}
