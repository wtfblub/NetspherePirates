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
        private readonly ConcurrentStack<uint> _pool = new ConcurrentStack<uint>();
        private long _counter = 1000;

        public uint New()
        {
            return _pool.TryPop(out var hostId) ? hostId : (uint)Interlocked.Increment(ref _counter);
        }

        public void Free(uint hostId)
        {
            _pool.Push(hostId);
        }
    }
}
