using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
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

            if (reliable)
            {
                var counter = ++_encryptCounter;
                dst.WriteByte((byte)(counter & 0x00FF));
                dst.WriteByte((byte)(counter & 0xFF00));
            }

            using (var encryptor = RC4.CreateEncryptor())
            using (var cs = new CryptoStream(dst, encryptor, CryptoStreamMode.Write))
                src.CopyTo(cs);
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
                    var counter = Interlocked.Increment(ref _decryptCounter);
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
