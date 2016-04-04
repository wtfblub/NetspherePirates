using BlubLib.Threading.Tasks;
using SharpLzo;

namespace Netsphere.Network
{
    public static class NetsphereExtensions
    {
        private static readonly AsyncLock _sync = new AsyncLock();

        public static byte[] CompressLZO(this byte[] data)
        {
            using(_sync.Lock())
                return miniLzo.Compress(data);
        }

        public static byte[] DecompressLZO(this byte[] data, int realSize)
        {
            using (_sync.Lock())
                return miniLzo.Decompress(data, realSize);
        }
    }
}
