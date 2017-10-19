using System.IO;
using System.Numerics;
using BlubLib.IO;

namespace Netsphere.Resource.Scene.Chunks
{
    public abstract class SceneChunk : IManualSerializer
    {
        public SceneContainer Container { get; private set; }

        public abstract ChunkType ChunkType { get; }
        public string Name { get; set; }
        public string SubName { get; set; }

        public float Unk1 { get; set; }
        public Matrix4x4 Matrix { get; set; }

        protected SceneChunk(SceneContainer container)
        {
            Name = "";
            SubName = "";
            Unk1 = 0.1f;
            Matrix = Matrix4x4.Identity;
            Container = container;
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk1);
                w.Write(Matrix);
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Unk1 = r.ReadSingle();
                Matrix = r.ReadMatrix();
            }
        }

        public override string ToString()
        {
            return Name + " - " + SubName;
        }
    }
}
