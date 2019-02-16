using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
    public class TeamManager : IReadOnlyDictionary<TeamId, Team>
    {
        private readonly ConcurrentDictionary<TeamId, Team> _teams = new ConcurrentDictionary<TeamId, Team>();
        private readonly object _mutex = new object();

        public Room Room { get; }
        public IEnumerable<Player> Players => _teams.Values.SelectMany(team => team.Players);
        public IEnumerable<Player> PlayersPlaying => _teams.Values.SelectMany(team => team.PlayersPlaying);
        public IEnumerable<Player> Spectators => _teams.Values.SelectMany(team => team.Spectators);

        public EventHandler<TeamEventArgs> PlayerJoinedTeam;
        public EventHandler<TeamEventArgs> PlayerLeftTeam;
        public EventHandler<PlayerTeamChangedEventArgs> PlayerTeamChanged;

        private void OnPlayerJoinedTeam(object _, TeamEventArgs e)
        {
            PlayerJoinedTeam?.Invoke(this, e);
        }

        private void OnPlayerLeftTeam(object _, TeamEventArgs e)
        {
            PlayerLeftTeam?.Invoke(this, e);
        }

        protected virtual void OnPlayerTeamChanged(Player plr, Team sourceTeam, Team targetTeam)
        {
            PlayerTeamChanged?.Invoke(this, new PlayerTeamChangedEventArgs(plr, sourceTeam, targetTeam));
        }

        public TeamManager(Room room)
        {
            Room = room;
        }

        public bool Add(TeamId id, int playerLimit, int spectatorLimit)
        {
            if (playerLimit < 0 || spectatorLimit < 0)
                throw new ArgumentOutOfRangeException();

            var team = new Team(this, id, (uint)playerLimit, (uint)spectatorLimit, _mutex);
            if (_teams.TryAdd(id, team))
            {
                team.PlayerJoined += OnPlayerJoinedTeam;
                team.PlayerLeft += OnPlayerLeftTeam;
                return true;
            }

            return false;
        }

        public bool Remove(Team team)
        {
            return Remove(team.Id);
        }

        public bool Remove(TeamId id)
        {
            if (_teams.TryRemove(id, out var team))
            {
                team.PlayerJoined -= OnPlayerJoinedTeam;
                team.PlayerLeft -= OnPlayerLeftTeam;
                return true;
            }

            return false;
        }

        public TeamJoinError Join(Player plr)
        {
            lock (_mutex)
            {
                IEnumerable<Team> teams;
                if (plr.Mode == PlayerGameMode.Spectate)
                {
                    teams = _teams.Values.Where(x => x.SpectatorLimit > 0 && x.Spectators.Count() < x.SpectatorLimit);
                }
                else
                {
                    teams = _teams.Values
                        .Where(x => x.PlayerLimit > 0 && x.Players.Count() < x.PlayerLimit)
                        .OrderBy(x => x.Count) // Order by player count
                        .ThenBy(x => x.Score) // Order by score
                        .ThenBy(t => t.Players.Sum(p => p.Score.GetTotalScore())); // Order by player score sum
                }

                return teams.FirstOrDefault()?.Join(plr) ?? TeamJoinError.TeamFull;
            }
        }

        public TeamChangeError ChangeTeam(Player plr, TeamId teamId)
        {
            lock (_mutex)
            {
                if (plr.Room != Room)
                    return TeamChangeError.WrongRoom;

                if (Room.GameRule.StateMachine.GameState != GameState.Waiting)
                    return TeamChangeError.GameIsRunning;

                if (plr.Team == null)
                    return TeamChangeError.NotInTeam;

                if (plr.Team.Id == teamId)
                    return TeamChangeError.AlreadyInTeam;

                if (plr.IsReady)
                    return TeamChangeError.PlayerIsReady;

                var sourceTeam = plr.Team;
                var targetTeam = this[teamId];
                if (targetTeam == null)
                    return TeamChangeError.InvalidTeam;

                var joinError = targetTeam.Join(plr);
                if (joinError != TeamJoinError.OK)
                    return TeamChangeError.Full;

                //Broadcast(new SChangeTeamAckMessage(plr.Account.Id, plr.Team.Id, plr.Mode));
                OnPlayerTeamChanged(plr, sourceTeam, targetTeam);
                return TeamChangeError.OK;
            }
        }

        public TeamChangeModeError ChangeMode(Player plr, PlayerGameMode mode)
        {
            lock (_mutex)
            {
                if (plr.Room != Room)
                    return TeamChangeModeError.WrongRoom;

                if (plr.State != PlayerState.Lobby)
                    return TeamChangeModeError.PlayerIsPlaying;

                if (plr.Mode == mode)
                    return TeamChangeModeError.AlreadyInMode;

                if (plr.Team == null)
                    return TeamChangeModeError.NotInTeam;

                if (plr.IsReady)
                    return TeamChangeModeError.PlayerIsReady;

                var team = plr.Team;
                switch (mode)
                {
                    case PlayerGameMode.Normal:
                        if (team.Players.Count() >= team.PlayerLimit)
                            return TeamChangeModeError.Full;

                        break;

                    case PlayerGameMode.Spectate:
                        if (team.Spectators.Count() >= team.SpectatorLimit)
                            return TeamChangeModeError.Full;

                        break;

                    default:
                        return TeamChangeModeError.InvalidMode;
                }

                plr.Mode = mode;
                Broadcast(new SPlayerGameModeChangeAckMessage(plr.Account.Id, mode));
                return TeamChangeModeError.OK;
            }
        }

        public void SwapPlayer(Player a, Player b)
        {
            lock (_mutex)
            {
                if (a.Team == b.Team)
                    return;

                var teamA = a.Team;
                var teamB = b.Team;
                teamA.Leave(a);
                teamB.Leave(b);
                teamA.Join(b);
                teamB.Join(a);
            }
        }

        public void Broadcast(IGameMessage message)
        {
            foreach (var team in _teams.Values)
                team.Broadcast(message);
        }

        public void Broadcast(IGameRuleMessage message)
        {
            foreach (var team in _teams.Values)
                team.Broadcast(message);
        }

        #region IReadOnlyDictionary
        public int Count => _teams.Count;
        public IEnumerable<TeamId> Keys => _teams.Keys;
        public IEnumerable<Team> Values => _teams.Values;
        public Team this[TeamId key]
        {
            get
            {
                TryGetValue(key, out var team);
                return team;
            }
        }

        public bool ContainsKey(TeamId key)
        {
            return _teams.ContainsKey(key);
        }

        public bool TryGetValue(TeamId key, out Team value)
        {
            return _teams.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TeamId, Team>> GetEnumerator()
        {
            return _teams.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class Team : IReadOnlyDictionary<byte, Player>
    {
        private readonly ConcurrentDictionary<byte, Player> _players = new ConcurrentDictionary<byte, Player>();
        private readonly object _mutex;

        public TeamManager TeamManager { get; }
        public TeamId Id { get; }
        public uint PlayerLimit { get; set; }
        public uint SpectatorLimit { get; set; }
        public uint Score { get; set; }

        public IEnumerable<Player> PlayersPlaying =>
            _players.Values.Where(plr => plr.Mode == PlayerGameMode.Normal && plr.State != PlayerState.Lobby);
        public IEnumerable<Player> Players => _players.Values.Where(plr => plr.Mode == PlayerGameMode.Normal);
        public IEnumerable<Player> Spectators => _players.Values.Where(plr => plr.Mode == PlayerGameMode.Spectate);

        public EventHandler<TeamEventArgs> PlayerJoined;
        public EventHandler<TeamEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(Player plr)
        {
            PlayerJoined?.Invoke(this, new TeamEventArgs(this, plr));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            PlayerLeft?.Invoke(this, new TeamEventArgs(this, plr));
        }

        public Team(TeamManager teamManager, TeamId id, uint playerLimit, uint spectatorLimit, object mutex)
        {
            TeamManager = teamManager;
            Id = id;
            PlayerLimit = playerLimit;
            SpectatorLimit = spectatorLimit;
            _mutex = mutex;
        }

        public TeamJoinError Join(Player plr)
        {
            lock (_mutex)
            {
                if (plr.Team == this)
                    return TeamJoinError.AlreadyInTeam;

                if (plr.Mode == PlayerGameMode.Normal)
                {
                    if (Players.Count() >= PlayerLimit)
                        return TeamJoinError.TeamFull;
                }
                else
                {
                    if (Spectators.Count() >= SpectatorLimit)
                        return TeamJoinError.TeamFull;
                }

                var isChange = false;
                if (plr.Team != null && plr.Team.Id != Id)
                {
                    plr.Team.Leave(plr);
                    isChange = true;
                }

                plr.Team = this;
                _players.TryAdd(plr.Slot, plr);

                if (isChange)
                    TeamManager.Broadcast(new SChangeTeamAckMessage(plr.Account.Id, Id, plr.Mode));

                OnPlayerJoined(plr);
                return TeamJoinError.OK;
            }
        }

        public void Leave(Player plr)
        {
            lock (_mutex)
            {
                if (plr.Team != this)
                    return;

                _players.Remove(plr.Slot);
                plr.Team = null;
                OnPlayerLeft(plr);
            }
        }

        public void Broadcast(IGameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(IGameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        #region IReadOnlyDictionary
        public int Count => _players.Count;
        public IEnumerable<byte> Keys => _players.Keys;
        public IEnumerable<Player> Values => _players.Values;

        public Player this[byte key]
        {
            get
            {
                TryGetValue(key, out var plr);
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
