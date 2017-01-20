using System.Collections.Concurrent;
using System.Threading;

namespace ProudNet
{
    public interface IHostIdFactory
    {
        uint New();
        void Free(uint hostId);
    }

    public class HostIdFactory : IHostIdFactory
    {
        private long _counter = 1000;
        private readonly ConcurrentStack<uint> _pool = new ConcurrentStack<uint>();

        public uint New()
        {
            uint hostId;
            return _pool.TryPop(out hostId) ? hostId : (uint)Interlocked.Increment(ref _counter);
        }

        public void Free(uint hostId)
        {
            _pool.Push(hostId);
        }
    }
}
