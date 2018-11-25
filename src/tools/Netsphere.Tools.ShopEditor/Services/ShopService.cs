using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia;
using LinqToDB;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Tools.ShopEditor.Models;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace Netsphere.Tools.ShopEditor.Services
{
    public class ShopService : ReactiveObject
    {
        public static ShopService Instance { get; } = new ShopService();

        private readonly IDatabaseProvider _databaseProvider;

        public IReactiveList<ShopPriceGroup> PriceGroups { get; }
        public IReactiveList<ShopEffectGroup> EffectGroups { get; }
        public IReactiveList<ShopItem> Items { get; }

        private ShopService()
        {
            _databaseProvider = AvaloniaLocator.Current.GetService<IDatabaseProvider>();
            PriceGroups = new ReactiveList<ShopPriceGroup>();
            EffectGroups = new ReactiveList<ShopEffectGroup>();
            Items = new ReactiveList<ShopItem>();
        }

        public async Task LoadFromDatabase()
        {
            PriceGroups.Clear();
            EffectGroups.Clear();
            Items.Clear();
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var priceGroupEntities = await db.PriceGroups.LoadWith(x => x.ShopPrices).ToArrayAsync();
                var priceGroups = priceGroupEntities.Select(x => new ShopPriceGroup(x));
                var effectGroupEntities = await db.EffectGroups.LoadWith(x => x.ShopEffects).ToArrayAsync();
                var effectGroups = effectGroupEntities.Select(x => new ShopEffectGroup(x));

                // I know this is ugly but somehow I cant get decent performance with joins 🤔
                IEnumerable<ShopItemEntity> itemEntities = await db.Items.ToArrayAsync();
                var itemInfoEntities = await db.ItemInfos.ToArrayAsync();
                itemEntities = itemEntities.GroupJoin(itemInfoEntities, x => x.Id, x => x.ShopItemId, (item, itemInfos) =>
                {
                    item.ItemInfos = itemInfos;
                    return item;
                });

                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    PriceGroups.AddRange(priceGroups);
                    EffectGroups.AddRange(effectGroups);
                    Items.AddRange(itemEntities.Select(x => new ShopItem(x)));
                });
            }
        }

        public async Task Delete(ShopPriceGroup priceGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.PriceGroups.Where(x => x.Id == priceGroup.Id).DeleteAsync();

            PriceGroups.Remove(priceGroup);
        }

        public async Task Delete(ShopPrice price)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.Prices.Where(x => x.Id == price.Id).DeleteAsync();

            price.PriceGroup.Prices.Remove(price);
        }

        public async Task Delete(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.EffectGroups.Where(x => x.Id == effectGroup.Id).DeleteAsync();

            EffectGroups.Remove(effectGroup);
        }

        public async Task Delete(ShopEffect effect)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.Effects.Where(x => x.Id == effect.Id).DeleteAsync();

            effect.EffectGroup.Effects.Remove(effect);
        }

        public async Task Delete(ShopItem item)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.Items.Where(x => x.Id == item.ItemNumber).DeleteAsync();

            Items.Remove(item);
        }

        public async Task Delete(ShopItemInfo itemInfo)
        {
            using (var db = _databaseProvider.Open<GameContext>())
                await db.ItemInfos.Where(x => x.Id == itemInfo.Id).DeleteAsync();

            itemInfo.Item.ItemInfos.Remove(itemInfo);
        }

        public async Task NewPriceGroup()
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var priceGroupEntity = new ShopPriceGroupEntity
                {
                    Name = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    PriceType = (byte)ItemPriceType.None
                };
                priceGroupEntity.Id = await db.InsertWithInt32IdentityAsync(priceGroupEntity);
                PriceGroups.Add(new ShopPriceGroup(priceGroupEntity));
            }
        }

        public async Task NewPrice(ShopPriceGroup priceGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var priceEntity = new ShopPriceEntity
                {
                    PriceGroupId = priceGroup.Id
                };
                priceEntity.Id = await db.InsertWithInt32IdentityAsync(priceEntity);
                priceGroup.Prices.Add(new ShopPrice(priceGroup, priceEntity));
            }
        }

        public async Task NewEffectGroup()
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var effectGroupEntity = new ShopEffectGroupEntity
                {
                    Name = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                };
                effectGroupEntity.Id = await db.InsertWithInt32IdentityAsync(effectGroupEntity);
                EffectGroups.Add(new ShopEffectGroup(effectGroupEntity));
            }
        }

        public async Task NewEffect(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var effectEntity = new ShopEffectEntity
                {
                    EffectGroupId = effectGroup.Id
                };
                effectEntity.Id = await db.InsertWithInt32IdentityAsync(effectEntity);
                effectGroup.Effects.Add(new ShopEffect(effectGroup, effectEntity));
            }
        }

        public async Task NewItem(ItemNumber itemNumber)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var itemEntity = new ShopItemEntity
                {
                    Id = itemNumber
                };
                itemEntity.Id = await db.InsertWithInt32IdentityAsync(itemEntity);
                Items.Add(new ShopItem(itemEntity));
            }
        }

        public async Task NewItemInfo(ShopItem item)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var itemInfoEntity = new ShopItemInfoEntity
                {
                    ShopItemId = item.ItemNumber,
                    EffectGroupId = EffectGroups.First().Id,
                    PriceGroupId = PriceGroups.First().Id
                };
                itemInfoEntity.Id = await db.InsertWithInt32IdentityAsync(itemInfoEntity);
                item.ItemInfos.Add(new ShopItemInfo(item, itemInfoEntity));
            }
        }

        public async Task Update(ShopPriceGroup priceGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var priceType = (byte)priceGroup.PriceType.Value;
                await db.PriceGroups.Where(x => x.Id == priceGroup.Id)
                    .Set(x => x.Name, priceGroup.Name.Value)
                    .Set(x => x.PriceType, priceType)
                    .UpdateAsync();
            }
        }

        public async Task Update(ShopPrice price)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                var periodType = (byte)price.PeriodType.Value;
                await db.Prices.Where(x => x.Id == price.Id)
                    .Set(x => x.PeriodType, periodType)
                    .Set(x => x.Period, price.Period.Value)
                    .Set(x => x.Price, price.Price.Value)
                    .Set(x => x.IsRefundable, price.IsRefundable.Value)
                    .Set(x => x.Durability, price.Durability.Value)
                    .Set(x => x.IsEnabled, price.IsEnabled.Value)
                    .UpdateAsync();
            }
        }

        public async Task Update(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                await db.EffectGroups.Where(x => x.Id == effectGroup.Id)
                    .Set(x => x.Name, effectGroup.Name.Value)
                    .UpdateAsync();
            }
        }

        public async Task Update(ShopEffect effect)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                await db.Effects.Where(x => x.Id == effect.Id)
                    .Set(x => x.Effect, effect.Effect.Value)
                    .UpdateAsync();
            }
        }

        public async Task Update(ShopItem item)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                await db.Items.Where(x => x.Id == item.ItemNumber)
                    .Set(x => x.RequiredGender, (byte)item.RequiredGender.Value)
                    .Set(x => x.RequiredLicense, (byte)item.RequiredLicense.Value)
                    .Set(x => x.Colors, item.Colors.Value)
                    .Set(x => x.UniqueColors, item.UniqueColors.Value)
                    .Set(x => x.RequiredLevel, item.RequiredLevel.Value)
                    .Set(x => x.LevelLimit, item.LevelLimit.Value)
                    .Set(x => x.RequiredMasterLevel, item.RequiredMasterLevel.Value)
                    .Set(x => x.IsOneTimeUse, item.IsOneTimeUse.Value)
                    .Set(x => x.IsDestroyable, item.IsDestroyable.Value)
                    .UpdateAsync();
            }
        }

        public async Task Update(ShopItemInfo itemInfo)
        {
            using (var db = _databaseProvider.Open<GameContext>())
            {
                await db.ItemInfos.Where(x => x.Id == itemInfo.Id)
                    .Set(x => x.EffectGroupId, itemInfo.EffectGroup.Value.Id)
                    .Set(x => x.PriceGroupId, itemInfo.PriceGroup.Value.Id)
                    .Set(x => x.DiscountPercentage, itemInfo.DiscountPercentage.Value)
                    .Set(x => x.IsEnabled, itemInfo.IsEnabled.Value)
                    .UpdateAsync();
            }
        }
    }
}
