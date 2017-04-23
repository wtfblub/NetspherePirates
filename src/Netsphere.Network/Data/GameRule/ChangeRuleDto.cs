using BlubLib.Serialization;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Data.GameRule
{
    [BlubContract]
    public class ChangeRuleDto
    {
        [BlubMember(0)]
        public Netsphere.GameRule GameRule { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; }

        [BlubMember(3)]
        public byte Unk3 { get; set; }

        [BlubMember(4)]
        public byte Unk4 { get; set; }

        [BlubMember(5)]
        public byte Unk5 { get; set; }

        [BlubMember(6)]
        public ushort ScoreLimit { get; set; }

        [BlubMember(7)]
        public byte Unk6 { get; set; }

        [BlubMember(8)]
        public int Unk7 { get; set; }

        [BlubMember(9)]
        public byte Unk8 { get; set; }

        [BlubMember(10)]
        public byte Unk9 { get; set; }

        [BlubMember(11)]
        public byte Unk10 { get; set; }

        [BlubMember(12)]
        public int WeaponLimit { get; set; }

        [BlubMember(13)]
        public byte Unk11 { get; set; }

        [BlubMember(14, typeof(StringSerializer))]
        public string Name { get; set; }

        [BlubMember(15, typeof(StringSerializer))]
        public string Password { get; set; }

        [BlubMember(16)]
        public byte Unk12 { get; set; }

        [BlubMember(17)]
        public byte Unk13 { get; set; }

        [BlubMember(18)]
        public byte Unk14 { get; set; }

        [BlubMember(19)]
        public int Unk15 { get; set; }

        [BlubMember(20)]
        public int Unk16 { get; set; }

        [BlubMember(21)]
        public byte Unk17 { get; set; }  // MakeRoomDto->Unk15

        public ChangeRuleDto()
        {
            Name = "";
            Password = "";
        }
    }
}
