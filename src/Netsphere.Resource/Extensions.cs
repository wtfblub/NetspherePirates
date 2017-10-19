using System.IO;
using System.Numerics;

namespace Netsphere.Resource
{
    public static class ResourceExtensions
    {
        internal static Matrix4x4 ReadMatrix(this BinaryReader r)
        {
            return new Matrix4x4(
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()
                );
        }

        internal static void Write(this BinaryWriter w, Matrix4x4 value)
        {
            w.Write(value.M11);
            w.Write(value.M12);
            w.Write(value.M13);
            w.Write(value.M14);

            w.Write(value.M21);
            w.Write(value.M22);
            w.Write(value.M23);
            w.Write(value.M24);

            w.Write(value.M31);
            w.Write(value.M32);
            w.Write(value.M33);
            w.Write(value.M34);

            w.Write(value.M41);
            w.Write(value.M42);
            w.Write(value.M43);
            w.Write(value.M44);
        }
    }
}
