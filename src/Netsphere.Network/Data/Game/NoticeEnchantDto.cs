using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class NoticeEnchantDto
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }
    }
}
