using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.IO;
using SlimMath;

namespace Netsphere.Resource.Scene.Chunks
{
    public class TransformKeyData2 : TransformKeyData
    {
        public IList<MorphKey> MorphKeys { get; set; }

        public TransformKeyData2()
        {
            MorphKeys = new List<MorphKey>();
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(MorphKeys.Count);
                w.Serialize(MorphKeys);
            }
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);

            using (var r = stream.ToBinaryReader(true))
                MorphKeys = r.DeserializeArray<MorphKey>(r.ReadInt32()).ToList();
        }
    }

    public class TransformKeyData : IManualSerializer
    {
        public TimeSpan Duration { get; set; }
        public TransformKey TransformKey { get; set; }
        public IList<FloatKey> FloatKeys { get; set; }

        public TransformKeyData()
        {
            Duration = TimeSpan.Zero;
            FloatKeys = new List<FloatKey>();
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write((uint)Duration.TotalMilliseconds);

                w.Write(TransformKey != null);
                if (TransformKey != null)
                    w.Serialize(TransformKey);

                w.Write(FloatKeys.Count);
                w.Serialize(FloatKeys);
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Duration = TimeSpan.FromMilliseconds(r.ReadUInt32());

                var flag = r.ReadBoolean();
                if (flag)
                    TransformKey = r.Deserialize<TransformKey>();

                FloatKeys = r.DeserializeArray<FloatKey>(r.ReadInt32()).ToList();
            }
        }
    }

    public class TransformKey : IManualSerializer
    {
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        // frames
        // int = frame time
        public IList<Tuple<int, Vector3>> Unk1 { get; set; }
        public IList<Tuple<int, Quaternion>> Unk2 { get; set; }
        public IList<Tuple<int, Vector3>> Unk3 { get; set; }

        public TransformKey()
        {
            Translation = Vector3.Zero;
            Rotation = Quaternion.Zero;
            Scale = Vector3.Zero;

            Unk1 = new List<Tuple<int, Vector3>>();
            Unk2 = new List<Tuple<int, Quaternion>>();
            Unk3 = new List<Tuple<int, Vector3>>();
        }

        public void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Translation.X);
                w.Write(Translation.Y);
                w.Write(Translation.Z);

                w.Write(Rotation.X);
                w.Write(Rotation.Y);
                w.Write(Rotation.Z);
                w.Write(Rotation.W);

                w.Write(Scale.X);
                w.Write(Scale.Y);
                w.Write(Scale.Z);

                w.Write(Unk1.Count);
                foreach (var unk in Unk1)
                {
                    w.Write(unk.Item1);

                    w.Write(unk.Item2.X);
                    w.Write(unk.Item2.Y);
                    w.Write(unk.Item2.Z);
                }

                w.Write(Unk2.Count);
                foreach (var unk in Unk2)
                {
                    w.Write(unk.Item1);

                    w.Write(unk.Item2.X);
                    w.Write(unk.Item2.Y);
                    w.Write(unk.Item2.Z);
                    w.Write(unk.Item2.W);
                }

                w.Write(Unk3.Count);
                foreach (var unk in Unk3)
                {
                    w.Write(unk.Item1);

                    w.Write(unk.Item2.X);
                    w.Write(unk.Item2.Y);
                    w.Write(unk.Item2.Z);
                }
            }
        }

        public void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Translation = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                Rotation = new Quaternion(new Vector4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));
                Scale = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                var count = r.ReadUInt32();
                for (var n = 0; n < count; n++)
                    Unk1.Add(Tuple.Create(r.ReadInt32(), new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle())));

                count = r.ReadUInt32();
                for (var n = 0; n < count; n++)
                    Unk2.Add(Tuple.Create(r.ReadInt32(), new Quaternion(new Vector4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()))));

                count = r.ReadUInt32();
                for (var n = 0; n < count; n++)
                    Unk3.Add(Tuple.Create(r.ReadInt32(), new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle())));
            }
        }
    }

    public class FloatKey : IManualSerializer
    {
        public int Unk1 { get; set; }
        public float Unk2 { get; set; }

        public void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk1);
                w.Write(Unk2);
            }
        }

        public void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Unk1 = r.ReadInt32();
                Unk2 = r.ReadSingle();
            }
        }
    }

    public class MorphKey : IManualSerializer
    {
        public int Unk { get; set; }
        public IList<Quaternion> Rotations { get; set; }
        public IList<Vector3> Positions { get; set; }

        public MorphKey()
        {
            Rotations = new List<Quaternion>();
            Positions = new List<Vector3>();
        }

        public void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.Write(Unk);

                w.Write(Rotations.Count);
                foreach (var rotation in Rotations)
                {
                    w.Write(rotation.X);
                    w.Write(rotation.Y);
                    w.Write(rotation.Z);
                    w.Write(rotation.W);
                }

                w.Write(Positions.Count);
                foreach (var position in Positions)
                {
                    w.Write(position.X);
                    w.Write(position.Y);
                    w.Write(position.Z);
                }
            }
        }

        public void Deserialize(Stream stream)
        {
            using (var r = stream.ToBinaryReader(true))
            {
                Unk = r.ReadInt32();

                var count = r.ReadInt32();
                for (var j = 0; j < count; j++)
                    Rotations.Add(new Quaternion(new Vector4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle())));

                count = r.ReadInt32();
                for (var j = 0; j < count; j++)
                    Positions.Add(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()));
            }
        }
    }
}
