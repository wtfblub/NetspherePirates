using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Network.Message;
using ExpressMapper.Extensions;
using Netsphere.Network;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;

namespace Netsphere
{
    internal class Channel
    {
        private readonly IDictionary<ulong, Player> _players = new ConcurrentDictionary<ulong, Player>();
        public uint Id { get; set; }
        public ChannelCategory Category { get; set; }
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
        public byte Type { get; set; }

        #region Events

        public event EventHandler<ChannelPlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<ChannelPlayerLeftEventArgs> PlayerLeft;
        public event EventHandler<ChannelMessageEventArgs> Message;

        protected virtual void OnPlayerJoined(ChannelPlayerJoinedEventArgs e)
        {
            PlayerJoined?.Invoke(this, e);
        }

        protected virtual void OnPlayerLeft(ChannelPlayerLeftEventArgs e)
        {
            PlayerLeft?.Invoke(this, e);
        }

        protected virtual void OnMessage(ChannelMessageEventArgs e)
        {
            Message?.Invoke(this, e);
        }

        #endregion

        public IReadOnlyDictionary<ulong, Player> Players => (IReadOnlyDictionary<ulong, Player>)_players;
        public RoomManager RoomManager { get; }

        public Channel()
        {
            RoomManager = new RoomManager(this);
        }

        public void Update(TimeSpan delta)
        {
            RoomManager.Update(delta);
        }

        public void Join(Player plr)
        {
            if (plr.Channel != null)
                throw new ChannelException("Player is already inside a channel");

            if(Players.Count >= PlayerLimit)
                throw new ChannelLimitReachedException();


            BroadcastChat(new SChannelEnterPlayerAckMessage(plr.Map<Player, UserDataWithNickDto>()));

            _players.Add(plr.Account.Id, plr);
            plr.SentPlayerList = false;
            plr.Channel = this;

            plr.Session.Send(new SServerResultInfoAckMessage(ServerResult.ChannelEnter));
            OnPlayerJoined(new ChannelPlayerJoinedEventArgs(this, plr));

            plr.ChatSession.Send(new SNoteReminderInfoAckMessage((byte) plr.Mailbox.Count(mail => mail.IsNew), 0, 0));
        }

        public void Leave(Player plr)
        {
            if (plr.Channel != this)
                return;

            _players.Remove(plr.Account.Id);
            plr.Channel = null;

            BroadcastChat(new SChannelLeavePlayerAckMessage(plr.Account.Id));

            OnPlayerLeft(new ChannelPlayerLeftEventArgs(this, plr));
        }

        public void SendChatMessage(Player plr, string message)
        {
            OnMessage(new ChannelMessageEventArgs(this, plr, message));

            foreach (var p in Players.Values.Where(p => !p.DenyManager.Contains(plr.Account.Id) && p.Room == null))
                p.ChatSession.Send(new SChatMessageAckMessage(ChatType.Channel, plr.Account.Id, plr.Account.Nickname, message));
        }

        public void Broadcast(IMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                plr.Session.Send(message);
        }

        public void BroadcastChat(IMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                plr.ChatSession.Send(message);
        }
    }
}
