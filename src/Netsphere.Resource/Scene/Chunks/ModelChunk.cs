using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.IO;
using SlimMath;
using System;
using System.Text;

namespace Netsphere.Resource.Scene.Chunks
{
    public class ModelChunk : SceneChunk
    {

        public override ChunkType ChunkType => ChunkType.ModelData;

        public float Unk2 { get; set; }
        public RenderState RenderState { get; set; }
        public TextureData TextureData { get; set; }
        public MeshData Mesh { get; set; }
        public IList<WeightData> WeightData { get; set; }
        public IDictionary<string, TransformKeyData2> Animation { get; set; }

        public ModelChunk(SceneContainer container)
            : base(container)
        {
            Unk1 = 0.1f;
            Matrix = Matrix.Identity;
            Unk2 = 0.1f;
            RenderState = RenderState.None;
            TextureData = new TextureData(this);
            Mesh = new MeshData(this);
            WeightData = new List<WeightData>();
            Animation = new Dictionary<string, TransformKeyData2>();
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk2);

                w.WriteEnum(RenderState);

                w.Serialize(TextureData);
                w.Serialize(Mesh);

                w.Write(WeightData.Count);
                w.Serialize(WeightData);

                w.Write(Animation.Count);
                foreach (var pair in Animation)
                {
                    w.WriteCString(pair.Key);
                    w.Serialize(pair.Value);
                }
            }
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
            {
                // ## CoreLib::Scene::CRenderable
                Unk2 = r.ReadSingle();
                RenderState = r.ReadEnum<RenderState>();
                // ## CoreLib::Scene::CRenderable

                TextureData = new TextureData(this);
                TextureData.Deserialize(stream);

                Mesh = new MeshData(this);
                Mesh.Deserialize(stream);

                WeightData = r.DeserializeArray<WeightData>(r.ReadInt32()).ToList();

                var count = r.ReadInt32();
                for (var i = 0; i < count; ++i)
                    Animation.Add(r.ReadCString(), r.Deserialize<TransformKeyData2>());
            }
        }
    }

    public class MeshData : IManualSerializer
    {
        public ModelChunk ModelChunk { get; }

        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Faces { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector2> UV { get; set; }
        public List<Vector2> UV2 { get; set; }

        public List<Vector3> Unk { get; set; }

        public MeshData(ModelChunk modelChunk)
        {
            Vertices = new List<Vector3>();
            Faces = new List<Vector3>();
            Normals = new List<Vector3>();
            UV = new List<Vector2>();
            UV2 = new List<Vector2>();
            Unk = new List<Vector3>();

            ModelChunk = modelChunk;
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Vertices.Count);
                foreach (var vertex in Vertices)
                {
                    w.Write(vertex.X);
                    w.Write(vertex.Y);
                    w.Write(vertex.Z);
                }

                w.Write(Faces.Count);
                foreach (var face in Faces)
                {
                    w.Write((short)face.X);
                    w.Write((short)face.Y);
                    w.Write((short)face.Z);
                }

                w.Write(Normals.Count);
                foreach (var normal in Normals)
                {
                    w.Write(normal.X);
                    w.Write(normal.Y);
                    w.Write(normal.Z);
                }

                w.Write(UV.Count);
                foreach (var uv in UV)
                {
                    w.Write(uv.X);
                    w.Write(uv.Y);
                }

                if (ModelChunk.TextureData.Unk2 == 1)
                {
                    foreach (var uv in UV2)
                    {
                        w.Write(uv.X);
                        w.Write(uv.Y);
                    }
                }

                w.Write(Unk.Count);
                foreach (var unk in Unk)
                {
                    w.Write(unk.X);
                    w.Write(unk.Y);
                    w.Write(unk.Z);
                }
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                var count = r.ReadInt32();
                for (var i = 0; i < count; i++)
                    Vertices.Add(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));

                count = r.ReadInt32();
                for (var i = 0; i < count; i++)
                    Faces.Add(new Vector3(r.ReadInt16(), r.ReadInt16(), r.ReadInt16()));

                count = r.ReadInt32();
                for (var i = 0; i < count; i++)
                    Normals.Add(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));

                count = r.ReadInt32();
                for (var i = 0; i < count; i++)
                    UV.Add(new Vector2(r.ReadSingle(), r.ReadSingle()));

                if (ModelChunk.TextureData.Unk2 == 1)
                {
                    for (var i = 0; i < count; i++)
                        UV2.Add(new Vector2(r.ReadSingle(), r.ReadSingle()));
                }

                count = r.ReadInt32();
                for (var i = 0; i < count; i++)
                    Unk.Add(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));
            }
        }
    }

    public class WeightData : IManualSerializer
    {
        public string Unk1 { get; set; }
        public Matrix Matrix { get; set; }
        public IList<Vector2> Unk2 { get; set; }

        public WeightData()
        {
            Unk1 = "";
            Matrix = Matrix.Identity;
            Unk2 = new List<Vector2>();
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteCString(Unk1);
                w.Write(Matrix);

                w.Write(Unk2.Count);
                foreach (var unk in Unk2)
                {
                    w.Write(unk.X);
                    w.Write(unk.Y);
                }
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Unk1 = r.ReadCString();
                Matrix = r.ReadMatrix();

                var count = r.ReadUInt32();
                for (var i = 0; i < count; i++)
                    Unk2.Add(new Vector2(r.ReadSingle(), r.ReadSingle()));
            }
        }
    }

    // Game::CActorGeomData
    public class TextureData : IManualSerializer
    {
        public float Unk1 { get; set; }
        public ModelChunk ModelChunk { get; private set; }
        public int Unk2 { get; set; }
        public List<TextureEntry> Textures { get; set; }

        public TextureData(ModelChunk modelChunk)
        {
            ModelChunk = modelChunk;
            Textures = new List<TextureEntry>();
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk1);
                if (Unk1 >= 0.2000000029802322f)
                    w.Write(Unk2);

                w.Write(Textures.Count);
                foreach (var texture in Textures)
                {
                    w.WriteCString(texture.FileName, 1024);
                    if (Unk1 >= 0.2000000029802322f)
                    {
                      byte[] fileName2 = Encoding.ASCII.GetBytes(texture.FileName2);
                      w.Write(fileName2, 0, 1024);
                      //w.WriteCString(texture.FileName2, 1024);
                    }

                    w.Write(texture.FaceCounter);
                    w.Write(texture.FaceCount);

                }
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Unk1 = r.ReadSingle();

                if (Unk1 >= 0.2000000029802322f)
                    Unk2 = r.ReadInt32();

                var count = r.ReadUInt32();
                for (var i = 0; i < count; i++)
                {
                    var textureData = new TextureEntry
                    {
                        FileName = r.ReadCString(1024),
                        FileName2 = ""
                    };

                    if (Unk1 >= 0.2000000029802322f)
                    {
                      char[] fileName2 = r.ReadChars(1024);
                      textureData.FileName2 = new string(fileName2);
                      //textureData.FileName2 = r.ReadCString(1024);
                    }

                    textureData.FaceCounter = r.ReadInt32();
                    textureData.FaceCount = r.ReadInt32();

                    Textures.Add(textureData);
                }
            }
        }
    }

    public struct TextureEntry
    {
        public string FileName;
        public string FileName2;
        public int FaceCounter;
        public int FaceCount;
    }
}
