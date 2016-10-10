using System.Collections.Generic;
using System.IO;
using Netsphere.Network;
using Netsphere.Shop;
using NLog.Fluent;
using ProudNet;

namespace Netsphere
{
    internal static class Extensions
    {
        public static LogBuilder Account(this LogBuilder builder, ulong id, string user, SecurityLevel securityLevel = SecurityLevel.User)
        {
            builder.LogEventInfo.Properties["account_id"] = id;
            builder.LogEventInfo.Properties["account_user"] = user;
            builder.LogEventInfo.Properties["account_level"] = securityLevel;
            return builder;
        }

        public static LogBuilder Account(this LogBuilder builder, Account account)
        {
            return builder.Account(account.Id, account.Username, account.SecurityLevel);
        }

        public static LogBuilder Account(this LogBuilder builder, Player player)
        {
            return builder.Account(player.Account);
        }

        public static LogBuilder Account(this LogBuilder builder, GameSession session)
        {
            return session.IsLoggedIn() ? builder.Account(session.Player) : builder;
        }

        public static LogBuilder Account(this LogBuilder builder, ChatSession session)
        {
            return session.IsLoggedIn() ? builder.Account(session.GameSession.Player) : builder;
        }

        public static LogBuilder Account(this LogBuilder builder, RelaySession session)
        {
            return session.IsLoggedIn() ? builder.Account(session.GameSession.Player) : builder;
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
                w.WriteProudString(@group.Id.ToString());

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

                w.Write(item.ItemInfos.Count);
                foreach (var info in item.ItemInfos)
                {
                    w.WriteProudString(info.IsEnabled ? "on" : "off");
                    w.WriteEnum(info.PriceGroup.PriceType);
                    w.Write((ushort)info.Discount);
                    w.WriteProudString(info.PriceGroup.Id.ToString());
                    w.WriteProudString(info.EffectGroup.Effects.Count > 0 ? info.EffectGroup.Id.ToString() : "");
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
        //        w.Write(item.color);
        //        w.WriteProudString(item.is_enabled > 0 ? "on" : "off");
        //        w.Write(item.can_refund);
        //        w.Write(item.reward);
        //        w.WriteProudString(""); // ToDo StartDate
        //        w.WriteProudString(""); // ToDo EndDate
        //    }
        //}
    }
}
