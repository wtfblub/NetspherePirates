using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class CombiDto
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint Unk4 { get; set; }

        [BlubMember(4)]
        public ulong Unk5 { get; set; }

        [BlubMember(5)]
        public ulong Unk6 { get; set; }

        [BlubMember(6)]
        public ulong Unk7 { get; set; }

        [BlubMember(7)]
        public ulong Unk8 { get; set; }

        [BlubMember(8)]
        public ulong Unk9 { get; set; }

        [BlubMember(9, typeof(StringSerializer))]
        public string Unk10 { get; set; }

        [BlubMember(10, typeof(StringSerializer))]
        public string Unk11 { get; set; }

        [BlubMember(11, typeof(StringSerializer))]
        public string Unk12 { get; set; }

        [BlubMember(12, typeof(StringSerializer))]
        public string Unk13 { get; set; }

        public CombiDto()
        {
            Unk10 = "";
            Unk11 = "";
            Unk12 = "";
            Unk13 = "";
        }
    }
}
