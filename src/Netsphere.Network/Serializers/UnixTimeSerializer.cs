using System;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Serializers
{
    public class UnixTimeSerializer : ISerializer<DateTimeOffset>
    {
        public bool CanHandle(Type type)
        {
            return typeof(DateTimeOffset) == type;
        }

        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, DateTimeOffset value)
        {
            writer.Write(value.ToUnixTimeSeconds());
        }

        public DateTimeOffset Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt64());
        }
    }
}
