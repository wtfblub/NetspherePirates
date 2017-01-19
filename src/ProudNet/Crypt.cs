using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using BlubLib.IO;
using BlubLib.Security.Cryptography;

namespace ProudNet
{
    internal class Crypt : IDisposable
    {
        private int _encryptCounter;
        private int _decryptCounter;

        public RC4 RC4 { get; private set; }

        public Crypt(int keySize)
        {
            RC4 = new RC4 { KeySize = keySize };
            RC4.GenerateKey();
        }

        public void Encrypt(Stream src, Stream dst, bool reliable)
        {
            if (RC4 == null)
                throw new ObjectDisposedException(GetType().FullName);

            using (var encryptor = RC4.CreateEncryptor())
            using (var cs = new CryptoStream(new NonClosingStream(dst), encryptor, CryptoStreamMode.Write))
            {
                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _encryptCounter) - 1);
                    cs.WriteByte((byte)(counter & 0x00FF));
                    cs.WriteByte((byte)(counter >> 8));
                }
                src.CopyTo(cs);
            }
        }

        public void Decrypt(Stream src, Stream dst, bool reliable)
        {
            if (RC4 == null)
                throw new ObjectDisposedException(GetType().FullName);

            using (var decryptor = RC4.CreateDecryptor())
            using (var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read))
            {
                if (reliable)
                {
                    var counter = (ushort)(Interlocked.Increment(ref _decryptCounter) - 1);
                    var messageCounter = cs.ReadByte() | cs.ReadByte() << 8;

                    if (counter != messageCounter)
                        throw new ProudException($"Invalid decrypt counter! Remote: {messageCounter} Local: {counter}");
                }

                cs.CopyTo(dst);
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
    }
}
