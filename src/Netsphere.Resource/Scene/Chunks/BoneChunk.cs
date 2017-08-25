using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.IO;

namespace Netsphere.Resource.Scene.Chunks
{
    public class BoneChunk : SceneChunk
    {
        public override ChunkType ChunkType => ChunkType.Bone;

        public float Unk2 { get; set; }
        public IList<Tuple<string, string, TransformKeyData>> Animation { get; set; }

        public BoneChunk(SceneContainer container)
            : base(container)
        {
            Unk2 = 0.1f;
            Animation = new List<Tuple<string, string, TransformKeyData>>();
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk2);

                w.Write(Animation.Count);
                foreach (var tuple in Animation)
                {
                    if (Unk2 >= 0.2000000029802322f)
                    {
                        w.WriteCString(tuple.Item1);
                        w.WriteCString(tuple.Item2);
                        if (string.IsNullOrWhiteSpace(tuple.Item2))
                            w.Serialize(tuple.Item3);
                    }
                    else
                    {
                        w.WriteCString(tuple.Item1);
                        w.Serialize(tuple.Item3);
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

                var count = r.ReadUInt32();
                for (var i = 0; i < count; i++)
                {
                    if (Unk2 >= 0.2000000029802322f)
                    {
                        var name = r.ReadCString();
                        var subName = r.ReadCString();
                        TransformKeyData transformKeyDatas = null;

                        if (string.IsNullOrWhiteSpace(subName))
                            transformKeyDatas = r.Deserialize<TransformKeyData>();

                        Animation.Add(Tuple.Create(name, subName, transformKeyDatas));
                    }
                    else
                    {
                        var name = r.ReadCString();
                        Animation.Add(Tuple.Create(name, default(string), r.Deserialize<TransformKeyData>()));
                    }
                }
            }
        }
    }
}
