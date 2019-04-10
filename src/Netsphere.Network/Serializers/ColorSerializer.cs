using System;
using System.Drawing;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Serializers
{
    public class ColorSerializer : ISerializer<Color>
    {
        public bool CanHandle(Type type)
        {
            return typeof(Color) == type;
        }

        public void Serialize(BlubSerializer blubSerializer, BinaryWriter writer, Color value)
        {
            writer.Write(value.ToArgb());
        }

        public Color Deserialize(BlubSerializer blubSerializer, BinaryReader reader)
        {
            return Color.FromArgb(reader.ReadInt32());
        }
    }
}
