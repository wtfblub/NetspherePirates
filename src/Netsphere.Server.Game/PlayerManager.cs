using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Netsphere.Database;
using ProudNet;

namespace Netsphere.Server.Game
{
    public class PlayerManager : IReadOnlyCollection<Player>
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly ConcurrentDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();

        public int Count => _players.Count;
        public Player this[ulong accountId] => Get(accountId);

        public event EventHandler<PlayerEventArgs> PlayerConnected;
        public event EventHandler<PlayerEventArgs> PlayerDisconnected;

        protected virtual void OnPlayerConnected(Player plr)
        {
            PlayerConnected?.Invoke(this, new PlayerEventArgs(plr));
        }

        protected virtual void OnPlayerDisconnected(Player plr)
        {
            PlayerDisconnected?.Invoke(this, new PlayerEventArgs(plr));
        }

        public PlayerManager(ILogger<PlayerManager> logger, ISessionManager sessionManager, IDatabaseProvider databaseProvider)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _databaseProvider = databaseProvider;
            _sessionManager.Removed += SessionDisconnected;
        }

        public Player Get(ulong accountId)
        {
            _players.TryGetValue(accountId, out var plr);
            return plr;
        }

        public Player GetByNickname(string nickname)
        {
            return _players.Values.FirstOrDefault(plr =>
                plr.Account.Nickname != null &&
                plr.Account.Nickname.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Add(Player plr)
        {
            if (!_players.TryAdd(plr.Account.Id, plr))
                throw new Exception($"Player {plr.Account.Id} already exists");

            OnPlayerConnected(plr);
        }

        public void Remove(Player plr)
        {
            Remove(plr.Account.Id);
        }

        public void Remove(ulong id)
        {
            _players.Remove(id);
        }

        public bool Contains(Player plr)
        {
            return Contains(plr.Account.Id);
        }

        public bool Contains(ulong id)
        {
            return _players.ContainsKey(id);
        }

        private async void SessionDisconnected(object sender, SessionEventArgs e)
        {
            try
            {
                var session = (Session)e.Session;
                if (session.Player != null && Contains(session.Player))
                {
                    using (session.Player.AddContextToLogger(_logger))
                        _logger.LogInformation("Disconnected - Saving...");

                    using (var db = _databaseProvider.Open<GameContext>())
                        await session.Player.Save(db);

                    OnPlayerDisconnected(session.Player);
                    session.Player.OnDisconnected();
                    Remove(session.Player);
                }
            }
            catch (Exception ex)
            {
                e.Session.Channel.Pipeline.FireExceptionCaught(ex);
            }
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return _players.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
