using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlubLib.Serialization;
using Sigil;

namespace Netsphere.Network.Serializers
{
    public class TimeSpanSecondsSerializer : ISerializer<TimeSpan>
    {
        public bool CanHandle(Type type)
        {
            return typeof(TimeSpan) == type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, TimeSpan value)
        {
            writer.Write((uint)value.TotalSeconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            return TimeSpan.FromSeconds(reader.ReadUInt32());
        }
    }
}
