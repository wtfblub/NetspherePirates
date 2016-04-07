using System;
using System.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using ProudNet;

namespace Netsphere.Network.Serializers
{
    internal class SGameRoomListAckMessageSerializer : ISerializer
    {
        public Type HandlesType => typeof(SGameRoomListAckMessage);

        public void Serialize(BinaryWriter writer, object value)
        {
            var message = (SGameRoomListAckMessage) value;
            using (var w2 = new BinaryWriter(new MemoryStream()))
            {
                w2.WriteEnum(message.ListType);
                w2.Write((ushort)message.Rooms.Length);

                var serializer = Serializer.GetSerializer<RoomDto>();
                foreach (var room in message.Rooms)
                    serializer.Serialize(w2, room);

                var data = w2.ToArray().CompressLZO();
                writer.WriteStruct(data);
                writer.Write(data.Length);
            }
        }

        public object Deserialize(BinaryReader reader)
        {
            var message = new SGameRoomListAckMessage();
            var compressed = reader.ReadStruct();
            reader.ReadUInt32(); // length

            var decompressed = compressed.DecompressLZO(compressed.Length * 4);

            using (var r2 = decompressed.ToBinaryReader())
            {
                message.ListType = r2.ReadEnum<ChannelInfoRequest>();
                message.Rooms = new RoomDto[r2.ReadInt16()];
                var serializer = Serializer.GetSerializer<RoomDto>();
                for (var i = 0; i < message.Rooms.Length; i++)
                    message.Rooms[i] = (RoomDto)serializer.Deserialize(r2);
            }
            return message;
        }
    }
}
