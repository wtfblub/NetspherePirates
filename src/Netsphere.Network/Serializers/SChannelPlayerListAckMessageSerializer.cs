using System;
using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using ProudNet;

namespace Netsphere.Network.Serializers
{
    internal class SChannelPlayerListAckMessageSerializer : ISerializer
    {
        public Type HandlesType => typeof(SChannelPlayerListAckMessage);

        public void Serialize(BinaryWriter writer, object value)
        {
            var message = (SChannelPlayerListAckMessage)value;

            using (var w2 = new BinaryWriter(new MemoryStream()))
            {
                w2.Write((ushort)message.UserData.Length);

                var serializer = Serializer.GetSerializer<UserDataWithNickDto>();
                foreach (var userData in message.UserData)
                    serializer.Serialize(w2, userData);

                var data = w2.ToArray();
                var compressed = data.CompressLZO();
                writer.WriteStruct(compressed);
                writer.Write(compressed.Length);
            }
        }

        public object Deserialize(BinaryReader reader)
        {
            var compressed = reader.ReadStruct();
            reader.ReadInt32(); // length

            var decompressed = compressed.DecompressLZO(compressed.Length * 10);

            using (var r2 = decompressed.ToBinaryReader())
            {
                var serializer = Serializer.GetSerializer<UserDataWithNickDto>();
                var userData = new UserDataWithNickDto[r2.ReadInt16()];
                for (var i = 0; i < userData.Length; i++)
                    userData[i] = (UserDataWithNickDto)serializer.Deserialize(r2);
                return new SChannelPlayerListAckMessage(userData);
            }
        }
    }
}
