using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using BlubLib.Collections.Generic;
using Logging;
using Netsphere.Database;
using Netsphere.Network.Message.Chat;
using ProudNet;

namespace Netsphere.Server.Chat
{
    public class PlayerManager : IReadOnlyCollection<Player>
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _sessionManager;
        private readonly DatabaseService _databaseService;
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

        public PlayerManager(ILogger<PlayerManager> logger, ISessionManager sessionManager, DatabaseService databaseService)
        {
            _logger = logger;
            _sessionManager = sessionManager;
            _databaseService = databaseService;
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
            var session = (Session)e.Session;
            var plr = session.Player;

            try
            {
                if (plr != null && Contains(plr))
                {
                    plr.AddContextToLogger(_logger).Information("Disconnected - Saving...");

                    using (var db = _databaseService.Open<GameContext>())
                    {
                        await session.Player.Save(db);
                        await db.SaveChangesAsync();
                    }

                    _players.Values.Where(x => x.Channel == null).ForEach(x =>
                        x.Session.Send(new ChannelLeavePlayerAckMessage(session.Player.Account.Id))
                    );

                    OnPlayerDisconnected(plr);
                    plr.OnDisconnected();
                }
            }
            catch (Exception ex)
            {
                e.Session.Channel.Pipeline.FireExceptionCaught(ex);
            }

            if (plr != null)
                Remove(plr);
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
