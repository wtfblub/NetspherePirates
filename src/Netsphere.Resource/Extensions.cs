using System.IO;
using SlimMath;

namespace Netsphere.Resource
{
    public static class ResourceExtensions
    {
        internal static Matrix ReadMatrix(this BinaryReader r)
        {
            var tmp = new float[16];
            for (var i = 0; i < 16; i++)
                tmp[i] = r.ReadSingle();
            return new Matrix(tmp);
        }

        internal static void Write(this BinaryWriter w, Matrix value)
        {
            foreach (var val in value.ToArray())
                w.Write(val);
        }
    }
}
