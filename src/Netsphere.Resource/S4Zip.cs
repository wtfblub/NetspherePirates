using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BlubLib.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
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
            data = Decrypt(data);
            using (var r = data.ToBinaryReader())
            {
                if (r.ReadInt32() != 1)
                    throw new Exception("Invalid s4 league file container");

                var entryCount = r.ReadInt32();
                if (entryCount < 0)
                    throw new Exception("Invalid s4 league file container");

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

                var data = w.ToArray();
                data = Encrypt(data);
                File.WriteAllBytes(fileName, data);
            }
        }

        public S4ZipEntry CreateEntry(string fullName, byte[] data)
        {
            fullName = fullName.ToLower();
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
            fullName = fullName.ToLower();
            S4ZipEntry entry;
            if (!_entries.TryGetValue(fullName, out entry))
                throw new ArgumentException(fullName + " does not exist", fullName);

            if (deleteFromDisk && File.Exists(entry.FileName))
                File.Delete(entry.FileName);
            _entries.Remove(fullName);
            return entry;
        }

        private static byte[] Encrypt(byte[] data)
        {
            return data.EncryptSeed();
        }

        private static byte[] Decrypt(byte[] data)
        {
            return data.DecryptSeed();
        }

        private static byte[] EncryptEntry(byte[] data)
        {
            S4Crypt.OldCapped32.Encrypt(data);
            return data;
        }

        private static byte[] DecryptEntry(byte[] data)
        {
            S4Crypt.OldCapped32.Decrypt(data);
            return data;
        }

        public static byte[] EncryptS4(byte[] data)
        {
            var realSize = data.Length;
            var buffer = miniLzo.Compress(data);
            S4Crypt.Old40.Encrypt(buffer);

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

            S4Crypt.Old40.Decrypt(buffer);
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
                if (isX7)
                    data = data.EncryptX7();
                data = data.EncryptSeed();
            }

            Checksum = GetChecksum(data);
            Length = data.Length;

            S4Crypt.OldCapped32.Encrypt(data);
            if (data.Length < 1048576)
                data = miniLzo.Compress(data);
            data.SwapBytes();

            return data;
        }

        private byte[] Decrypt(byte[] data)
        {
            data.SwapBytes();
            if (data.Length < 1048576)
                data = miniLzo.Decompress(data, Length);
            S4Crypt.OldCapped32.Decrypt(data);

            var isX7 = Name.EndsWith(".x7", StringComparison.InvariantCultureIgnoreCase);
            if (Name.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase) || isX7)
            {
                data = data.DecryptSeed();
                if (isX7)
                    data = data.DecryptX7();
            }

            return data;
        }

        private long GetChecksum(byte[] data)
        {
            return data.S4CRC(FullName);
        }
    }

    internal class S4Crypt
    {
        private static readonly byte[] s_keyTable =
        {
            0x82, 0x53, 0x43, 0x4C, 0x2B,
            0x0D, 0x37, 0xD7, 0xD9, 0xD8,
            0x1B, 0x6D, 0xA0, 0xC3, 0x2B,
            0xEE, 0x45, 0x88, 0x1A, 0xA6,
            0x18, 0x1D, 0x9D, 0x38, 0x2A,
            0x55, 0x03, 0x1D, 0xCD, 0xA6,
            0x73, 0x07, 0xED, 0x8D, 0xC5,
            0xDB, 0xA3, 0xBD, 0xB6, 0xD5
        };

        // ReSharper disable RedundantExplicitArraySize
        private static readonly byte[][][] s_keyTable2 = new byte[2][][]
        {
            #region Table1
            new byte[10][]
            {
                new byte[40]
                {
                    0xA1,0x0C,0x24,0x7A,0xE3,
                    0x8C,0x77,0xC0,0x49,0xC0,
                    0x93,0x9A,0x23,0x82,0x8D,
                    0xC8,0x9D,0xB3,0xE4,0x50,
                    0xB1,0xE2,0x9E,0x44,0x15,
                    0x54,0x0B,0x22,0x64,0xBD,
                    0x8B,0x3A,0x53,0xA5,0x33,
                    0x0A,0x0A,0x73,0x1E,0x6A,
                },
                new byte[40]
                {
                    0x70,0xB0,0xB7,0xE6,0x51,
                    0x4E,0xB5,0x38,0x8A,0x37,
                    0x10,0xDA,0x29,0xD6,0xAA,
                    0x63,0x7A,0x74,0x3B,0x7B,
                    0x9E,0x74,0xB4,0xDD,0x33,
                    0x5E,0x41,0x21,0x7C,0x65,
                    0xA3,0x26,0x44,0x95,0x75,
                    0x34,0x54,0x95,0x3C,0x5E,
                },
                new byte[40]
                {
                    0x28,0xA9,0xB8,0x0E,0x85,
                    0xC7,0x66,0x3C,0xDC,0x28,
                    0x19,0x10,0x43,0x78,0x64,
                    0xA6,0x2B,0x60,0xCD,0x7D,
                    0x4D,0xA1,0x60,0xC7,0x9B,
                    0x76,0x97,0x12,0xCB,0xA4,
                    0x22,0xB9,0x51,0x79,0xB9,
                    0x74,0xD3,0x93,0xC3,0xE6,
                },
                new byte[40]
                {
                    0x68,0x5C,0x59,0xDB,0xE6,
                    0xA6,0x28,0x4A,0xBA,0x01,
                    0xB8,0x67,0xBA,0x37,0x90,
                    0xC1,0x47,0x80,0x40,0xBA,
                    0x95,0x57,0xCA,0xCD,0xAE,
                    0x69,0xD8,0x2B,0xC7,0x0D,
                    0x98,0x54,0x8E,0x95,0x73,
                    0x10,0x26,0x64,0xAA,0x60,
                },
                new byte[40]
                {
                    0x66,0xB6,0x41,0x92,0x13,
                    0x6C,0x41,0x91,0x8D,0x91,
                    0xD1,0x7E,0xC3,0x80,0xAE,
                    0xC4,0xEE,0x65,0x28,0x9D,
                    0xEE,0x7A,0x2C,0xDB,0xC3,
                    0xA1,0x72,0x26,0x20,0x72,
                    0x41,0x40,0x5A,0x6D,0x01,
                    0x88,0x05,0x08,0x29,0x30,
                },
                new byte[40]
                {
                    0x14,0x15,0x97,0x07,0x07,
                    0x30,0xE7,0x67,0x51,0xB4,
                    0x89,0x21,0x78,0x68,0x68,
                    0xE4,0xA9,0x18,0x59,0x38,
                    0x75,0x43,0x52,0x04,0xC9,
                    0xD8,0x5C,0x38,0x95,0x03,
                    0xD9,0x27,0xE5,0xDB,0xDA,
                    0x28,0x0B,0xB0,0xB3,0xE3,
                },
                new byte[40]
                {
                    0xDC,0xE3,0x3D,0x0D,0x42,
                    0xA7,0xE4,0x1C,0x73,0x47,
                    0xDB,0x27,0xA3,0x64,0x08,
                    0x26,0xC3,0x5E,0x3E,0xA2,
                    0x6E,0xB6,0xA2,0x22,0x3C,
                    0x08,0x88,0x03,0x01,0x73,
                    0x24,0x09,0xBD,0x3A,0x2E,
                    0x13,0x9E,0xBA,0xD3,0x99,
                },
                new byte[40]
                {
                    0x30,0x67,0x01,0x18,0x61,
                    0x41,0xEA,0x84,0x86,0xDA,
                    0x7A,0x1C,0x83,0xBE,0x67,
                    0x85,0x27,0x60,0x20,0xE7,
                    0xBC,0x37,0xBC,0x51,0xC6,
                    0x6B,0x32,0x05,0x67,0x9B,
                    0xE5,0x3A,0x7C,0xA8,0xC7,
                    0x58,0xA1,0x53,0x53,0x78,
                },
                new byte[40]
                {
                    0x8D,0xC0,0x52,0x20,0xEE,
                    0xC8,0x74,0xC5,0xAA,0x83,
                    0x0C,0x90,0xD0,0xBC,0x58,
                    0x63,0x2A,0x1B,0x9E,0x93,
                    0x04,0x2A,0x05,0x8E,0xED,
                    0x9C,0x27,0x37,0x57,0xDE,
                    0x3C,0xD9,0xB3,0x43,0xC3,
                    0x29,0x70,0x88,0x94,0x10,
                },
                new byte[40]
                {
                    0xDB,0xC3,0xE9,0x63,0x52,
                    0xEA,0x5A,0x18,0xB8,0xEB,
                    0x6A,0x1B,0xEA,0x01,0x37,
                    0x9A,0xA5,0x3A,0x11,0x9A,
                    0xB5,0x02,0x74,0x71,0x01,
                    0x10,0x0C,0x09,0x97,0xA0,
                    0xD7,0x8B,0xB0,0x06,0x62,
                    0x18,0x9A,0x6D,0xBE,0x87,
                }
            },
            #endregion
                
            #region Table2
            new byte[10][]
            {
                new byte[40]
                {
                    0x69,0x45,0xD9,0x28,0x4C,
                    0x97,0x8E,0x14,0xE6,0x84,
                    0x26,0x58,0x0D,0xE2,0x51,
                    0x84,0xE0,0x41,0xED,0xC3,
                    0x7B,0x97,0xCE,0x5D,0x1C,
                    0x7C,0x9E,0x0D,0x01,0x66,
                    0x4C,0x7D,0x08,0xDD,0x17,
                    0x8E,0x55,0x3A,0x3C,0x27,
                },
                new byte[40]
                {
                    0x43,0x25,0xC0,0x8C,0x21,
                    0x34,0x7D,0xB2,0xB5,0x3D,
                    0xCD,0xCE,0x53,0xB6,0x76,
                    0x81,0xBA,0x5E,0xCB,0x04,
                    0x5D,0x84,0x87,0xD4,0xCA,
                    0x69,0x21,0x65,0x3E,0xDE,
                    0x87,0x44,0x52,0x0A,0x28,
                    0x8A,0x90,0x62,0x63,0x71,
                },
                new byte[40]
                {
                    0x11,0xB7,0xA1,0x01,0xA4,
                    0x09,0xA3,0xEA,0x49,0xDC,
                    0x5B,0x11,0x35,0x8E,0x80,
                    0x36,0xDD,0x4E,0x38,0xD5,
                    0x88,0x57,0xBE,0x2E,0x98,
                    0x36,0x0C,0x05,0xA2,0xBB,
                    0xCB,0xE9,0x51,0xE1,0x6D,
                    0xA0,0x3C,0x63,0x28,0x43,
                },
                new byte[40]
                {
                    0xE9,0x63,0xC0,0xB4,0x0E,
                    0x89,0x83,0x29,0x65,0xC7,
                    0x86,0x8D,0xA0,0xA4,0x67,
                    0x95,0x7C,0xA1,0xC0,0x53,
                    0xDB,0xD7,0xD8,0x04,0x0C,
                    0x62,0x84,0x89,0x43,0x4D,
                    0xD1,0x73,0x21,0x1D,0x6A,
                    0xBD,0x31,0x61,0x20,0x34,
                },
                new byte[40]
                {
                    0xDB,0x35,0x45,0x9A,0x54,
                    0x8C,0xD1,0x9C,0x25,0x53,
                    0x16,0xC3,0x2A,0x60,0xA5,
                    0x5B,0x4E,0x7B,0xEB,0x24,
                    0x1B,0x36,0xC7,0x41,0x23,
                    0x52,0x74,0x33,0x28,0xC6,
                    0x45,0x2E,0x42,0x6D,0x42,
                    0xEE,0xE2,0x97,0x9B,0xDE,
                },
                new byte[40]
                {
                    0x37,0x0D,0xD7,0xD8,0xD9,
                    0x53,0x82,0x43,0x2B,0x4C,
                    0x6D,0x1B,0xA0,0x2B,0xC3,
                    0x1D,0x18,0x9D,0x2A,0x38,
                    0x45,0xEE,0x88,0xA6,0x1A,
                    0x03,0x55,0x1D,0xA6,0xCD,
                    0xA3,0xDB,0xBD,0xD5,0xB6,
                    0x07,0x73,0xED,0xC5,0x8D,
                },
                new byte[40]
                {
                    0x7E,0x5E,0x50,0x14,0x64,
                    0x08,0x3B,0x21,0x47,0x2D,
                    0xE7,0x8B,0x27,0x79,0x56,
                    0x85,0x23,0x74,0x24,0x47,
                    0x85,0xCB,0x5E,0x17,0x4B,
                    0xA9,0x75,0x10,0x85,0xEB,
                    0xD0,0x20,0x86,0x78,0x7D,
                    0x69,0x42,0x57,0x07,0x4A,
                },
                new byte[40]
                {
                    0xE6,0x27,0x78,0x2E,0x7A,
                    0x90,0x7A,0x29,0x62,0x04,
                    0x61,0x20,0x23,0x15,0x2E,
                    0x20,0x7D,0x50,0x07,0x97,
                    0x98,0x6D,0x62,0x62,0x25,
                    0x8E,0x54,0x7C,0x0C,0x37,
                    0x72,0x6B,0xE6,0x22,0xE9,
                    0x2E,0x38,0xC9,0x06,0x56,
                },
                new byte[40]
                {
                    0x57,0x88,0x01,0x3A,0x4A,
                    0x52,0x69,0xBD,0x4A,0x8C,
                    0x01,0x9B,0x49,0x14,0x78,
                    0x32,0x86,0xA4,0x95,0x02,
                    0x50,0xBC,0x31,0xCD,0x39,
                    0x30,0x71,0x9C,0x5B,0x4D,
                    0x67,0x21,0xE8,0x82,0x5C,
                    0xD6,0x9B,0xBD,0x63,0x26,
                },
                new byte[40]
                {
                    0x61,0xA6,0xBE,0x37,0xAD,
                    0x0E,0xD5,0xD5,0xE7,0xD4,
                    0x28,0x36,0xB1,0xE2,0x20,
                    0x80,0x0C,0x77,0x2C,0x0E,
                    0x7D,0x66,0xA9,0x10,0xD2,
                    0x13,0xD6,0x65,0x17,0x70,
                    0x04,0x53,0xEB,0x84,0x0D,
                    0x52,0xAE,0x3B,0x40,0xEE,
                }
            }
            #endregion
        };
        // ReSharper restore RedundantExplicitArraySize

        public static S4Crypt Default { get; } = new S4Crypt();
        public static S4Crypt Old32 { get; } = new S4Crypt(1, 32);
        public static S4Crypt Old40 { get; } = new S4Crypt(1);
        public static S4Crypt OldCapped32 { get; } = new S4Crypt(1, 32, 256);
        public static S4Crypt OldCapped40 { get; } = new S4Crypt(1, 40, 256);

        private readonly int _version;
        private readonly int _keySize;
        private readonly int _lengthLimit;

        public S4Crypt(int version = 2, int keySize = 40, int lengthLimit = 0)
        {
            if (version > 2 || version < 1)
                throw new ArgumentOutOfRangeException(nameof(version));

            if (keySize < 1)
                throw new ArgumentOutOfRangeException(nameof(keySize));

            _version = version;
            _keySize = keySize;

            if (lengthLimit < 0)
                lengthLimit = 0;
            _lengthLimit = lengthLimit;
        }

        public void Encrypt(byte[] data, int lengthForKeySearch = 0, int blockIndex = -1)
        {
            var key = _version == 1 ? s_keyTable : GetKey(data.Length);
            var length = data.Length;
            if (_lengthLimit > 0 && length > _lengthLimit)
                length = _lengthLimit;

            for (var i = 0; i < length; ++i)
            {
                byte x;
                switch (_version)
                {
                    case 1:
                        x = (byte)(data[i] ^ key[i % _keySize]);
                        data[i] = (byte)(((x & 0x7F) << 1) | ((x & 0x80) >> 7));
                        break;

                    case 2:
                        x = data[i];
                        data[i] = (byte)(((x & 0x80) >> 7) & 1 | (x << 1) & 0xFE);
                        data[i] ^= key[i % _keySize];
                        break;
                }
            }
        }

        public void Decrypt(byte[] data, int lengthForKeySearch = 0, int blockIndex = -1)
        {
            var key = _version == 1 ? s_keyTable : GetKey(data.Length);
            var length = data.Length;
            if (_lengthLimit > 0 && length > _lengthLimit)
                length = _lengthLimit;

            for (var i = 0; i < length; ++i)
            {
                byte x;
                switch (_version)
                {
                    case 1:
                        x = data[i];
                        data[i] = (byte)(((x >> 1) & 0x7F) | ((x & 1) << 7));
                        data[i] ^= key[i % _keySize];
                        break;

                    case 2:
                        x = (byte)(data[i] ^ key[i % _keySize]);
                        data[i] = (byte)((x >> 1) & 0x7F | ((x & 1) << 7) & 0x80);
                        break;
                }
            }
        }

        private static byte[] GetKey(int length)
        {
            const uint xorKey = 0xCD4802EF;

            var num = (uint)((length - 8) >> 2);
            var keyIndex = (num ^ xorKey) % 10;
            var blockIndex = (num ^ xorKey) % 2;
            return s_keyTable2[blockIndex][keyIndex];
        }
    }

    internal static class S4CryptoUtilities
    {
        private static readonly SecureRandom s_random = new SecureRandom();

        private static byte[] BuildX7(byte[] data, uint crc, int realSize)
        {
            var encrypted = data.FastClone();
            S4Crypt.Default.Encrypt(encrypted);

            using (var w = new BinaryWriter(new MemoryStream()))
            {
                var encryptedSize = (int)(realSize ^ 0xFE292513);
                w.Write(encryptedSize);
                w.Write(crc);
                for (var i = 0; i < data.Length; i++)
                {
                    w.Write(data[i]);
                    w.Write(encrypted[i]);
                    w.Write((byte)0);
                    w.Write(encrypted[i]);
                }

                return w.ToArray();
            }
        }

        private static byte[] RemoveX7Junk(byte[] data)
        {
            var newSize = (data.Length - 8) >> 2;
            var outBuffer = new byte[newSize];

            for (var i = 0; i < newSize; i++)
                outBuffer[i] = data[i * 4 + 8];

            return outBuffer;
        }

        private static uint X7CRC(byte[] data)
        {
            var crc = Hash.GetUInt32<CRC32>(data);
            return crc ^ 0xBAD0A4B3;
        }

        #region Extensions

        public static long S4CRC(this byte[] @this, string fullName)
        {
            long dataCRC = Hash.GetUInt32<CRC32>(@this);
            long pathCRC = Hash.GetUInt32<CRC32>(Encoding.ASCII.GetBytes(fullName));
            var finalCRC = dataCRC | (pathCRC << 32);

            var tmp = BitConverter.GetBytes(finalCRC);
            S4Crypt.OldCapped32.Encrypt(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }

        public static void SwapBytes(this byte[] @this)
        {
            var size = @this.Length;
            var i = 0;
            var sizeCapped = size >= 128 ? 128 : size;

            while (i < sizeCapped / 2)
            {
                var j = size - 1 - i;
                var swap = @this[j];
                @this[j] = @this[i];
                @this[i++] = swap;
            }
        }

        public static void SwapBlocks(this byte[] @this)
        {
            const int blockSize = 16;
            var buffer = new byte[blockSize];

            var numBlocks = @this.Length / blockSize;
            for (var i = 0; i < numBlocks; i++)
            {
                Array.Copy(@this, i * blockSize, buffer, 0, blockSize);
                for (var j = 0; j < blockSize; j++)
                {
                    var block = j / 4;
                    var blockIndex = j % 4;
                    @this[i * blockSize + j] = buffer[blockIndex * 4 + block];
                }
            }
        }

        public static byte[] InsertKeys(this byte[] @this, byte[] key, byte[] iv)
        {
            byte[] output;

            if (@this.Length >= 6)
            {
                var blockSize = @this.Length / 3;

                using (var r = @this.ToBinaryReader())
                {
                    using (var w = new BinaryWriter(new MemoryStream()))
                    {
                        w.Write(r.ReadBytes(blockSize));
                        w.Write(key);
                        w.Write(r.ReadBytes(blockSize));
                        w.Write(iv);
                        w.Write(r.ReadBytes(@this.Length - (int)r.BaseStream.Position));

                        output = w.ToArray();
                    }
                }
            }
            else
            {
                using (var r = @this.ToBinaryReader())
                {
                    using (var w = new BinaryWriter(new MemoryStream()))
                    {
                        w.Write(key);
                        w.Write(r.ReadBytes(@this.Length));
                        w.Write(iv);

                        output = w.ToArray();
                    }
                }
            }
            return output;
        }

        public static byte[] ExtractKeys(this byte[] @this, out byte[] key, out byte[] iv)
        {
            var newSize = @this.Length - 16 * 2;
            byte[] output;

            if (newSize >= 6)
            {
                var blockSize = newSize / 3;
                using (var r = @this.ToBinaryReader())
                {
                    using (var w = new BinaryWriter(new MemoryStream()))
                    {
                        w.Write(r.ReadBytes(blockSize));
                        key = r.ReadBytes(16);
                        w.Write(r.ReadBytes(blockSize));
                        iv = r.ReadBytes(16);
                        w.Write(r.ReadBytes(@this.Length - (int)r.BaseStream.Position));

                        output = w.ToArray();
                    }
                }
            }
            else
            {
                using (var r = @this.ToBinaryReader())
                {
                    using (var w = new BinaryWriter(new MemoryStream()))
                    {
                        key = r.ReadBytes(16);
                        w.Write(r.ReadBytes(newSize));
                        iv = r.ReadBytes(16);

                        output = w.ToArray();
                    }
                }
            }
            return output;
        }

        public static byte[] EncryptSeed(this byte[] @this)
        {
            var key = new byte[16];
            var iv = new byte[16];
            s_random.NextBytes(key);
            s_random.NextBytes(iv);

            @this = @this.FastClone();
            @this.SwapBlocks();

            var parameters = new ParametersWithIV(new KeyParameter(key), iv);
            var cipher = CipherUtilities.GetCipher("SEED/SIC");
            cipher.Init(true, parameters);

            var output = new byte[cipher.GetOutputSize(@this.Length)];
            var len = cipher.ProcessBytes(@this, 0, @this.Length, output, 0);
            cipher.DoFinal(output, len);

            S4Crypt.Default.Encrypt(output);
            output = output.InsertKeys(key, iv);
            output.SwapBlocks();

            return output;
        }

        public static byte[] DecryptSeed(this byte[] @this)
        {
            byte[] key;
            byte[] iv;

            @this = @this.FastClone();
            @this.SwapBlocks();
            @this = @this.ExtractKeys(out key, out iv);
            S4Crypt.Default.Decrypt(@this);

            var parameters = new ParametersWithIV(new KeyParameter(key), iv);
            var cipher = CipherUtilities.GetCipher("SEED/SIC");
            cipher.Init(false, parameters);

            var output = new byte[cipher.GetOutputSize(@this.Length)];
            var len = cipher.ProcessBytes(@this, 0, @this.Length, output, 0);
            cipher.DoFinal(output, len);

            output.SwapBlocks();
            return output;
        }

        public static byte[] EncryptX7(this byte[] @this)
        {
            var crc = X7CRC(@this);
            var realSize = @this.Length;

            @this = miniLzo.Compress(@this);
            @this = BuildX7(@this, crc, realSize);

            return @this;
        }

        public static byte[] DecryptX7(this byte[] @this)
        {
            var realSize = (int)(BitConverter.ToInt32(@this, 0) ^ 0xFE292513);
            @this = RemoveX7Junk(@this);

            LzoResult res;
            @this = miniLzo.Decompress(@this, realSize, out res);
            return @this;
        }

        #endregion
    }
}
