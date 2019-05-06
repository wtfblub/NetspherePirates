using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RequitalGiveItemResultDto
    {
        [BlubMember(0)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; }

        public RequitalGiveItemResultDto()
        {
        }

        public RequitalGiveItemResultDto(ItemNumber itemNumber, int unk)
        {
            ItemNumber = itemNumber;
            Unk = unk;
        }
    }
}
