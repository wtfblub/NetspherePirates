using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BlubLib.Serialization;
using Netsphere.Server.Game.Data;
using ProudNet;

namespace Netsphere.Server.Game.Serializers
{
    internal class ShopEffectSerializer : ISerializer<ImmutableDictionary<int, ShopEffectGroup>>
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(ImmutableDictionary<int, ShopEffectGroup>);
        }

        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, ImmutableDictionary<int, ShopEffectGroup> value)
        {
            writer.Write(value.Count);
            foreach (var group in value.Values)
            {
                writer.WriteProudString(group.Id.ToString());

                writer.Write(group.Effects.Count);
                foreach (var effect in group.Effects.OrderBy(x => x.Effect))
                    writer.Write(effect.Effect);
            }
        }

        public ImmutableDictionary<int, ShopEffectGroup> Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            // This is not needed
            throw new NotSupportedException();
        }
    }
}
