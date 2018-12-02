using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Netsphere.Network.Message.Relay;
using ProudNet;

namespace Netsphere.Server.Relay
{
    public class Room
    {
        private readonly RoomManager _roomManager;
        private readonly ConcurrentDictionary<ulong, Player> _players;

        public uint Id { get; }
        public P2PGroup Group { get; }
        public IReadOnlyDictionary<ulong, Player> Players => _players;

        public Room(RoomManager roomManager, uint id, P2PGroup group)
        {
            _roomManager = roomManager;
            Id = id;
            Group = group;
            _players = new ConcurrentDictionary<ulong, Player>();
        }

        public async Task Join(Player plr)
        {
            _players.TryAdd(plr.Account.Id, plr);

            await plr.Session.SendAsync(new SEnterLoginPlayerMessage(plr.Session.HostId, plr.Account.Id, plr.Account.Nickname));
            foreach (var otherPlayer in _players.Values.Where(x => x.Account.Id != plr.Account.Id))
            {
                await otherPlayer.Session.SendAsync(new SEnterLoginPlayerMessage(
                    plr.Session.HostId, plr.Account.Id, plr.Account.Nickname));

                await plr.Session.SendAsync(new SEnterLoginPlayerMessage(
                    otherPlayer.Session.HostId, otherPlayer.Account.Id, otherPlayer.Account.Nickname));
            }

            Group.Join(plr.Session.HostId);

            plr.Disconnected += Player_OnDisconnected;
        }

        public void Leave(Player plr)
        {
            if (!_players.TryRemove(plr.Account.Id, out _))
                return;

            plr.Disconnected -= Player_OnDisconnected;
            Group.Leave(plr.Session.HostId);
            plr.Disconnect();

            if (_players.Count == 0)
                _roomManager.Remove(this);
        }

        private void Player_OnDisconnected(object sender, PlayerEventArgs e)
        {
            Leave(e.Player);
        }
    }
}
