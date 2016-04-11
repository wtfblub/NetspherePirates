using System;
using System.Collections.Generic;
using System.IO;
using SlimMath;

namespace Netsphere.Resource.Scene.Chunks
{
    public class ShapeChunk : SceneChunk
    {
        public override ChunkType ChunkType => ChunkType.BoneSystem;

        public float Unk2 { get; set; }
        public IList<Tuple<Vector3, Vector3>> Unk3 { get; set; }

        public ShapeChunk(SceneContainer container)
            : base(container)
        {
            Unk3 = new List<Tuple<Vector3, Vector3>>();
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk2);
                if (Unk2 >= 0.1000000014901161f)
                {
                    w.Write(Unk3.Count);
                    foreach(var unk in Unk3)
                    {
                        w.Write(unk.Item1.X);
                        w.Write(unk.Item1.Y);
                        w.Write(unk.Item1.Z);

                        w.Write(unk.Item2.X);
                        w.Write(unk.Item2.Y);
                        w.Write(unk.Item2.Z);
                    }
                }
            }
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
            {
                Unk2 = r.ReadSingle();

                if (Unk2 >= 0.1000000014901161f)
                {
                    var count = r.ReadUInt32();
                    for (var i = 0; i < count; i++)
                    {
                        Unk3.Add(Tuple.Create(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()),
                            new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle())));
                    }
                }
            }
        }
    }
}
