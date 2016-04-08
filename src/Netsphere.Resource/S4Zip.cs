using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BlubLib.Security.Cryptography;
using SharpLzo;

namespace Netsphere.Resource
{
    public class S4Zip : IReadOnlyDictionary<string, S4ZipEntry>
    {
        private readonly Dictionary<string, S4ZipEntry> _entries;

        public string ZipPath { get; }
        public string ResourcePath { get; }

        private S4Zip(string zipPath)
        {
            _entries = new Dictionary<string, S4ZipEntry>();
            ZipPath = zipPath;
            // ReSharper disable once AssignNullToNotNullAttribute
            ResourcePath = Path.Combine(Path.GetDirectoryName(zipPath), "_resources");
        }

        public static S4Zip OpenZip(string fileName)
        {
            var zip = new S4Zip(fileName);
            zip.Open(fileName);
            return zip;
        }

        public void Open(string fileName)
        {
            Open(File.ReadAllBytes(fileName));
        }

        public void Open(byte[] data)
        {
            var tmp = Decrypt(data);
            using (var r = tmp.ToBinaryReader())
            {
                r.ReadInt32();

                var entryCount = r.ReadInt32();
                for (var i = 0; i < entryCount; i++)
                {
                    var entrySize = r.ReadInt32();
                    var entryData = r.ReadBytes(entrySize);
                    entryData = DecryptEntry(entryData);

                    using (var entryReader = entryData.ToBinaryReader())
                    {
                        var fullName = entryReader.ReadCString(256).ToLower();
                        var checksum = entryReader.ReadInt64();
                        var length = entryReader.ReadInt32();
                        var unk = entryReader.ReadInt32();

                        var entry = new S4ZipEntry(this, fullName, length, checksum, unk);
                        _entries.Add(fullName, entry);
                    }
                }
            }
        }

        public void Save()
        {
            Save(ZipPath);
        }

        public void Save(string fileName)
        {
            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Write(1);
                w.Write(_entries.Count);
                foreach (var entry in _entries.Values)
                {
                    using (var entryWriter = new BinaryWriter(new MemoryStream()))
                    {
                        entryWriter.WriteCString(entry.FullName, 256);
                        entryWriter.Write(entry.Checksum);
                        entryWriter.Write(entry.Length);
                        entryWriter.Write(entry.Unk);

                        var entryData = entryWriter.ToArray();
                        entryData = EncryptEntry(entryData);

                        w.Write(entryData.Length);
                        w.Write(entryData);
                    }
                }

                var outBuffer = w.ToArray();
                outBuffer = Encrypt(outBuffer);
                File.WriteAllBytes(fileName, outBuffer);
            }
        }

        public S4ZipEntry CreateEntry(string fullName, byte[] data)
        {
            if (_entries.ContainsKey(fullName))
                throw new ArgumentException(fullName + " already exists", fullName);

            var entry = new S4ZipEntry(this, fullName);
            entry.SetData(data);
            _entries.Add(fullName, entry);
            return entry;
        }

        public S4ZipEntry RemoveEntry(string fullName)
        {
            return RemoveEntry(fullName, false);
        }

        public S4ZipEntry RemoveEntry(string fullName, bool deleteFromDisk)
        {
            S4ZipEntry entry;
            if (!_entries.TryGetValue(fullName, out entry))
                throw new ArgumentException(fullName + " does not exist", fullName);

            if (deleteFromDisk && File.Exists(entry.FileName))
                File.Delete(entry.FileName);
            _entries.Remove(fullName);
            return entry;
        }

        private byte[] Encrypt(byte[] data)
        {
            var cipher = S4Crypto.EncryptAES(data);
            S4Crypto.Encrypt(cipher);
            return cipher;
        }

        private byte[] Decrypt(byte[] data)
        {
            S4Crypto.Decrypt(data);
            return S4Crypto.DecryptAES(data);
        }

        private byte[] EncryptEntry(byte[] data)
        {
            S4Crypto.EncryptCapped(data);
            return data;
        }

        private byte[] DecryptEntry(byte[] data)
        {
            S4Crypto.DecryptCapped(data);
            return data;
        }

        public static byte[] EncryptS4(byte[] data)
        {
            var realSize = data.Length;
            var buffer = miniLzo.Compress(data);
            S4Crypto.Encrypt(buffer, 0, 0);

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                w.Write(realSize);
                w.Write(buffer);
                return w.ToArray();
            }
        }

        public static byte[] DecryptS4(byte[] data)
        {
            int realSize;
            byte[] buffer;
            using (var r = data.ToBinaryReader())
            {
                realSize = r.ReadInt32();
                buffer = r.ReadToEnd();
            }

            S4Crypto.Decrypt(buffer, 0, 0);
            return miniLzo.Decompress(buffer, realSize);
        }

        #region IReadOnlyDictionary

        public int Count => _entries.Count;

        public IEnumerable<string> Keys => _entries.Keys;

        public IEnumerable<S4ZipEntry> Values => _entries.Values;

        public S4ZipEntry this[string key]
        {
            get
            {
                S4ZipEntry entry;
                TryGetValue(key.ToLower(), out entry);
                return entry;
            }
        }

        public bool ContainsKey(string key)
        {
            return _entries.ContainsKey(key.ToLower());
        }

        public bool TryGetValue(string key, out S4ZipEntry value)
        {
            return _entries.TryGetValue(key.ToLower(), out value);
        }

        public IEnumerator<KeyValuePair<string, S4ZipEntry>> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

    public class S4ZipEntry
    {
        public string Name { get; }
        public string FullName { get; }
        public int Length { get; private set; }
        public long Checksum { get; private set; }
        public int Unk { get; }

        public string FileName => Path.Combine(Archive.ResourcePath, Checksum.ToString("x"));
        public S4Zip Archive { get; protected set; }

        internal S4ZipEntry(S4Zip archive, string fullName)
        {
            Archive = archive;
            FullName = fullName;
            Name = Path.GetFileName(fullName);
        }
        internal S4ZipEntry(S4Zip archive, string fullName, int length, long checksum, int unk)
        {
            Archive = archive;
            FullName = fullName;
            Name = Path.GetFileName(fullName);
            Length = length;
            Checksum = checksum;
            Unk = unk;
        }

        public byte[] GetData()
        {
            return Decrypt(File.ReadAllBytes(FileName));
        }
        public void SetData(byte[] data)
        {
            Length = data.Length;
            Checksum = GetChecksum(data);
            var encrypted = Encrypt(data);
            File.WriteAllBytes(FileName, encrypted);
        }

        public void Remove()
        {
            Remove(true);
        }
        public void Remove(bool deleteFromDisk)
        {
            Archive.RemoveEntry(FullName, deleteFromDisk);
        }

        public override string ToString()
        {
            return FullName;
        }

        private byte[] Encrypt(byte[] data)
        {
            var isX7 = Name.EndsWith(".x7", StringComparison.InvariantCultureIgnoreCase);
            if (Name.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase) || isX7)
            {
                if (isX7) data = S4Crypto.EncryptX7(data);
                data = S4Crypto.EncryptAES(data);
                S4Crypto.Encrypt(data);
            }

            Length = data.Length;

            S4Crypto.EncryptCapped(data);
            if (data.Length < 1048576)
                data = miniLzo.Compress(data);
            S4Crypto.SwapBytes(data);

            return data;
        }
        private byte[] Decrypt(byte[] data)
        {
            S4Crypto.SwapBytes(data);
            if (data.Length < 1048576)
                data = miniLzo.Decompress(data, Length);
            S4Crypto.DecryptCapped(data);

            var isX7 = Name.EndsWith(".x7", StringComparison.InvariantCultureIgnoreCase);
            if (Name.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase) || isX7)
            {
                S4Crypto.Decrypt(data);
                data = S4Crypto.DecryptAES(data);
                if (isX7) data = S4Crypto.DecryptX7(data);
            }

            return data;
        }

        private long GetChecksum(byte[] data)
        {
            return S4CRC.ComputeHash(data, FullName);
        }
    }

    internal static class S4Crypto
    {
        // ReSharper disable RedundantExplicitArraySize
        private static readonly byte[][][] KeyTable = new byte[2][][]
        {
            #region Table1
            new byte[10][]
            {
                new byte[40]
                {
                    0x82, 0x53, 0x43, 0x4C, 0x2B,
                    0x0D, 0x37, 0xD7, 0xD9, 0xD8,
                    0x1B, 0x6D, 0xA0, 0xC3, 0x2B,
                    0xEE, 0x45, 0x88, 0x1A, 0xA6,
                    0x18, 0x1D, 0x9D, 0x38, 0x2A,
                    0x55, 0x03, 0x1D, 0xCD, 0xA6,
                    0x73, 0x07, 0xED, 0x8D, 0xC5,
                    0xDB, 0xA3, 0xBD, 0xB6, 0xD5
                },
                new byte[40]
                {
                    0x34, 0xB5, 0xB2, 0x3D, 0x7D,
                    0x43, 0x8C, 0xC0, 0x21, 0x25,
                    0xCD, 0xB6, 0x53, 0x76, 0xCE,
                    0x5D, 0xD4, 0x87, 0xCA, 0x84,
                    0x81, 0xCB, 0x5E, 0x04, 0xBA,
                    0x69, 0x3E, 0x65, 0xDE, 0x21,
                    0x8A, 0x63, 0x62, 0x71, 0x90,
                    0x87, 0x0A, 0x52, 0x28, 0x44
                },
                new byte[40]
                {
                    0xA3, 0x49, 0xDC, 0xEA, 0x09,
                    0xB7, 0x01, 0xA4, 0xA1, 0x11,
                    0x11, 0x8E, 0x80, 0x35, 0x5B,
                    0xDD, 0x38, 0xD5, 0x4E, 0x36,
                    0x0C, 0xA2, 0xBB, 0x05, 0x36,
                    0x57, 0x2E, 0x98, 0xBE, 0x88,
                    0x3C, 0x28, 0x43, 0x63, 0xA0,
                    0xE9, 0xE1, 0x6D, 0x51, 0xCB
                },
                new byte[40]
                {
                    0x4D, 0x62, 0x84, 0x43, 0x89,
                    0xC7, 0x89, 0x83, 0x65, 0x29,
                    0x53, 0x95, 0x7C, 0xC0, 0xA1,
                    0x0C, 0xDB, 0xD7, 0x04, 0xD8,
                    0x6A, 0xD1, 0x73, 0x1D, 0x21,
                    0x67, 0x86, 0x8D, 0xA4, 0xA0,
                    0x34, 0xBD, 0x31, 0x20, 0x61,
                    0x0E, 0xE9, 0x63, 0xB4, 0xC0
                },
                new byte[40]
                {
                    0xC7, 0x36, 0x1B, 0x41, 0x23,
                    0x9C, 0xD1, 0x8C, 0x25, 0x53,
                    0x42, 0x2E, 0x45, 0x6D, 0x42,
                    0x7B, 0x4E, 0x5B, 0xEB, 0x24,
                    0x33, 0x74, 0x52, 0x28, 0xC6,
                    0x2A, 0xC3, 0x16, 0x60, 0xA5,
                    0x45, 0x35, 0xDB, 0x9A, 0x54,
                    0x97, 0xE2, 0xEE, 0x9B, 0xDE
                },
                new byte[40]
                {
                    0xE0, 0xC3, 0x84, 0x41, 0xED,
                    0x45, 0x4C, 0x69, 0xD9, 0x28,
                    0x55, 0x27, 0x8E, 0x3A, 0x3C,
                    0x8E, 0x84, 0x97, 0x14, 0xE6,
                    0x58, 0x51, 0x26, 0x0D, 0xE2,
                    0x9E, 0x66, 0x7C, 0x0D, 0x01,
                    0x7D, 0x17, 0x4C, 0x08, 0xDD,
                    0x97, 0x1C, 0x7B, 0xCE, 0x5D
                },
                new byte[40]
                {
                    0x54, 0x37, 0x7C, 0x0C, 0x8E,
                    0x27, 0x7A, 0x78, 0x2E, 0xE6,
                    0x6D, 0x25, 0x62, 0x62, 0x98,
                    0x20, 0x2E, 0x23, 0x15, 0x61,
                    0x7D, 0x97, 0x50, 0x07, 0x20,
                    0x7A, 0x04, 0x29, 0x62, 0x90,
                    0x6B, 0xE9, 0xE6, 0x22, 0x72,
                    0x38, 0x56, 0xC9, 0x06, 0x2E
                },
                new byte[40]
                {
                    0x3B, 0x47, 0x08, 0x2D, 0x21,
                    0x42, 0x07, 0x69, 0x4A, 0x57,
                    0x8B, 0x79, 0xE7, 0x56, 0x27,
                    0x23, 0x24, 0x85, 0x47, 0x74,
                    0x75, 0x85, 0xA9, 0xEB, 0x10,
                    0xCB, 0x17, 0x85, 0x4B, 0x5E,
                    0x20, 0x78, 0xD0, 0x7D, 0x86,
                    0x5E, 0x14, 0x7E, 0x64, 0x50
                },
                new byte[40]
                {
                    0x69, 0x52, 0x4A, 0xBD, 0x8C,
                    0x9B, 0xD6, 0x63, 0xBD, 0x26,
                    0x86, 0x32, 0x95, 0xA4, 0x02,
                    0x9B, 0x01, 0x14, 0x49, 0x78,
                    0x88, 0x57, 0x3A, 0x01, 0x4A,
                    0xBC, 0x50, 0xCD, 0x31, 0x39,
                    0x71, 0x30, 0x5B, 0x9C, 0x4D,
                    0x21, 0x67, 0x82, 0xE8, 0x5C
                },
                new byte[40]
                {
                    0x66, 0x10, 0xA9, 0x7D, 0xD2,
                    0x36, 0xE2, 0xB1, 0x28, 0x20,
                    0xD5, 0xE7, 0xD5, 0x0E, 0xD4,
                    0x0C, 0x2C, 0x77, 0x80, 0x0E,
                    0xA6, 0x37, 0xBE, 0x61, 0xAD,
                    0xD6, 0x17, 0x65, 0x13, 0x70,
                    0xAE, 0x40, 0x3B, 0x52, 0xEE,
                    0x53, 0x84, 0xEB, 0x04, 0x0D
                }
            },
            #endregion
                
            #region Table2
            new byte[10][]
            {
                new byte[40]
                {
                    0x49, 0x8C, 0x77, 0xC0, 0xC0,
                    0x64, 0x54, 0x0B, 0x22, 0xBD,
                    0x82, 0x93, 0x9A, 0x23, 0x8D,
                    0xE4, 0xC8, 0x9D, 0xB3, 0x50,
                    0x44, 0xB1, 0xE2, 0x9E, 0x15,
                    0x7A, 0xA1, 0x0C, 0x24, 0xE3,
                    0x1E, 0x0A, 0x0A, 0x73, 0x6A,
                    0xA5, 0x8B, 0x3A, 0x53, 0x33
                },
                new byte[40]
                {
                    0xB0, 0xE6, 0xB7, 0x51, 0x70,
                    0xDA, 0xD6, 0x29, 0xAA, 0x10,
                    0xB5, 0x8A, 0x38, 0x37, 0x4E,
                    0x7A, 0x3B, 0x74, 0x7B, 0x63,
                    0x41, 0x7C, 0x21, 0x65, 0x5E,
                    0x26, 0x95, 0x44, 0x75, 0xA3,
                    0x74, 0xDD, 0xB4, 0x33, 0x9E,
                    0x54, 0x3C, 0x95, 0x5E, 0x34
                },
                new byte[40]
                {
                    0x10, 0x19, 0x43, 0x64, 0x78,
                    0x2B, 0xA6, 0x60, 0x7D, 0xCD,
                    0xA9, 0x28, 0xB8, 0x85, 0x0E,
                    0x66, 0xC7, 0x3C, 0x28, 0xDC,
                    0xA1, 0x4D, 0x60, 0x9B, 0xC7,
                    0xD3, 0x74, 0x93, 0xE6, 0xC3,
                    0x97, 0x76, 0x12, 0xA4, 0xCB,
                    0xB9, 0x22, 0x51, 0xB9, 0x79
                },
                new byte[40]
                {
                    0x5C, 0x68, 0xDB, 0xE6, 0x59,
                    0x57, 0x95, 0xCD, 0xAE, 0xCA,
                    0x67, 0xB8, 0x37, 0x90, 0xBA,
                    0x54, 0x98, 0x95, 0x73, 0x8E,
                    0x47, 0xC1, 0x40, 0xBA, 0x80,
                    0x26, 0x10, 0xAA, 0x60, 0x64,
                    0xD8, 0x69, 0xC7, 0x0D, 0x2B,
                    0x28, 0xA6, 0xBA, 0x01, 0x4A
                },
                new byte[40]
                {
                    0xEE, 0x28, 0x65, 0xC4, 0x9D,
                    0x41, 0x8D, 0x91, 0x6C, 0x91,
                    0x7E, 0x80, 0xC3, 0xD1, 0xAE,
                    0xB6, 0x92, 0x41, 0x66, 0x13,
                    0x72, 0x20, 0x26, 0xA1, 0x72,
                    0x05, 0x29, 0x08, 0x88, 0x30,
                    0x40, 0x6D, 0x5A, 0x41, 0x01,
                    0x7A, 0xDB, 0x2C, 0xEE, 0xC3
                },
                new byte[40]
                {
                    0x5C, 0x03, 0x38, 0xD8, 0x95,
                    0xE7, 0xB4, 0x67, 0x30, 0x51,
                    0x21, 0x68, 0x78, 0x89, 0x68,
                    0x0B, 0xE3, 0xB0, 0x28, 0xB3,
                    0xA9, 0x38, 0x18, 0xE4, 0x59,
                    0x43, 0xC9, 0x52, 0x75, 0x04,
                    0x15, 0x07, 0x97, 0x14, 0x07,
                    0x27, 0xDA, 0xE5, 0xD9, 0xDB
                },
                new byte[40]
                {
                    0xDB, 0x08, 0x27, 0xA3, 0x64,
                    0xDC, 0x42, 0xE3, 0x3D, 0x0D,
                    0x26, 0xA2, 0xC3, 0x5E, 0x3E,
                    0xA7, 0x47, 0xE4, 0x1C, 0x73,
                    0x13, 0x99, 0x9E, 0xBA, 0xD3,
                    0x08, 0x73, 0x88, 0x03, 0x01,
                    0x24, 0x2E, 0x09, 0xBD, 0x3A,
                    0x6E, 0x3C, 0xB6, 0xA2, 0x22
                },
                new byte[40]
                {
                    0xE7, 0x27, 0x60, 0x20, 0x85,
                    0xDA, 0xEA, 0x84, 0x86, 0x41,
                    0x67, 0x1C, 0x83, 0xBE, 0x7A,
                    0x61, 0x67, 0x01, 0x18, 0x30,
                    0xC6, 0x37, 0xBC, 0x51, 0xBC,
                    0x78, 0xA1, 0x53, 0x53, 0x58,
                    0x9B, 0x32, 0x05, 0x67, 0x6B,
                    0xC7, 0x3A, 0x7C, 0xA8, 0xE5
                },
                new byte[40]
                {
                    0x70, 0x10, 0x29, 0x88, 0x94,
                    0xC0, 0xEE, 0x8D, 0x52, 0x20,
                    0xD9, 0xC3, 0x3C, 0xB3, 0x43,
                    0x74, 0x83, 0xC8, 0xC5, 0xAA,
                    0x90, 0x58, 0x0C, 0xD0, 0xBC,
                    0x2A, 0xED, 0x04, 0x05, 0x8E,
                    0x27, 0xDE, 0x9C, 0x37, 0x57,
                    0x2A, 0x93, 0x63, 0x1B, 0x9E
                },
                new byte[40]
                {
                    0xC3, 0x52, 0xDB, 0xE9, 0x63,
                    0x9A, 0x87, 0x18, 0x6D, 0xBE,
                    0x1B, 0x37, 0x6A, 0xEA, 0x01,
                    0x02, 0x01, 0xB5, 0x74, 0x71,
                    0xA5, 0x9A, 0x9A, 0x3A, 0x11,
                    0x8B, 0x62, 0xD7, 0xB0, 0x06,
                    0x0C, 0xA0, 0x10, 0x09, 0x97,
                    0x5A, 0xEB, 0xEA, 0x18, 0xB8
                }
            }
            #endregion
        };
        // ReSharper restore RedundantExplicitArraySize

        private static readonly uint[] KeyTable2 = { 0xFE292513, 0xCD4802EF };

        private static readonly byte[] AesIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static readonly byte[] AesKey =
        {
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
            0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF
        };

        public static void EncryptCapped(byte[] data)
        {
            var length = data.Length > 256 ? 256 : data.Length;
            for (var i = 0; i < length; i++)
            {
                data[i] ^= KeyTable[0][0][i % 32];
                data[i] = (byte)(((data[i] & 0x80) >> 7) & 1 | (data[i] << 1) & 0xFE);
            }
        }

        public static void Encrypt(byte[] data)
        {
            uint blockIndex;
            uint keyIndex;
            GetKeyTableIndex(data.Length, out blockIndex, out keyIndex);
            Encrypt(data, blockIndex, keyIndex);
        }

        public static void Encrypt(byte[] data, uint blockIndex, uint keyIndex)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] ^= KeyTable[blockIndex][keyIndex][i % 40];
                data[i] = (byte)(((data[i] & 0x80) >> 7) & 1 | (data[i] << 1) & 0xFE);
            }
        }

        public static void DecryptCapped(byte[] data)
        {
            var length = data.Length > 256 ? 256 : data.Length;
            for (var i = 0; i < length; i++)
            {
                data[i] = (byte)((data[i] >> 1) & 0x7F | ((data[i] & 1) << 7) & 0x80);
                data[i] ^= KeyTable[0][0][i % 32];
            }
        }

        public static void Decrypt(byte[] data)
        {
            uint blockIndex;
            uint keyIndex;
            GetKeyTableIndex(data.Length, out blockIndex, out keyIndex);
            Decrypt(data, blockIndex, keyIndex);
        }

        public static void Decrypt(byte[] data, uint blockIndex, uint keyIndex)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)((data[i] >> 1) & 0x7F | ((data[i] & 1) << 7) & 0x80);
                data[i] ^= KeyTable[blockIndex][keyIndex][i % 40];
            }
        }

        public static byte[] EncryptAES(byte[] data)
        {
            using (var aes = new RijndaelManaged())
            {
                aes.Mode = CipherMode.CFB;
                aes.Padding = PaddingMode.Zeros;
                aes.KeySize = 192;
                aes.BlockSize = 128;

                Array.Resize(ref data, data.Length + 16 - data.Length % 16);
                aes.GenerateKeys(data.Length);
                return aes.Encrypt(data);
            }
        }
        public static byte[] DecryptAES(byte[] data)
        {
            using (var aes = new RijndaelManaged())
            {
                aes.Mode = CipherMode.CFB;
                aes.Padding = PaddingMode.Zeros;
                aes.KeySize = 192;
                aes.BlockSize = 128;
                aes.GenerateKeys(data.Length);

                Array.Resize(ref data, data.Length + 16 - data.Length % 16);

                return aes.Decrypt(data);
            }
        }

        public static void SwapBytes(byte[] data)
        {
            var size = data.Length;
            var i = 0;
            var sizeCapped = size >= 128 ? 128 : size;

            while (i < sizeCapped / 2)
            {
                var j = size - 1 - i;
                var swap = data[j];
                data[j] = data[i];
                data[i++] = swap;
            }
        }

        public static byte[] EncryptX7(byte[] data)
        {
            var crc = X7CRC.ComputeHash(data);
            var realSize = data.Length;

            data = miniLzo.Compress(data);
            data = BuildX7(data, crc, realSize);

            return data;
        }
        public static byte[] DecryptX7(byte[] data)
        {
            var realSize = (int)(BitConverter.ToInt32(data, 0) ^ KeyTable2[0]);
            data = RemoveX7Junk(data);

            LzoResult res;
            data = miniLzo.Decompress(data, realSize, out res);
            return data;
        }

        public static byte[] BuildX7(byte[] data, uint crc, int realSize)
        {
            var buffer1 = data.FastClone();
            var buffer2 = data.FastClone();
            Encrypt(buffer1);
            Encrypt(buffer2);

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                var encryptedSize = (int)(realSize ^ KeyTable2[0]);
                w.Write(encryptedSize);
                w.Write(crc);
                for (var i = 0; i < data.Length; i++)
                {
                    w.Write(data[i]);
                    w.Write(buffer1[i]);
                    w.Write((byte)0);
                    w.Write(buffer2[i]);
                }

                return w.ToArray();
            }
        }
        private static byte[] RemoveX7Junk(byte[] data)
        {
            var newSize = (data.Length - 8) / 4;
            var outBuffer = new byte[newSize];

            for (var i = 0; i < newSize; i++)
                outBuffer[i] = data[i * 4 + 8];

            return outBuffer;
        }

        private static void GetKeyTableIndex(int length, out uint blockIndex, out uint keyIndex)
        {
            var num = (uint)((length - 8) / 4);
            keyIndex = (num ^ KeyTable2[1]) % 10;
            blockIndex = (num ^ KeyTable2[1]) % 2;
        }

        private static void GenerateKeys(this SymmetricAlgorithm aes, int length)
        {
            uint blockIndex;
            uint keyIndex;

            GetKeyTableIndex(length, out blockIndex, out keyIndex);

            var key = AesKey.FastClone();
            var iv = AesIV.FastClone();
            Decrypt(key, blockIndex, keyIndex);
            Decrypt(iv, blockIndex, keyIndex);

            var tmp = new byte[iv.Length + 8];
            Array.Copy(iv, 0, tmp, 0, iv.Length);
            Array.Copy(key, 0, tmp, iv.Length, 8);

            aes.Key = key;
            aes.IV = tmp;
        }
    }

    internal static class S4CRC
    {
        public static long ComputeHash(byte[] data, string fullName)
        {
            long dataCRC = Hash.GetUInt32<CRC32>(data);
            long pathCRC = Hash.GetUInt32<CRC32>(Encoding.ASCII.GetBytes(fullName));
            var finalCRC = dataCRC | (pathCRC << 32);

            var tmp = BitConverter.GetBytes(finalCRC);
            S4Crypto.EncryptCapped(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }
    }

    internal static class X7CRC
    {
        public static uint ComputeHash(byte[] data)
        {
            var crc = Hash.GetUInt32<CRC32>(data);
            return crc ^ 0xBAD0A4B3;
        }
    }
}
