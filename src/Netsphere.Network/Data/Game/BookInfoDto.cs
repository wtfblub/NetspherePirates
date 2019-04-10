using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class BookInfoDto
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        [BlubSerializer(typeof(FixedArraySerializer), 6)]
        public int[] Unk4 { get; set; }

        public BookInfoDto()
        {
            Unk4 = new int[6];
        }
    }
}
