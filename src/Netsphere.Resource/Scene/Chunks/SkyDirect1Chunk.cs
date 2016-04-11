using System;
using System.IO;

namespace Netsphere.Resource.Scene.Chunks
{
    public class SkyDirect1Chunk : SceneChunk
    {
        public override ChunkType ChunkType => ChunkType.SkyDirect1;
        public byte[] Data { get; set; }

        public SkyDirect1Chunk(SceneContainer container)
            : base(container)
        {
            Data = new byte[96];
        }

        public override void Serialize(Stream stream)
        {
            if (Data.Length != 96)
                throw new Exception("SkyDirect1 data must have 164 bytes");

            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
                w.Write(Data);
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
                Data = r.ReadBytes(96);
        }
    }
}
