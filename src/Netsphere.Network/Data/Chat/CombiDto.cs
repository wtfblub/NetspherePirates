using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Data.Chat
{
    public class CombiDto
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }

        [Serialize(4)]
        public ulong Unk5 { get; set; }

        [Serialize(5)]
        public ulong Unk6 { get; set; }

        [Serialize(6)]
        public ulong Unk7 { get; set; }

        [Serialize(7)]
        public ulong Unk8 { get; set; }

        [Serialize(8)]
        public ulong Unk9 { get; set; }

        [Serialize(9, typeof(StringSerializer))]
        public string Unk10 { get; set; }

        [Serialize(10, typeof(StringSerializer))]
        public string Unk11 { get; set; }

        [Serialize(11, typeof(StringSerializer))]
        public string Unk12 { get; set; }

        [Serialize(12, typeof(StringSerializer))]
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
