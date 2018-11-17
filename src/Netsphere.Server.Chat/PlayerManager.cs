using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using ProudNet;

namespace Netsphere.Server.Chat
{
    public class PlayerManager : IReadOnlyCollection<Session>
    {
        private readonly ISessionManager _sessionManager;
        private readonly ConcurrentDictionary<ulong, Session> _players = new ConcurrentDictionary<ulong, Session>();

        public int Count => _players.Count;
        public Session this[ulong accountId] => Get(accountId);

        public PlayerManager(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.Removed += SessionDisconnected;
        }

        public Session Get(ulong accountId)
        {
            _players.TryGetValue(accountId, out var plr);
            return plr;
        }

        public Session GetByNickname(string nickname)
        {
            return _players.Values.FirstOrDefault(plr =>
                plr.Nickname != null &&
                plr.Nickname.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Add(Session plr)
        {
            if (!_players.TryAdd(plr.AccountId, plr))
                throw new Exception($"Player {plr.AccountId} already exists");
        }

        public void Remove(Session plr)
        {
            Remove(plr.AccountId);
        }

        public void Remove(ulong id)
        {
            _players.Remove(id);
        }

        public bool Contains(Session plr)
        {
            return Contains(plr.AccountId);
        }

        public bool Contains(ulong id)
        {
            return _players.ContainsKey(id);
        }

        private void SessionDisconnected(object sender, SessionEventArgs e)
        {
            var session = (Session)e.Session;
            if (session.AccountId != 0 && Contains(session))
                Remove(session);
        }

        public IEnumerator<Session> GetEnumerator()
        {
            return _players.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
