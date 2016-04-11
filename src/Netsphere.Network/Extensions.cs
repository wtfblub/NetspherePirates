using System;
using System.IO;
using BlubLib.Threading.Tasks;
using SharpLzo;
using SlimMath;

namespace Netsphere.Network
{
    public static class NetsphereExtensions
    {
        private static readonly AsyncLock Sync = new AsyncLock();

        public static byte[] CompressLZO(this byte[] data)
        {
            using (Sync.Lock())
                return miniLzo.Compress(data);
        }

        public static byte[] DecompressLZO(this byte[] data, int realSize)
        {
            using (Sync.Lock())
                return miniLzo.Decompress(data, realSize);
        }

        public static Vector2 ReadRotation(this BinaryReader r)
        {
            return new Vector2(r.ReadByte(), r.ReadByte());
        }

        public static void WriteRotation(this BinaryWriter w, Vector2 v)
        {
            w.Write((byte)v.X);
            w.Write((byte)v.Y);
        }

        public static short Compress(this float value)
        {
            var tmp = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            var value1 = (tmp & 0x80000000) >> 0x1F;
            var value2 = ((tmp & 0x7F800000) >> 0x17) - 0x7F;
            var value3 = tmp & 0x7FFFFF;

            return (short)((value1 << 0xF) | ((value2 + 0x7) << 0x9) | (value3 >> 0xE));
        }

        public static float Decompress(this ushort value)
        {
            var result = ((value & 0x1FF) << 14) | ((((value & 0x7F00) >> 9) - 7 + 127) << 23) | ((value & 0x8000) >> 15 << 31);
            return BitConverter.ToSingle(BitConverter.GetBytes(result), 0);
        }

        public static float Decompress(this short value)
        {
            var result = ((value & 0x1FF) << 14) | ((((value & 0x7F00) >> 9) - 7 + 127) << 23) | ((value & 0x8000) >> 15 << 31);
            return BitConverter.ToSingle(BitConverter.GetBytes(result), 0);
        }

        public static float ReadCompressedFloat(this BinaryReader r)
        {
            return r.ReadInt16().Decompress();
        }

        public static Vector3 ReadCompressedVector3(this BinaryReader r)
        {
            return new Vector3(r.ReadCompressedFloat(), r.ReadCompressedFloat(), r.ReadCompressedFloat());
        }

        public static void WriteCompressed(this BinaryWriter w, float value)
        {
            w.Write(value.Compress());
        }

        public static void WriteCompressed(this BinaryWriter w, Vector3 value)
        {
            w.WriteCompressed(value.X);
            w.WriteCompressed(value.Y);
            w.WriteCompressed(value.Z);
        }
    }
}
