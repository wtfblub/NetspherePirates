using System;
using System.IO;
using System.Text;
using DotNetty.Buffers;
using Ionic.Zlib;

namespace ProudNet
{
    public static class ProudNetBinaryReaderExtensions
    {
        public static int ReadScalar(this BinaryReader @this)
        {
            var prefix = @this.ReadByte();
            switch (prefix)
            {
                case 1:
                    return @this.ReadByte();

                case 2:
                    return @this.ReadInt16();

                case 4:
                    return @this.ReadInt32();

                default:
                    throw new Exception($"Invalid prefix {prefix}");
            }
        }

        public static byte[] ReadStruct(this BinaryReader @this)
        {
            var size = @this.ReadScalar();
            return @this.ReadBytes(size);
        }

        public static string ReadProudString(this BinaryReader @this)
        {
            var stringType = @this.ReadByte();
            var size = @this.ReadScalar();
            if (size <= 0) return "";

            switch (stringType)
            {
                case 1:
                    return Constants.Encoding.GetString(@this.ReadBytes(size));

                case 2:
                    return Encoding.UTF8.GetString(@this.ReadBytes(size * 2));

                default:
                    throw new Exception("Unknown StringType: " + stringType);
            }
        }
    }

    public static class ProudNetBinaryWriterExtensions
    {
        public static void WriteScalar(this BinaryWriter @this, int value)
        {
            byte prefix = 4;
            if (value < 128)
                prefix = 1;
            else if (value < 32768)
                prefix = 2;

            switch (prefix)
            {
                case 1:
                    @this.Write(prefix);
                    @this.Write((byte)value);
                    break;

                case 2:
                    @this.Write(prefix);
                    @this.Write((short)value);
                    break;

                case 4:
                    @this.Write(prefix);
                    @this.Write(value);
                    break;

                default:
                    throw new Exception("Invalid prefix");
            }
        }

        public static void WriteStruct(this BinaryWriter @this, byte[] data)
        {
            @this.WriteScalar(data.Length);
            @this.Write(data);
        }

        public static void WriteProudString(this BinaryWriter @this, string value, bool unicode = false)
        {
            @this.Write((byte)(unicode ? 2 : 1));

            var size = value.Length;
            @this.WriteScalar(size);
            if (size <= 0)
                return;

            var encoding = unicode ? Encoding.UTF8 : Constants.Encoding;
            var bytes = encoding.GetBytes(value);
            @this.Write(bytes);
        }
    }

    public static class ProudNetByteArrayExtensions
    {
        public static byte[] CompressZLib(this byte[] @this)
        {
            using (var ms = new MemoryStream())
            using (var zlib = new ZlibStream(ms, CompressionMode.Compress))
            {
                zlib.Write(@this, 0, @this.Length);
                zlib.Close();
                return ms.ToArray();
            }
        }

        public static byte[] DecompressZLib(this byte[] @this)
        {
            using (var zlib = new ZlibStream(new MemoryStream(@this), CompressionMode.Decompress))
                return zlib.ReadToEnd();
        }
    }

    public static class ProudNetIByteBufferExtensions
    {
        public static int ReadScalar(this IByteBuffer @this)
        {
            var prefix = @this.ReadByte();
            switch (prefix)
            {
                case 1:
                    return @this.ReadByte();

                case 2:
                    return @this.ReadShort();

                case 4:
                    return @this.ReadInt();

                default:
                    throw new Exception($"Invalid prefix {prefix}");
            }
        }

        public static IByteBuffer ReadStruct(this IByteBuffer @this)
        {
            var size = @this.ReadScalar();
            return @this.ReadBytes(size);
        }

        public static string ReadProudString(this IByteBuffer @this)
        {
            var stringType = @this.ReadByte();
            var size = @this.ReadScalar();
            if (size <= 0) return "";

            string str;
            switch (stringType)
            {
                case 1:
                    str = @this.ToString(@this.ReaderIndex, size, Constants.Encoding);
                    @this.SkipBytes(size);
                    break;

                case 2:
                    str = @this.ToString(@this.ReaderIndex, size * 2, Encoding.UTF8);
                    @this.SkipBytes(size * 2);
                    break;

                default:
                    throw new Exception("Unknown StringType: " + stringType);
            }
            return str;
        }

        public static IByteBuffer WriteScalar(this IByteBuffer @this, int value)
        {
            byte prefix = 4;
            if (value <= sbyte.MaxValue)
                prefix = 1;
            else if (value <= short.MaxValue)
                prefix = 2;

            switch (prefix)
            {
                case 1:
                    @this.WriteByte(prefix);
                    @this.WriteByte((byte)value);
                    break;

                case 2:
                    @this.WriteByte(prefix);
                    @this.WriteShort((short)value);
                    break;

                case 4:
                    @this.WriteByte(prefix);
                    @this.WriteInt(value);
                    break;

                default:
                    throw new Exception("Invalid prefix");
            }

            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, IByteBuffer data)
        {
            @this.WriteScalar(data.ReadableBytes)
                .WriteBytes(data);
            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, IByteBuffer data, int length)
        {
            @this.WriteScalar(length)
                .WriteBytes(data, length);
            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, IByteBuffer data, int offset, int length)
        {
            @this.WriteScalar(length)
                .WriteBytes(data, offset, length);
            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, byte[] data)
        {
            @this.WriteScalar(data.Length)
                .WriteBytes(data);
            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, byte[] data, int length)
        {
            @this.WriteScalar(length)
                .WriteBytes(data, 0, length);
            return @this;
        }

        public static IByteBuffer WriteStruct(this IByteBuffer @this, byte[] data, int offset, int length)
        {
            @this.WriteScalar(length)
                .WriteBytes(data, offset, length);
            return @this;
        }

        public static IByteBuffer WriteProudString(this IByteBuffer @this, string value, bool unicode = false)
        {
            @this.WriteByte((byte)(unicode ? 2 : 1));

            var size = value.Length;
            @this.WriteScalar(size);
            if (size <= 0)
                return @this;

            var encoding = unicode ? Encoding.UTF8 : Constants.Encoding;
            var bytes = encoding.GetBytes(value);
            @this.WriteBytes(bytes);
            return @this;
        }
    }
}
