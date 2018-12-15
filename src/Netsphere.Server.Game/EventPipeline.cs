using System.Collections.Generic;
using System.Threading;

namespace Netsphere.Server.Game
{
    public class EventPipeline<TEventArgs>
    {
        public delegate bool SubscriberDelegate(TEventArgs eventArgs);

        private readonly ReaderWriterLockSlim _mutex = new ReaderWriterLockSlim();
        private readonly HashSet<SubscriberDelegate> _subscribers = new HashSet<SubscriberDelegate>();

        public void Subscribe(SubscriberDelegate subscriber)
        {
            _mutex.EnterWriteLock();
            _subscribers.Add(subscriber);
            _mutex.ExitWriteLock();
        }

        public void Unsubscribe(SubscriberDelegate subscriber)
        {
            _mutex.EnterWriteLock();
            _subscribers.Remove(subscriber);
            _mutex.ExitWriteLock();
        }

        public void Invoke(TEventArgs eventArgs)
        {
            _mutex.EnterReadLock();
            try
            {
                foreach (var subscriber in _subscribers)
                {
                    var continueChain = subscriber(eventArgs);
                    if (!continueChain)
                        break;
                }
            }
            finally
            {
                _mutex.ExitReadLock();
            }
        }
    }
}
