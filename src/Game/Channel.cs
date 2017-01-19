using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (Players.Count >= PlayerLimit)
                throw new ChannelLimitReachedException();

            BroadcastAsync(new SChannelEnterPlayerAckMessage(plr.Map<Player, UserDataWithNickDto>())).WaitEx();

            _players.Add(plr.Account.Id, plr);
            plr.SentPlayerList = false;
            plr.Channel = this;

            plr.Session.SendAsync(new SServerResultInfoAckMessage(ServerResult.ChannelEnter)).WaitEx();
            OnPlayerJoined(new ChannelPlayerJoinedEventArgs(this, plr));

            plr.ChatSession.SendAsync(new SNoteReminderInfoAckMessage((byte)plr.Mailbox.Count(mail => mail.IsNew), 0, 0)).WaitEx();
        }

        public void Leave(Player plr)
        {
            if (plr.Channel != this)
                throw new ChannelException("Player is not in this channel");

            _players.Remove(plr.Account.Id);
            plr.Channel = null;

            BroadcastAsync(new SChannelLeavePlayerAckMessage(plr.Account.Id)).WaitEx();

            OnPlayerLeft(new ChannelPlayerLeftEventArgs(this, plr));
            plr.Session?.SendAsync(new SServerResultInfoAckMessage(ServerResult.ChannelLeave)).WaitEx();
        }

        public async Task SendChatMessageAsync(Player plr, string message)
        {
            OnMessage(new ChannelMessageEventArgs(this, plr, message));

            foreach (var p in Players.Values.Where(p => !p.DenyManager.Contains(plr.Account.Id) && p.Room == null))
                await p.ChatSession.SendAsync(new SChatMessageAckMessage(ChatType.Channel, plr.Account.Id, plr.Account.Nickname, message))
                    .ConfigureAwait(false);
        }

        public async Task BroadcastAsync(IGameMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                await plr.Session.SendAsync(message)
                    .ConfigureAwait(false);
        }

        public async Task BroadcastAsync(IChatMessage message, bool excludeRooms = false)
        {
            foreach (var plr in Players.Values.Where(plr => !excludeRooms || plr.Room == null))
                await plr.ChatSession.SendAsync(message)
                    .ConfigureAwait(false);
        }
    }
}
