using System;
using System.IO;
using SlimMath;

namespace Netsphere.Resource.Scene.Chunks
{
    public class BoxChunk : SceneChunk
    {
        public override ChunkType ChunkType => ChunkType.Box;

        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public int Unk4 { get; set; }
        public float Unk5 { get; set; }
        public int Unk6 { get; set; }
        public Vector3[] Unk7 { get; set; }
        public Vector3 Size { get; set; }

        public BoxChunk(SceneContainer container)
            : base(container)
        {
            Unk2 = 0.1f;
            Unk3 = 0.1f;
            Unk4 = 0;
            Unk5 = 0;
            Unk6 = 0;
            Unk7 = new []
            {
                new Vector3(1, 0, 0), 
                new Vector3(0, 1, 0), 
                new Vector3(0, 0, 1)
            };
            Size = new Vector3();
        }

        public override void Serialize(Stream stream)
        {
            if (Unk7.Length != 3)
                throw new Exception("Unk7 must have a length of 3");

            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk2);
                w.Write(Unk3);
                w.Write(Unk4);
                w.Write(Unk5);
                w.Write(Unk6);

                foreach (var vec in Unk7)
                {
                    w.Write(vec.X);
                    w.Write(vec.Y);
                    w.Write(vec.Z);
                }

                w.Write(Size.X);
                w.Write(Size.Y);
                w.Write(Size.Z);
            }
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
            {
                Unk2 = r.ReadSingle();
                Unk3 = r.ReadSingle();
                Unk4 = r.ReadInt32();
                Unk5 = r.ReadSingle();
                Unk6 = r.ReadInt32();

                for (var i = 0; i < 3; i++)
                    Unk7[i] = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                Size = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            }
        }
    }
}
