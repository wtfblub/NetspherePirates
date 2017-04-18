using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class ItemEffectDto
    {
        [BlubMember(0)]
        public uint Effect { get; set; }

        [BlubMember(1)]
        public int Unk1 { get; set; }

        [BlubMember(2)]
        public long Unk2 { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        public ItemEffectDto()
        {
            Unk1 = -1;
        }
    }
}
