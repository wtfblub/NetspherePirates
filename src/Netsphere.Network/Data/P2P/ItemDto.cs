using BlubLib.Serialization;

namespace Netsphere.Network.Data.P2P
{
    [BlubContract]
    public class ItemDto
    {
        [BlubMember(0)]
        public ItemNumber ItemNumber { get; set; }

        [BlubMember(1)]
        public int Color { get; set; }

        public ItemDto()
        {
            ItemNumber = 0;
        }

        public ItemDto(ItemNumber itemNumber, int color)
        {
            ItemNumber = itemNumber;
            Color = color;
        }

        public override string ToString()
        {
            return ItemNumber.ToString();
        }
    }
}
