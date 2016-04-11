namespace Netsphere.Shop
{
    internal class BuyItemInfo
    {
        public ShopItem ShopItem { get; set; }
        public ShopItemInfo ShopItemInfo { get; set; }
        public ShopPrice Price { get; set; }
        public byte Color { get; set; }
        public uint Effect { get; set; }
    }
}
