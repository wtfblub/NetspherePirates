using Netsphere;

namespace WebApi.Models
{
    public class PlayerItemDto
    {
        public ulong Id { get; set; }
        public ItemDto Item { get; set; }
        public ItemPriceType PriceType { get; set; }
        public ItemPeriodType PeriodType { get; set; }
        public int Period { get; set; }
        public int Color { get; set; }
        public uint Effect { get; set; }
        public long PurchaseTimestamp { get; set; }
        public int Durability { get; set; }
        public int Count { get; set; }
    }
}
