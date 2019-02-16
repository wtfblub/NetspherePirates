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

        public SymmetricAlgorithm RC4 { get; private set; }

        public Crypt(int keySize)
        {
            if (keySize == 0)
            {
                RC4 = new RC4
                {
                    KeySize = s_defaultKey.Length * 8,
                    Key = s_defaultKey
                };
            }
            else
            {
                RC4 = new RC4 { KeySize = keySize };
                RC4.GenerateKey();
            }
        }

        public void Encrypt(IByteBufferAllocator allocator, EncryptMode mode, Stream src, Stream dst, bool reliable)
        {
            using (var data = new BufferWrapper(allocator.Buffer()))
            using (var encryptor = GetAlgorithm(mode).CreateEncryptor())
            using (var cs = new CryptoStream(new NonClosingStream(dst), encryptor, CryptoStreamMode.Write))
            using (var w = cs.ToBinaryWriter(false))
            {
                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _encryptCounter) - 1);
                    data.Buffer.WriteShortLE(counter);
                }

                using (var dataStream = new WriteOnlyByteBufferStream(data.Buffer, false))
                    src.CopyTo(dataStream);

                using (var dataStream = new ReadOnlyByteBufferStream(data.Buffer, false))
                {
                    dataStream.Position = 0;
                    dataStream.CopyTo(cs);
                }
            }
        }

        public void Decrypt(IByteBufferAllocator allocator, EncryptMode mode, Stream src, Stream dst, bool reliable)
        {
            using (var data = new BufferWrapper(allocator.Buffer()))
            using (var decryptor = GetAlgorithm(mode).CreateDecryptor())
            using (var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read))
            {
                using (var dataStream = new WriteOnlyByteBufferStream(data.Buffer, false))
                    cs.CopyTo(dataStream);

                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _decryptCounter) - 1);
                    var messageCounter = data.Buffer.GetShortLE(data.Buffer.ReaderIndex);

                    if (counter != messageCounter)
                        throw new ProudException($"Invalid decrypt counter! Remote: {messageCounter} Local: {counter}");
                }

                var slice = data.Buffer.ReadSlice(data.Buffer.ReadableBytes);
                using (var dataStream = new ReadOnlyByteBufferStream(slice, false))
                {
                    dataStream.Position = reliable ? 2 : 0;
                    dataStream.CopyTo(dst);
                }
            }
        }

        public void Dispose()
        {
            if (RC4 != null)
            {
                RC4.Dispose();
                RC4 = null;
            }
        }

        private SymmetricAlgorithm GetAlgorithm(EncryptMode mode)
        {
            switch (mode)
            {
                case EncryptMode.Fast:
                    return RC4;

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
