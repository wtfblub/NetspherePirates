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
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter writer, Color value)
        {
            writer.Write(value.ToArgb());
        }

        public Color Deserialize(BinaryReader reader)
        {
            return Color.FromArgb(reader.ReadInt32());
        }
    }
}
