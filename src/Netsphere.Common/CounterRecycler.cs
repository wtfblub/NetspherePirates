using System.Collections.Concurrent;
using System.Threading;

namespace Netsphere.Common
{
    public class CounterRecycler
    {
        private readonly ConcurrentQueue<uint> _ids;
        private long _idCounter;

        public CounterRecycler(uint startNumber = 0)
        {
            _ids = new ConcurrentQueue<uint>();
            _idCounter = 0;
        }

        public uint GetId()
        {
            if (_ids.TryDequeue(out var id))
                return id;

            return (uint)Interlocked.Increment(ref _idCounter);
        }

        public void Return(uint id)
        {
            _ids.Enqueue(id);
        }
    }
}
