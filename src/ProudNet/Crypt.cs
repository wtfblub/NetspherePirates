using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using BlubLib.DotNetty;
using BlubLib.IO;
using BlubLib.Security.Cryptography;
using DotNetty.Buffers;
using ReadOnlyByteBufferStream = BlubLib.DotNetty.ReadOnlyByteBufferStream;

namespace ProudNet
{
    internal class Crypt : IDisposable
    {
        private static readonly byte[] s_defaultKey = { 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private int _encryptCounter;
        private int _decryptCounter;

        public SymmetricAlgorithm AES { get; private set; }
        
        public Crypt(int keySize)
        {
            if (keySize == 0)
            {
                AES = new RijndaelManaged
                {
                    BlockSize = s_defaultKey.Length * 8,
                    KeySize = s_defaultKey.Length * 8,
                    Padding = PaddingMode.None,
                    Mode = CipherMode.ECB,
                    Key = s_defaultKey
                };
            }
            else
            {
                AES = new RijndaelManaged
                {
                    BlockSize = keySize,
                    KeySize = keySize,
                    Padding = PaddingMode.None,
                    Mode = CipherMode.ECB,
                };
                AES.GenerateKey();
            }
        }

        public Crypt(byte[] secureKey)
        {
            AES = new RijndaelManaged
            {
                BlockSize = secureKey.Length * 8,
                KeySize = secureKey.Length * 8,
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                Key = secureKey
            };
        }

        public void Encrypt(IByteBufferAllocator allocator, EncryptMode mode, Stream src, Stream dst, bool reliable)
        {
            using (var data = new BufferWrapper(allocator.Buffer().WithOrder(ByteOrder.LittleEndian)))
            using (var encryptor = GetAlgorithm(mode).CreateEncryptor())
            using (var cs = new CryptoStream(new NonClosingStream(dst), encryptor, CryptoStreamMode.Write))
            using (var w = cs.ToBinaryWriter(false))
            {
                var blockSize = AES.BlockSize / 8;
                var padding = blockSize - (src.Length + 1 + 4) % blockSize;
                if (reliable)
                    padding = blockSize - (src.Length + 1 + 4 + 2) % blockSize;

                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _encryptCounter) - 1);
                    data.Buffer.WriteShort(counter);
                }

                using (var dataStream = new WriteOnlyByteBufferStream(data.Buffer, false))
                    src.CopyTo(dataStream);

                w.Write((byte)padding);
                using (var dataStream = new ReadOnlyByteBufferStream(data.Buffer, false))
                {
                    w.Write(Hash.GetUInt32<CRC32>(dataStream));
                    dataStream.Position = 0;
                    dataStream.CopyTo(cs);
                }
                w.Fill((int)padding);
            }
        }

        public void Decrypt(IByteBufferAllocator allocator, EncryptMode mode, Stream src, Stream dst, bool reliable)
        {
            using (var data = new BufferWrapper(allocator.Buffer().WithOrder(ByteOrder.LittleEndian)))
            using (var decryptor = GetAlgorithm(mode).CreateDecryptor())
            using (var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read))
            {
                var padding = cs.ReadByte();
                var checksum = cs.ReadByte() | cs.ReadByte() << 8 | cs.ReadByte() << 16 | cs.ReadByte() << 24;

                using (var dataStream = new WriteOnlyByteBufferStream(data.Buffer, false))
                    cs.CopyTo(dataStream);

                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _decryptCounter) - 1);
                    var messageCounter = data.Buffer.GetShort(data.Buffer.ReaderIndex);

                    if (counter != messageCounter)
                        throw new ProudException($"Invalid decrypt counter! Remote: {messageCounter} Local: {counter}");
                }

                var slice = data.Buffer.ReadSlice(data.Buffer.ReadableBytes - padding);
                using (var dataStream = new ReadOnlyByteBufferStream(slice, false))
                {
                    if (Hash.GetUInt32<CRC32>(dataStream) != (uint)checksum)
                        throw new ProudException("Invalid checksum");

                    dataStream.Position = reliable ? 2 : 0;
                    dataStream.CopyTo(dst);
                }
            }
        }

        public void Dispose()
        {
            if (AES != null)
            {
                AES.Dispose();
                AES = null;
            }
        }

        private SymmetricAlgorithm GetAlgorithm(EncryptMode mode)
        {
            switch (mode)
            {
                case EncryptMode.Secure:
                    return AES;

                default:
                    throw new ArgumentException("Invalid mode", nameof(mode));
            }
        }

        private class BufferWrapper : IDisposable
        {
            public IByteBuffer Buffer { get; private set; }

            public BufferWrapper(IByteBuffer buffer)
            {
                Buffer = buffer;
            }

            public void Dispose()
            {
                Buffer?.Release();
                Buffer = null;
            }
        }
    }
}
