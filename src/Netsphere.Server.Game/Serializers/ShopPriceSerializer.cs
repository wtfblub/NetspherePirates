using System;
using System.Collections.Immutable;
using System.IO;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Server.Game.Data;
using ProudNet;

namespace Netsphere.Server.Game.Serializers
{
    internal class ShopPriceSerializer : ISerializer<ImmutableDictionary<int, ShopPriceGroup>>
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(ImmutableDictionary<int, ShopPriceGroup>);
        }

        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, ImmutableDictionary<int, ShopPriceGroup> value)
        {
            writer.Write(value.Count);
            foreach (var group in value.Values)
            {
                writer.WriteProudString(group.Id.ToString());
                writer.WriteEnum(group.PriceType);

                writer.Write(group.Prices.Count);
                foreach (var price in group.Prices)
                {
                    writer.WriteEnum(price.PeriodType);
                    writer.Write(price.Period);
                    writer.Write(price.Price);
                    writer.Write(price.CanRefund);
                    writer.Write(price.Durability);
                    writer.Write(price.IsEnabled);
                }
            }
        }

        public ImmutableDictionary<int, ShopPriceGroup> Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            // This is not needed
            throw new NotSupportedException();
        }
    }
}
