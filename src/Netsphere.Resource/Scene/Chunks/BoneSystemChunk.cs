using System.IO;

namespace Netsphere.Resource.Scene.Chunks
{
    public class BoneSystemChunk : SceneChunk
    {
        public override ChunkType ChunkType => ChunkType.BoneSystem;

        public int Unk2 { get; set; }

        public BoneSystemChunk(SceneContainer container)
            : base(container)
        { }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk2);
            }
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
            {
                Unk2 = r.ReadInt32();
            }
        }
    }
}
