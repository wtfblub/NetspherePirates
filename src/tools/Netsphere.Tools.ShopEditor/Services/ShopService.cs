using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia;
using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Tools.ShopEditor.Models;
using ReactiveUI;
using ReactiveUI.Legacy;
using Z.EntityFramework.Plus;

namespace Netsphere.Tools.ShopEditor.Services
{
    public class ShopService : ReactiveObject
    {
        public static ShopService Instance { get; } = new ShopService();

        private readonly DatabaseService _databaseService;

        public IReactiveList<ShopPriceGroup> PriceGroups { get; }
        public IReactiveList<ShopEffectGroup> EffectGroups { get; }
        public IReactiveList<ShopItem> Items { get; }

        private ShopService()
        {
            _databaseService = AvaloniaLocator.Current.GetService<DatabaseService>();
            PriceGroups = new ReactiveList<ShopPriceGroup>();
            EffectGroups = new ReactiveList<ShopEffectGroup>();
            Items = new ReactiveList<ShopItem>();
        }

        public async Task LoadFromDatabase()
        {
            PriceGroups.Clear();
            EffectGroups.Clear();
            Items.Clear();
            using (var db = _databaseService.Open<GameContext>())
            {
                var priceGroupEntities = await db.PriceGroups.Include(x => x.ShopPrices).ToArrayAsync();
                var priceGroups = priceGroupEntities.Select(x => new ShopPriceGroup(x));
                var effectGroupEntities = await db.EffectGroups.Include(x => x.ShopEffects).ToArrayAsync();
                var effectGroups = effectGroupEntities.Select(x => new ShopEffectGroup(x));

                // I know this is ugly but somehow I cant get decent performance with joins 🤔
                IEnumerable<ShopItemEntity> itemEntities = await db.Items.ToArrayAsync();
                var itemInfoEntities = await db.ItemInfos.ToArrayAsync();
                itemEntities = itemEntities.GroupJoin(itemInfoEntities, x => x.Id, x => x.ShopItemId, (item, itemInfos) =>
                {
                    item.ItemInfos = itemInfos.ToList();
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
            using (var db = _databaseService.Open<GameContext>())
                await db.PriceGroups.Where(x => x.Id == priceGroup.Id).DeleteAsync();

            PriceGroups.Remove(priceGroup);
        }

        public async Task Delete(ShopPrice price)
        {
            using (var db = _databaseService.Open<GameContext>())
                await db.Prices.Where(x => x.Id == price.Id).DeleteAsync();

            price.PriceGroup.Prices.Remove(price);
        }

        public async Task Delete(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseService.Open<GameContext>())
                await db.EffectGroups.Where(x => x.Id == effectGroup.Id).DeleteAsync();

            EffectGroups.Remove(effectGroup);
        }

        public async Task Delete(ShopEffect effect)
        {
            using (var db = _databaseService.Open<GameContext>())
                await db.Effects.Where(x => x.Id == effect.Id).DeleteAsync();

            effect.EffectGroup.Effects.Remove(effect);
        }

        public async Task Delete(ShopItem item)
        {
            using (var db = _databaseService.Open<GameContext>())
                await db.Items.Where(x => x.Id == item.ItemNumber).DeleteAsync();

            Items.Remove(item);
        }

        public async Task Delete(ShopItemInfo itemInfo)
        {
            using (var db = _databaseService.Open<GameContext>())
                await db.ItemInfos.Where(x => x.Id == itemInfo.Id).DeleteAsync();

            itemInfo.Item.ItemInfos.Remove(itemInfo);
        }

        public async Task NewPriceGroup()
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var priceGroupEntity = new ShopPriceGroupEntity
                {
                    Name = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    PriceType = (byte)ItemPriceType.None
                };

                db.PriceGroups.Add(priceGroupEntity);
                await db.SaveChangesAsync();
                PriceGroups.Add(new ShopPriceGroup(priceGroupEntity));
            }
        }

        public async Task NewPrice(ShopPriceGroup priceGroup)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var priceEntity = new ShopPriceEntity
                {
                    PriceGroupId = priceGroup.Id
                };
                db.Prices.Add(priceEntity);
                await db.SaveChangesAsync();
                priceGroup.Prices.Add(new ShopPrice(priceGroup, priceEntity));
            }
        }

        public async Task NewEffectGroup()
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var effectGroupEntity = new ShopEffectGroupEntity
                {
                    Name = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                };
                db.EffectGroups.Add(effectGroupEntity);
                await db.SaveChangesAsync();
                EffectGroups.Add(new ShopEffectGroup(effectGroupEntity));
            }
        }

        public async Task NewEffect(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var effectEntity = new ShopEffectEntity
                {
                    EffectGroupId = effectGroup.Id
                };
                db.Effects.Add(effectEntity);
                await db.SaveChangesAsync();
                effectGroup.Effects.Add(new ShopEffect(effectGroup, effectEntity));
            }
        }

        public async Task NewItem(ItemNumber itemNumber)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var itemEntity = new ShopItemEntity
                {
                    Id = itemNumber
                };
                db.Items.Add(itemEntity);
                await db.SaveChangesAsync();
                Items.Add(new ShopItem(itemEntity));
            }
        }

        public async Task NewItemInfo(ShopItem item)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var itemInfoEntity = new ShopItemInfoEntity
                {
                    ShopItemId = item.ItemNumber,
                    EffectGroupId = EffectGroups.First().Id,
                    PriceGroupId = PriceGroups.First().Id
                };
                db.ItemInfos.Add(itemInfoEntity);
                await db.SaveChangesAsync();
                item.ItemInfos.Add(new ShopItemInfo(item, itemInfoEntity));
            }
        }

        public async Task Update(ShopPriceGroup priceGroup)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var priceType = (byte)priceGroup.PriceType.Value;
                await db.PriceGroups
                    .Where(x => x.Id == priceGroup.Id)
                    .UpdateAsync(x => new ShopPriceGroupEntity
                    {
                        Name = priceGroup.Name.Value,
                        PriceType = priceType
                    });
            }
        }

        public async Task Update(ShopPrice price)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var periodType = (byte)price.PeriodType.Value;
                await db.Prices
                    .Where(x => x.Id == price.Id)
                    .UpdateAsync(x => new ShopPriceEntity
                    {
                        PeriodType = periodType,
                        Period = (int)price.Period.Value,
                        Price = price.Price.Value,
                        IsRefundable = price.IsRefundable.Value,
                        Durability = price.Durability.Value,
                        IsEnabled = price.IsEnabled.Value
                    });
            }
        }

        public async Task Update(ShopEffectGroup effectGroup)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.EffectGroups
                    .Where(x => x.Id == effectGroup.Id)
                    .UpdateAsync(x => new ShopEffectGroupEntity
                    {
                        Name = effectGroup.Name.Value,
                        PreviewEffect = effectGroup.PreviewEffect.Value
                    });
            }
        }

        public async Task Update(ShopEffect effect)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.Effects
                    .Where(x => x.Id == effect.Id)
                    .UpdateAsync(x => new ShopEffectEntity
                    {
                        Effect = effect.Effect.Value
                    });
            }
        }

        public async Task Update(ShopItem item)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.Items
                    .Where(x => x.Id == item.ItemNumber)
                    .UpdateAsync(x => new ShopItemEntity
                    {
                        RequiredGender = (byte)item.RequiredGender.Value,
                        Colors = item.Colors.Value,
                        UniqueColors = item.UniqueColors.Value,
                        RequiredLevel = item.RequiredLevel.Value,
                        LevelLimit = item.LevelLimit.Value,
                        RequiredMasterLevel = item.RequiredMasterLevel.Value,
                        IsOneTimeUse = item.IsOneTimeUse.Value,
                        IsDestroyable = item.IsDestroyable.Value,
                        MainTab = item.MainTab.Value,
                        SubTab = item.SubTab.Value
                    });
            }
        }

        public async Task Update(ShopItemInfo itemInfo)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.ItemInfos
                    .Where(x => x.Id == itemInfo.Id)
                    .UpdateAsync(x => new ShopItemInfoEntity
                    {
                        EffectGroupId = itemInfo.EffectGroup.Value.Id,
                        PriceGroupId = itemInfo.PriceGroup.Value.Id,
                        DiscountPercentage = itemInfo.DiscountPercentage.Value,
                        IsEnabled = itemInfo.IsEnabled.Value
                    });
            }
        }
    }
}
