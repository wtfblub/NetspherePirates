using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Server.Chat
{
    public class Channel
    {
        private readonly IDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();

        public uint Id { get; }
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

        public Channel(uint id)
        {
            Id = id;
        }

        public void Join(Player plr)
        {
            _players.Add(plr.Account.Id, plr);
            plr.Channel = this;
            Broadcast(new SChannelEnterPlayerAckMessage(plr.Map<Player, UserDataWithNickDto>()));
            OnPlayerJoined(plr);
        }

        public void Leave(Player plr)
        {
            _players.Remove(plr.Account.Id);
            plr.Channel = null;
            Broadcast(new SChannelLeavePlayerAckMessage(plr.Account.Id));
            OnPlayerLeft(plr);
        }

        public void Broadcast(IChatMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || !plr.IsInRoom))
                plr.Session.Send(message);
        }

        public void SendChatMessage(Player sender, string message)
        {
            Broadcast(new SChatMessageAckMessage(ChatType.Channel, sender.Account.Id, sender.Account.Nickname, message), true);
        }
    }
}
