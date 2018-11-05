using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Network;
using Netsphere.Network.Message.Game;

namespace Netsphere.Server.Game
{
    public class Channel
    {
        private readonly IDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();

        public uint Id { get; }
        public ChannelCategory Category { get; }
        public string Name { get; }
        public int PlayerLimit { get; }
        public byte Type { get; }
        public IReadOnlyDictionary<ulong, Player> Players => (IReadOnlyDictionary<ulong, Player>)_players;

        public event EventHandler<ChannelEventArgs> PlayerJoined;
        public event EventHandler<ChannelEventArgs> PlayerLeft;

        protected virtual void OnPlayerJoined(Player plr)
        {
            PlayerJoined?.Invoke(this, new ChannelEventArgs(this, plr));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            PlayerLeft?.Invoke(this, new ChannelEventArgs(this, plr));
        }

        public Channel(uint id, ChannelCategory category, string name, int playerLimit, byte type)
        {
            Id = id;
            Category = category;
            Name = name;
            PlayerLimit = playerLimit;
            Type = type;
        }

        public ChannelJoinError Join(Player plr)
        {
            if (plr.Channel != null)
                return ChannelJoinError.AlreadyInChannel;

            if (Players.Count >= PlayerLimit)
                return ChannelJoinError.ChannelFull;

            _players.Add(plr.Account.Id, plr);
            plr.Channel = this;
            plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelEnter));
            OnPlayerJoined(plr);
            return ChannelJoinError.OK;
        }

        public void Leave(Player plr)
        {
            if (plr.Channel != this)
                return;

            _players.Remove(plr.Account.Id);
            plr.Channel = null;
            OnPlayerLeft(plr);
            plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelLeave));
        }

        public void Broadcast(IGameMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                plr.Session.SendAsync(message);
        }
    }
}
