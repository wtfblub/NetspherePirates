using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Network.Message.Game;

namespace Netsphere.Server.Game
{
    public class Channel
    {
        private static readonly EventPipeline<ChannelJoinHookEventArgs> s_joinHook =
            new EventPipeline<ChannelJoinHookEventArgs>();

        private readonly IDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();

        public uint Id { get; }
        public ChannelCategory Category { get; }
        public string Name { get; }
        public int PlayerLimit { get; }
        public byte Type { get; }
        public IReadOnlyDictionary<ulong, Player> Players => (IReadOnlyDictionary<ulong, Player>)_players;
        public RoomManager RoomManager { get; }

        public event EventHandler<ChannelEventArgs> PlayerJoined;
        public event EventHandler<ChannelEventArgs> PlayerLeft;
        public static event EventPipeline<ChannelJoinHookEventArgs>.SubscriberDelegate JoinHook
        {
            add => s_joinHook.Subscribe(value);
            remove => s_joinHook.Unsubscribe(value);
        }

        protected virtual void OnPlayerJoined(Player plr)
        {
            PlayerJoined?.Invoke(this, new ChannelEventArgs(this, plr));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            PlayerLeft?.Invoke(this, new ChannelEventArgs(this, plr));
        }

        public Channel(uint id, ChannelCategory category, string name, int playerLimit, byte type, RoomManager roomManager)
        {
            Id = id;
            Category = category;
            Name = name;
            PlayerLimit = playerLimit;
            Type = type;
            RoomManager = roomManager;
            RoomManager.Initialize(this);
        }

        public ChannelJoinError Join(Player plr)
        {
            var eventArgs = new ChannelJoinHookEventArgs(this, plr);
            s_joinHook.Invoke(eventArgs);
            if (eventArgs.Error != ChannelJoinError.OK)
                return eventArgs.Error;

            if (plr.Channel != null)
                return ChannelJoinError.AlreadyInChannel;

            if (Players.Count >= PlayerLimit)
                return ChannelJoinError.ChannelFull;

            _players.Add(plr.Account.Id, plr);
            plr.Channel = this;
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
        }

        public void Broadcast(IGameMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                plr.Session.SendAsync(message);
        }
    }
}
