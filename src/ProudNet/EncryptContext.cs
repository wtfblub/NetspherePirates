using System;
using System.Security.Cryptography;
using BlubLib.Security.Cryptography;

namespace ProudNet
{
    internal class EncryptContext : IDisposable
    {
        public RC4 RC4 { get; private set; }
        public ushort EncryptCounter { get; private set; }
        public ushort DecryptCounter { get; private set; }

        public EncryptContext(int keySize)
        {
            RC4 = new RC4 { KeySize = keySize };
            RC4.GenerateKey();
        }

        public byte[] Encrypt(byte[] data)
        {
            EncryptCounter++;
            return RC4.Encrypt(data);
        }

        public byte[] Decrypt(byte[] data)
        {
            DecryptCounter++;
            return RC4.Decrypt(data);
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
