using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class PlayerInventory : IReadOnlyCollection<PlayerItem>
    {
        private readonly GameDataService _gameDataService;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly ConcurrentDictionary<ulong, PlayerItem> _items;
        private readonly ConcurrentStack<PlayerItem> _itemsToRemove;

        public Player Player { get; private set; }
        public int Count => _items.Count;

        /// <summary>
        /// Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem this[ulong id] => GetItem(id);

        public PlayerInventory(GameDataService gameDataService, IdGeneratorService idGeneratorService)
        {
            _gameDataService = gameDataService;
            _idGeneratorService = idGeneratorService;
            _items = new ConcurrentDictionary<ulong, PlayerItem>();
            _itemsToRemove = new ConcurrentStack<PlayerItem>();
        }

        internal void Initialize(Player plr, PlayerEntity entity)
        {
            Player = plr;

            foreach (var item in entity.Items.Select(x => new PlayerItem(_gameDataService, this, x)))
                _items.TryAdd(item.Id, item);
        }

        /// <summary>
        /// Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem GetItem(ulong id)
        {
            _items.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem Create(ItemNumber itemNumber, ItemPriceType priceType, ItemPeriodType periodType, ushort period,
            byte color, uint effect, uint count)
        {
            // TODO Remove exceptions and instead return a error code

            var shopItemInfo = _gameDataService.GetShopItemInfo(itemNumber, priceType);
            if (shopItemInfo == null)
                throw new ArgumentException("Item not found");

            var price = shopItemInfo.PriceGroup.GetPrice(periodType, period);
            if (price == null)
                throw new ArgumentException("Price not found");

            return Create(shopItemInfo, price, color, effect, count);
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        public PlayerItem Create(ShopItemInfo shopItemInfo, ShopPrice price, byte color, uint effect, uint count)
        {
            var itemId = _idGeneratorService.GetNextId(IdKind.Item);
            var item = new PlayerItem(_gameDataService, this,
                itemId, shopItemInfo, price, color, effect, DateTimeOffset.Now, count);
            _items.TryAdd(item.Id, item);
            Player.Session.Send(new SInventoryActionAckMessage(InventoryAction.Add, item.Map<PlayerItem, ItemDto>()));
            return item;
        }

        /// <summary>
        /// Removes the item from the inventory
        /// </summary>
        public bool Remove(PlayerItem item)
        {
            return Remove(item.Id);
        }

        /// <summary>
        /// Removes the item from the inventory
        /// </summary>
        public bool Remove(ulong id)
        {
            var item = GetItem(id);
            if (item == null)
                return false;

            _items.Remove(item.Id);
            if (item.Exists)
                _itemsToRemove.Push(item);

            Player.Session.Send(new SDeleteItemInventoryAckMessage(item.Id));
            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_itemsToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_itemsToRemove.TryPop(out var itemToRemove))
                    idsToRemove.Add((long)itemToRemove.Id);

                await db.PlayerItems.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var item in _items.Values)
            {
                if (!item.Exists)
                {
                    db.PlayerItems.Add(new PlayerItemEntity
                    {
                        Id = (long)item.Id,
                        PlayerId = (int)Player.Account.Id,
                        ShopItemInfoId = item.GetShopItemInfo().Id,
                        ShopPriceId = item.GetShopItemInfo().PriceGroup.GetPrice(item.PeriodType, item.Period).Id,
                        Effect = item.Effect,
                        Color = item.Color,
                        PurchaseDate = item.PurchaseDate.ToUnixTimeSeconds(),
                        Durability = item.Durability,
                        Count = (int)item.Count
                    });

                    item.SetExistsState(true);
                }
                else
                {
                    if (!item.IsDirty)
                        continue;

                    db.Update(new PlayerItemEntity
                    {
                        Id = (long)item.Id,
                        PlayerId = (int)Player.Account.Id,
                        ShopItemInfoId = item.GetShopItemInfo().Id,
                        ShopPriceId = item.GetShopPrice().Id,
                        Effect = item.Effect,
                        Color = item.Color,
                        PurchaseDate = item.PurchaseDate.ToUnixTimeSeconds(),
                        Durability = item.Durability,
                        Count = (int)item.Count
                    });

                    item.SetDirtyState(false);
                }
            }
        }

        public bool Contains(ulong id)
        {
            return _items.ContainsKey(id);
        }

        public IEnumerator<PlayerItem> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
