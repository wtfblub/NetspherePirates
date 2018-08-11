using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BlubLib.Collections.Concurrent;

namespace ProudNet
{
    internal class SessionManager<TKey> : IInternalSessionManager<TKey>
    {
        private readonly ConcurrentDictionary<TKey, ProudSession> _sessions;

        public event EventHandler<SessionEventArgs> Added;
        public event EventHandler<SessionEventArgs> Removed;

        protected virtual void OnAdded(ProudSession session)
        {
            Added?.Invoke(this, new SessionEventArgs(session));
        }

        protected virtual void OnRemoved(ProudSession session)
        {
            Removed?.Invoke(this, new SessionEventArgs(session));
        }

        public IReadOnlyDictionary<TKey, ProudSession> Sessions => _sessions;

        public SessionManager()
        {
            _sessions = new ConcurrentDictionary<TKey, ProudSession>();
        }

        public ProudSession GetSession(TKey key)
        {
            return _sessions.GetValueOrDefault(key);
        }

        public void Broadcast(object message)
        {
            foreach (var session in _sessions.Values)
                session.Send(message);
        }

        public void Broadcast(object message, Predicate<ProudSession> predicate)
        {
            foreach (var session in _sessions.Values)
            {
                if (predicate(session))
                    session.Send(message);
            }
        }

        public void AddSession(TKey key, ProudSession session)
        {
            if (!_sessions.TryAdd(key, session))
                throw new ProudException($"Session {key} is already registered");

            OnAdded(session);
        }

        public void RemoveSession(TKey key)
        {
            if (_sessions.TryRemove(key, out var session))
                OnRemoved(session);
        }
    }

    internal class SessionManager : SessionManager<uint>, ISessionManager
    {
    }

    public interface ISessionManager : ISessionManager<uint>
    {
    }

    public interface ISessionManager<TKey>
    {
        event EventHandler<SessionEventArgs> Added;
        event EventHandler<SessionEventArgs> Removed;

        IReadOnlyDictionary<TKey, ProudSession> Sessions { get; }

        ProudSession GetSession(TKey key);

        void Broadcast(object message);

        void Broadcast(object message, Predicate<ProudSession> predicate);
    }

    internal interface IInternalSessionManager<TKey> : ISessionManager<TKey>
    {
        void AddSession(TKey key, ProudSession session);

        void RemoveSession(TKey key);
    }
}
