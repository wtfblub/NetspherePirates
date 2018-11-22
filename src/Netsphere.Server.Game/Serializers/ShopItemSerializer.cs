using System;
using System.Collections.Immutable;
using System.IO;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Server.Game.Data;
using ProudNet;

namespace Netsphere.Server.Game.Serializers
{
    internal class ShopItemSerializer : ISerializer<ImmutableDictionary<ItemNumber, ShopItem>>
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(ImmutableDictionary<ItemNumber, ShopItem>);
        }

        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, ImmutableDictionary<ItemNumber, ShopItem> value)
        {
            writer.Write(value.Count);
            foreach (var item in value.Values)
            {
                writer.Write(item.ItemNumber);

                switch (item.Gender)
                {
                    case Gender.Female:
                        writer.Write((uint)1);
                        break;

                    case Gender.Male:
                        writer.Write((uint)0);
                        break;

                    case Gender.None:
                        writer.Write((uint)2);
                        break;
                }

                writer.Write((ushort)item.License);
                writer.Write((ushort)item.ColorGroup);
                writer.Write((ushort)item.UniqueColorGroup);
                writer.Write((ushort)item.MinLevel);
                writer.Write((ushort)item.MaxLevel);
                writer.Write((ushort)item.MasterLevel);
                writer.Write(0); // RepairCost
                writer.Write(item.IsOneTimeUse);
                writer.Write(!item.IsDestroyable);

                writer.Write(item.ItemInfos.Count);
                foreach (var info in item.ItemInfos)
                {
                    writer.WriteProudString(info.IsEnabled ? "on" : "off");
                    writer.WriteEnum(info.PriceGroup.PriceType);
                    writer.Write((ushort)info.Discount);
                    writer.WriteProudString(info.PriceGroup.Id.ToString());
                    writer.WriteProudString(info.EffectGroup.Effects.Count > 0 ? info.EffectGroup.Id.ToString() : "");
                }
            }
        }

        public ImmutableDictionary<ItemNumber, ShopItem> Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            // This is not needed
            throw new NotSupportedException();
        }
    }
}
