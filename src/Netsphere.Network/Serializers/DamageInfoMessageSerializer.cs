using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network.Message.P2P;

namespace Netsphere.Network.Serializers
{
    internal class DamageInfoMessageSerializer : ISerializer
    {
        public Type HandlesType => typeof (DamageInfoMessage);

        public void Serialize(BinaryWriter writer, object value)
        {
            var message = (DamageInfoMessage) value;

            writer.Write(message.Target);
            writer.WriteEnum(message.AttackAttribute);
            writer.Write(message.GameTime);
            writer.Write(message.Source);
            writer.Write(message.Unk5);
            writer.WriteRotation(message.Rotation);
            writer.WriteCompressed(message.Position);
            writer.WriteCompressed(message.Unk6);
            writer.WriteCompressed(message.Damage);
            writer.Write(message.Unk8);
            writer.Write(message.Unk9);

            var ls = new List<byte>();
            var bw = new BitStreamWriter(ls);
            bw.Write(message.Flag1, 3);
            bw.Write(message.Flag2, 2);
            bw.Write(message.Flag3, 1);
            bw.Write(message.Flag4, 1);
            bw.Write(message.Flag5, 1);

            bw.Write(message.Flag6, 1);
            bw.Write(message.Flag7, 7);

            bw.Write(message.IsCritical, 4);
            bw.Write(message.Flag9, 4);

            writer.Write(ls);
        }

        public object Deserialize(BinaryReader reader)
        {
            var message = new DamageInfoMessage();
            message.Target = reader.ReadUInt16();
            message.AttackAttribute = reader.ReadEnum<AttackAttribute>();
            message.GameTime = reader.ReadUInt32();
            message.Source = reader.ReadUInt16();
            message.Unk5 = reader.ReadByte();
            message.Rotation = reader.ReadRotation();
            message.Position = reader.ReadCompressedVector3();
            message.Unk6 = reader.ReadCompressedFloat();
            message.Damage = reader.ReadCompressedFloat();
            message.Unk8 = reader.ReadInt16();
            message.Unk9 = reader.ReadUInt16();

            var br = new BitStreamReader(reader.ReadBytes(3));
            message.Flag1 = br.ReadByte(3);
            message.Flag2 = br.ReadByte(2);
            message.Flag3 = br.ReadByte(1);
            message.Flag4 = br.ReadByte(1);
            message.Flag5 = br.ReadByte(1);

            message.Flag6 = br.ReadByte(1);
            message.Flag7 = br.ReadByte(7);

            message.IsCritical = br.ReadByte(4);
            message.Flag9 = br.ReadByte(4);

            return message;
        }
    }
}
