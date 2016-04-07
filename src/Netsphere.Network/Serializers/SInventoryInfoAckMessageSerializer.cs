using System;
using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using ProudNet;

namespace Netsphere.Network.Serializers
{
    internal class SInventoryInfoAckMessageSerializer : ISerializer
    {
        public Type HandlesType => typeof(SInventoryInfoAckMessage);

        public void Serialize(BinaryWriter writer, object value)
        {
            var message = (SInventoryInfoAckMessage) value;
            using (var w2 = new BinaryWriter(new MemoryStream()))
            {
                w2.Write((ushort)message.Items.Length);

                var serializer = Serializer.GetSerializer<ItemDto>();
                foreach (var item in message.Items)
                    serializer.Serialize(w2, item);

                var data = w2.ToArray().CompressLZO();
                writer.WriteStruct(data);
                writer.Write(data.Length);
            }
        }

        public object Deserialize(BinaryReader reader)
        {
            var message = new SInventoryInfoAckMessage();
            var compressed = reader.ReadStruct();
            reader.ReadUInt32(); // length

            var decompressed = compressed.DecompressLZO(compressed.Length * 4);

            using (var r2 = decompressed.ToBinaryReader())
            {
                message.Items = new ItemDto[r2.ReadInt16()];
                var serializer = Serializer.GetSerializer<ItemDto>();
                for (var i = 0; i < message.Items.Length; i++)
                    message.Items[i] = (ItemDto)serializer.Deserialize(r2);
            }
            return message;
        }
    }
}
