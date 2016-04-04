using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace ProudNet
{
    public static class ProudNetBinaryReaderExtensions
    {
        internal static Encoding ProudEncoding { get; } = Encoding.GetEncoding("Windows-1252");

        public static int ReadScalar(this BinaryReader r)
        {
            var prefix = r.ReadByte();
            switch (prefix)
            {
                case 1:
                    return r.ReadByte();

                case 2:
                    return r.ReadInt16();

                case 4:
                    return r.ReadInt32();

                default:
                    throw new Exception($"Invalid prefix {prefix}");
            }
        }

        public static byte[] ReadStruct(this BinaryReader r)
        {
            var size = r.ReadScalar();
            return r.ReadBytes(size);
        }

        public static string ReadProudString(this BinaryReader r)
        {
            var stringType = r.ReadByte();
            var size = r.ReadScalar();
            if (size <= 0) return "";

            switch (stringType)
            {
                case 1:
                    return ProudEncoding.GetString(r.ReadBytes(size));

                case 2:
                    return Encoding.UTF8.GetString(r.ReadBytes(size * 2));

                default:
                    throw new Exception("Unknown StringType: " + stringType);
            }
        }
    }

    public static class ProudNetBinaryWriterExtensions
    {
        public static void WriteScalar(this BinaryWriter w, int value)
        {
            byte prefix = 4;
            if (value < 128)
                prefix = 1;
            else if (value < 32768)
                prefix = 2;

            switch (prefix)
            {
                case 1:
                    w.Write(prefix);
                    w.Write((byte)value);
                    break;

                case 2:
                    w.Write(prefix);
                    w.Write((short)value);
                    break;

                case 4:
                    w.Write(prefix);
                    w.Write(value);
                    break;

                default:
                    throw new Exception("Invalid prefix");
            }
        }

        public static void WriteStruct(this BinaryWriter w, byte[] data)
        {
            w.WriteScalar(data.Length);
            w.Write(data);
        }

        public static void WriteProudString(this BinaryWriter w, string value, bool unicode = false)
        {
            w.Write((byte)(unicode ? 2 : 1));

            var size = value.Length;
            w.WriteScalar(size);
            if (size <= 0)
                return;

            var encoding = unicode ? Encoding.UTF8 : ProudNetBinaryReaderExtensions.ProudEncoding;
            var bytes = encoding.GetBytes(value);
            w.Write(bytes);
        }
    }

    public static class ProudNetByteArrayExtensions
    {
        public static byte[] CompressZLib(this byte[] data)
        {
            using (var ms = new MemoryStream())
            using (var zlib = new ZlibStream(ms, CompressionMode.Compress))
            {
                zlib.Write(data, 0, data.Length);
                zlib.Close();
                return ms.ToArray();
            }
        }

        public static byte[] DecompressZLib(this byte[] data)
        {
            using (var zlib = new ZlibStream(new MemoryStream(data), CompressionMode.Decompress))
                return zlib.ReadToEnd();
        }
    }
}
