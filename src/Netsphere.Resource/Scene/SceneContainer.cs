using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.IO;
using Netsphere.Resource.Scene.Chunks;
using SlimMath;

namespace Netsphere.Resource.Scene
{
    public class SceneContainer : List<SceneChunk>
    {
        public SceneHeader Header { get; }

        public SceneContainer()
        {
            Header = new SceneHeader();
        }

        public SceneContainer(IEnumerable<SceneChunk> collection)
            : base(collection)
        {
            Header = new SceneHeader();
        }

        #region ReadFrom
        
        public static SceneContainer ReadFrom(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadFrom(fs);
        }

        public static SceneContainer ReadFrom(byte[] data)
        {
            using (var s = new MemoryStream(data))
                return ReadFrom(s);
        }

        public static SceneContainer ReadFrom(Stream stream)
        {
            var container = new SceneContainer();

            using (var r = new BinaryReader(stream))
            {
                container.Header.Deserialize(stream);

                // CoreLib::Scene::CSceneGroup
                var chunkCount = r.ReadUInt32();

                if (container.Header.Unk2 >= 0.2000000029802322f)
                    r.ReadByte(); // ToDo ReadString

                for (var i = 0; i < chunkCount; i++)
                {
                    var type = r.ReadEnum<ChunkType>();
                    var name = r.ReadCString();
                    var subName = r.ReadCString();

                    SceneChunk chunk;
                    switch (type)
                    {
                        case ChunkType.ModelData:
                            chunk = new ModelChunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        case ChunkType.Box:
                            chunk = new BoxChunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        case ChunkType.Bone:
                            chunk = new BoneChunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        case ChunkType.BoneSystem:
                            chunk = new BoneSystemChunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        case ChunkType.Shape:
                            chunk = new ShapeChunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        case ChunkType.SkyDirect1:
                            chunk = new SkyDirect1Chunk(container)
                            {
                                Name = name,
                                SubName = subName
                            };
                            chunk.Deserialize(stream);
                            container.Add(chunk);
                            break;

                        default:
                            throw new Exception($"Unknown chunk type: 0x{(int)type:X4} StreamPosition: {r.BaseStream.Position}");
                    }
                }
            }

            return container;
        }

        #endregion

        public void Write(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                Write(fs);
        }

        public void Write(Stream stream)
        {
            using (var w = new BinaryWriter(stream))
            {
                w.Serialize(Header);

                w.Write(Count);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Header.Unk2 >= 0.2000000029802322f)
                    w.Write((byte)0);

                foreach (var chunk in this)
                {
                    w.WriteEnum(chunk.ChunkType);
                    w.WriteCString(chunk.Name);
                    w.WriteCString(chunk.SubName);

                    w.Serialize(chunk);
                }
            }
        }
    }

    public class SceneHeader : IManualSerializer
    {
        //public const uint Version = 1;
        public const uint Magic = 0x6278d57a;

        public string Name { get; set; }
        public string SubName { get; set; }
        public float Unk1 { get; set; }
        public Matrix Matrix { get; set; }
        public float Unk2 { get; set; }

        internal SceneHeader()
        {
            Name = "";
            SubName = "";
            Unk1 = 0.1f;
            Matrix = Matrix.Identity;
            Unk2 = 0.1f;
        }

        public void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                //w.Write(Version);
                w.Write(Magic);

                w.WriteCString(Name);
                w.WriteCString(SubName);

                w.Write(Unk1);
                w.Write(Matrix);
                w.Write(Unk2);
            }
        }

        public void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                uint value;
                do
                {
                    value = r.ReadUInt32();
                    if (value != Magic)
                        r.BaseStream.Seek(-3, SeekOrigin.Current);
                } while (value != Magic);

                Name = r.ReadCString();
                SubName = r.ReadCString();

                // CoreLib::Scene::CSceneNode
                Unk1 = r.ReadSingle();
                Matrix = r.ReadMatrix();

                // CoreLib::Scene::CSceneGroup
                Unk2 = r.ReadSingle();
            }
        }
    }
}
