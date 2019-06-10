using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Generic;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Server.Chat
{
    public class Channel
    {
        private readonly PlayerManager _playerManager;
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

        public Channel(uint id, PlayerManager playerManager)
        {
            _playerManager = playerManager;
            Id = id;
        }

        public void Join(Player plr)
        {
            _players.Add(plr.Account.Id, plr);
            plr.Channel = this;
            Broadcast(new ChannelEnterPlayerAckMessage(plr.Map<Player, PlayerInfoShortDto>()));
            plr.Session.Send(new ChannelPlayerListAckMessage(
                Players.Values.Where(x => x.RoomId == 0).Select(x => x.Map<Player, PlayerInfoShortDto>()).ToArray()
            ));
            _playerManager.Where(x => x.Channel == null).ForEach(x =>
                x.Session.Send(new ChannelLeavePlayerAckMessage(plr.Account.Id))
            );
            OnPlayerJoined(plr);
        }

        public void Leave(Player plr)
        {
            _players.Remove(plr.Account.Id);
            plr.Channel = null;
            plr.SentPlayerList = false;
            Broadcast(new ChannelLeavePlayerAckMessage(plr.Account.Id));
            plr.Session.Send(new ChannelPlayerListAckMessage(
                _playerManager.Where(x => x.Channel == null).Select(x => x.Map<Player, PlayerInfoShortDto>()).ToArray()
            ));
            _playerManager.Where(x => x.Channel == null).ForEach(x =>
                x.Session.Send(new ChannelEnterPlayerAckMessage(plr.Map<Player, PlayerInfoShortDto>()))
            );
            OnPlayerLeft(plr);
        }

        public void Broadcast(IChatMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.RoomId == 0))
                plr.Session.Send(message);
        }

        public void SendChatMessage(Player sender, string message)
        {
            Broadcast(new MessageChatAckMessage(ChatType.Channel, sender.Account.Id, sender.Account.Nickname, message), true);
        }
    }
}
