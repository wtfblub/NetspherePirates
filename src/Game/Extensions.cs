using System.Collections.Generic;
using System.IO;
using BlubLib.IO;
using Netsphere.Network;
using Netsphere.Shop;
using ProudNet;
using Serilog;

namespace Netsphere
{
    internal static class Extensions
    {
        public static ILogger ForAccount(this ILogger logger, ulong id, string user, SecurityLevel securityLevel = SecurityLevel.User)
        {
            return logger
                .ForContext("account_id", id)
                .ForContext("account_user", user)
                .ForContext("account_level", securityLevel);
        }

        public static ILogger ForAccount(this ILogger logger, Account account)
        {
            return logger.ForAccount(account.Id, account.Username, account.SecurityLevel);
        }

        public static ILogger ForAccount(this ILogger logger, Player player)
        {
            return logger.ForAccount(player.Account);
        }

        public static ILogger ForAccount(this ILogger logger, GameSession session)
        {
            return session.IsLoggedIn() ? logger.ForAccount(session.Player) : logger;
        }

        public static ILogger ForAccount(this ILogger logger, ChatSession session)
        {
            return session.IsLoggedIn() ? logger.ForAccount(session.GameSession.Player) : logger;
        }

        public static ILogger ForAccount(this ILogger logger, RelaySession session)
        {
            return session.IsLoggedIn() ? logger.ForAccount(session.GameSession.Player) : logger;
        }

        public static bool IsLoggedIn(this GameSession session)
        {
            return !string.IsNullOrWhiteSpace(session?.Player?.Account.Nickname);
        }

        public static bool IsLoggedIn(this ChatSession session)
        {
            return !string.IsNullOrWhiteSpace(session?.GameSession?.Player?.Account.Nickname);
        }

        public static bool IsLoggedIn(this RelaySession session)
        {
            return !string.IsNullOrWhiteSpace(session?.GameSession?.Player?.Account.Nickname);
        }

        public static bool IsLoggedIn(this Player plr)
        {
            return plr.Session.IsLoggedIn() && plr.ChatSession.IsLoggedIn();
        }

        public static void Serialize(this BinaryWriter w, ICollection<ShopPriceGroup> value)
        {
            w.Write(value.Count);
            foreach (var group in value)
            {
                w.WriteProudString(@group.Id.ToString());
                w.WriteEnum(group.PriceType);

                w.Write(group.Prices.Count);
                foreach (var price in group.Prices)
                {
                    w.WriteEnum(price.PeriodType);
                    w.Write(price.Period);
                    w.Write(price.Price);
                    w.Write(price.CanRefund);
                    w.Write(price.Durability);
                    w.Write(price.IsEnabled);
                }
            }
        }

        public static void Serialize(this BinaryWriter w, ICollection<ShopEffectGroup> value)
        {
            w.Write(value.Count);
            foreach (var group in value)
            {
                w.Write(@group.Id);

                w.Write(group.Effects.Count);
                foreach (var effect in group.Effects)
                    w.Write(effect.Effect);
            }
        }

        public static void Serialize(this BinaryWriter w, ICollection<ShopItem> value)
        {
            w.Write(value.Count);
            foreach (var item in value)
            {
                w.Write(item.ItemNumber);

                switch (item.Gender)
                {
                    case Gender.Female:
                        w.Write((uint)1);
                        break;

                    case Gender.Male:
                        w.Write((uint)0);
                        break;

                    case Gender.None:
                        w.Write((uint)2);
                        break;
                }
                //w.Write((uint)item.Gender);
                w.Write((ushort)item.License);
                w.Write((ushort)item.ColorGroup);
                w.Write((ushort)item.UniqueColorGroup);
                w.Write((ushort)item.MinLevel);
                w.Write((ushort)item.MaxLevel);
                w.Write((ushort)item.MasterLevel);
                w.Write(0); // RepairCost
                w.Write(item.IsOneTimeUse);
                w.Write(item.IsDestroyable);
                w.Write((ushort)item.MainTab);
                w.Write((ushort)item.SubTab);
                w.Write((ushort)0); // shop_order

                w.Write(item.ItemInfos.Count);
                foreach (var info in item.ItemInfos)
                {
                    w.WriteProudString(info.IsEnabled ? "on" : "off");
                    w.WriteEnum(info.PriceGroup.PriceType);
                    w.Write((ushort)info.Discount);
                    w.WriteProudString(info.PriceGroup.Id.ToString());
                    w.Write(info.EffectGroup.Effects.Count > 0 ? info.EffectGroup.Id : 0);
                }
            }
        }

        // ToDo
        //public static void Serialize(this BinaryWriter w, ICollection<ShopUniqueItemDto> value)
        //{
        //    w.Write(value.Count);
        //    foreach (var item in value)
        //    {
        //        w.Write((uint)item.item_number);
        //        w.Write((uint)item.price_type);
        //        w.Write((ushort)item.discount);
        //        w.Write((uint)item.period_type);
        //        w.Write((ushort)item.period);
        //        w.Write((byte)item.color);
        //        w.WriteProudString(item.is_enabled > 0 ? "on" : "off");
        //        w.Write(item.can_refund);
        //        w.Write((int)item.reward);
        //        w.WriteProudString(""); // ToDo StartDate
        //        w.WriteProudString(""); // ToDo EndDate
        //    }
        //}
    }
}
