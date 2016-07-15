using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Network;
using Netsphere.Network.Message;
using Netsphere.Network.Message.GameRule;

// ReSharper disable once CheckNamespace
namespace Netsphere.Game.Systems
{
    internal class TeamManager : IReadOnlyDictionary<Team, PlayerTeam>
    {
        private readonly ConcurrentDictionary<Team, PlayerTeam> _teams = new ConcurrentDictionary<Team, PlayerTeam>();

        public Room Room { get; }
        public IEnumerable<Player> Players => _teams.Values.SelectMany(team => team.Players);
        public IEnumerable<Player> PlayersPlaying => _teams.Values.SelectMany(team => team.PlayersPlaying);
        public IEnumerable<Player> Spectators => _teams.Values.SelectMany(team => team.Spectators);

        public EventHandler<TeamChangedEventArgs> TeamChanged;

        protected virtual void OnTeamChanged(PlayerTeam from, PlayerTeam to, Player plr)
        {
            TeamChanged?.Invoke(this, new TeamChangedEventArgs(from, to, plr));
        }

        public TeamManager(Room room)
        {
            Room = room;
        }

        public void Add(Team team, uint playerLimit, uint spectatorLimit)
        {
            var playerTeam = new PlayerTeam(this, team, playerLimit, spectatorLimit);
            if(!_teams.TryAdd(team, playerTeam))
                throw new Exception($"Team {team} already exists");
        }

        public void Remove(Team team)
        {
            _teams.Remove(team);
        }

        public void Join(Player plr)
        {
            // Get teams with space
            var teams = _teams.Values.Where(t => t.PlayerLimit > 0 && t.Players.Count() < t.PlayerLimit).ToArray();

            // get teams with least player count
            var min = (uint)teams.Min(t => t.Count);
            teams = teams.Where(t => t.Count == min).ToArray();

            // get teams with least score
            min = teams.Min(t => t.Score);
            teams = teams.Where(t => t.Score == min).ToArray();

            teams[0].Join(plr);
        }

        public void ChangeTeam(Player plr, Team team, bool report = true)
        {
            //if (plr.Room != Room)
            //throw new RoomException("Player is not inside this room");

            if (Room.GameRuleManager.GameRule.StateMachine.IsInState(GameRuleState.Playing))
                throw new RoomException("Game is running");

            if (plr.RoomInfo.Team == null)
                throw new RoomException("Player is not in a team");

            if (plr.RoomInfo.Team.Team == team)
                throw new RoomException($"Already in team {team}");

            if (plr.RoomInfo.IsReady)
            {
				if(report)
					plr.Session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                throw new RoomException("Player is already ready");
            }

            var oldTeam = plr.RoomInfo.Team;
            var targetTeam = this[team];
            if (targetTeam == null)
                throw new RoomException($"Invalid team {team}");

            try
            {
                targetTeam.Join(plr);
                OnTeamChanged(oldTeam, targetTeam, plr);
            }
            catch (TeamLimitReachedException)
            {
				if(report)
					plr.Session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
            }
        }

        public void ChangeMode(Player plr, PlayerGameMode mode)
        {
            //if (plr.Room != Room)
            //throw new RoomException("Player is not inside this room");

            if (plr.RoomInfo.State != PlayerState.Lobby)
                throw new RoomException("Player is playing");

            if (plr.RoomInfo.Mode == mode)
                throw new RoomException($"Already in mode {mode}");

            if (plr.RoomInfo.Team == null)
                throw new RoomException("Player is not in a team");

            if (plr.RoomInfo.IsReady)
            {
                plr.Session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                throw new RoomException("Player is already ready");
            }

            var team = plr.RoomInfo.Team;
            switch (mode)
            {
                case PlayerGameMode.Normal:
					if (team.Players.Count() >= team.PlayerLimit)
                    {
                        plr.Session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                        throw new TeamLimitReachedException();
                    }
                    break;

                case PlayerGameMode.Spectate:
					Console.WriteLine($"{team.Spectators.Count()},{team.SpectatorLimit}");
					Console.WriteLine($"{team.Spectators.Count() >= team.SpectatorLimit}");
					if (team.Spectators.Count() >= team.SpectatorLimit)
                    {
						plr.Session.Send(new SChangeTeamFailAckMessage(ChangeTeamResult.Full));
                        throw new TeamLimitReachedException();
                    }
                    break;

                default:
                    throw new RoomException($"Invalid mode {mode}");
            }

            plr.RoomInfo.Mode = mode;
            Broadcast(new SPlayerGameModeChangeAckMessage(plr.Account.Id, mode));
        }

        #region Broadcast

        public void Broadcast(GameMessage message)
        {
            foreach (var team in _teams.Values)
                team.Broadcast(message);
        }

        public void Broadcast(GameRuleMessage message)
        {
            foreach (var team in _teams.Values)
                team.Broadcast(message);
        }

        public void Broadcast(ChatMessage message)
        {
            foreach (var team in _teams.Values)
                team.Broadcast(message);
        }

        #endregion

        #region IReadOnlyDictionary

        public int Count => _teams.Count;
        public IEnumerable<Team> Keys => _teams.Keys;
        public IEnumerable<PlayerTeam> Values => _teams.Values;

        public PlayerTeam this[Team key]
        {
            get
            {
                PlayerTeam team;
                TryGetValue(key, out team);
                return team;
            }
        }

        public bool ContainsKey(Team key)
        {
            return _teams.ContainsKey(key);
        }

        public bool TryGetValue(Team key, out PlayerTeam value)
        {
            return _teams.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<Team, PlayerTeam>> GetEnumerator()
        {
            return _teams.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    internal class PlayerTeam : IReadOnlyDictionary<byte, Player>
    {
        private readonly ConcurrentDictionary<byte, Player> _players = new ConcurrentDictionary<byte, Player>();

        public TeamManager TeamManager { get; }
        public Team Team { get; }
        public uint PlayerLimit { get; set; }
        public uint SpectatorLimit { get; set; }
        public uint Score { get; set; }

        public IEnumerable<Player> PlayersPlaying => _players.Values.Where(plr => plr.RoomInfo.Mode == PlayerGameMode.Normal && plr.RoomInfo.State != PlayerState.Lobby);
        public IEnumerable<Player> Players => _players.Values.Where(plr => plr.RoomInfo.Mode == PlayerGameMode.Normal);
        public IEnumerable<Player> Spectators => _players.Values.Where(plr => plr.RoomInfo.Mode == PlayerGameMode.Spectate);

        public EventHandler<RoomPlayerEventArgs> PlayerJoined;
        public EventHandler<RoomPlayerEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(Player plr)
        {
            PlayerJoined?.Invoke(this, new RoomPlayerEventArgs(plr));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            PlayerLeft?.Invoke(this, new RoomPlayerEventArgs(plr));
        }

        public PlayerTeam(TeamManager teamManager, Team team, uint playerLimit, uint spectatorLimit)
        {
            TeamManager = teamManager;
            Team = team;
            PlayerLimit = playerLimit;
            SpectatorLimit = spectatorLimit;
        }

        public void Join(Player plr)
        {
            if (plr.RoomInfo.Team == this)
                throw new RoomException("Actor is already in this team");

            if (plr.RoomInfo.Mode == PlayerGameMode.Normal)
            {
                if (Players.Count() >= PlayerLimit)
                    throw new TeamLimitReachedException();
            }
            else
            {
                if (Spectators.Count() >= SpectatorLimit)
                    throw new TeamLimitReachedException();
            }

            var isChange = false;
            if (plr.RoomInfo.Team != null)
            {
                plr.RoomInfo.Team.Leave(plr);
                isChange = true;
            }

            plr.RoomInfo.Team = this;
            _players.TryAdd(plr.RoomInfo.Slot, plr);

            if (isChange)
                TeamManager.Broadcast(new SChangeTeamAckMessage(plr.Account.Id, Team, plr.RoomInfo.Mode));

            OnPlayerJoined(plr);
        }

        public void Leave(Player plr)
        {
            if (plr.RoomInfo.Team != this)
                return;

            _players.Remove(plr.RoomInfo.Slot);
            plr.RoomInfo.Team = null;

            OnPlayerLeft(plr);
        }

        #region Broadcast

        public void Broadcast(GameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(GameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(ChatMessage message)
        {
            foreach (var plr in _players.Values)
                plr.ChatSession.Send(message);
        }

        #endregion

        #region IReadOnlyDictionary

        public int Count => _players.Count;
        public IEnumerable<byte> Keys => _players.Keys;
        public IEnumerable<Player> Values => _players.Values;

        public Player this[byte key]
        {
            get
            {
                Player plr;
                TryGetValue(key, out plr);
                return plr;
            }
        }

        public bool ContainsKey(byte key)
        {
            return _players.ContainsKey(key);
        }

        public bool TryGetValue(byte key, out Player value)
        {
            return _players.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<byte, Player>> GetEnumerator()
        {
            return _players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
