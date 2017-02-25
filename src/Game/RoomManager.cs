using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BlubLib.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Game;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using ProudNet;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    internal class RoomManager : IReadOnlyCollection<Room>
    {
        private readonly ConcurrentDictionary<uint, Room> _rooms = new ConcurrentDictionary<uint, Room>();
        private readonly AsyncLock _sync = new AsyncLock();

        public Channel Channel { get; }
        public GameRuleFactory GameRuleFactory { get; }

        #region Events
        

        #endregion

        public RoomManager(Channel channel)
        {
            Channel = channel;
            GameRuleFactory = new GameRuleFactory();
        }

        public void Update(TimeSpan delta)
        {
            foreach (var room in _rooms.Values)
                room.Update(delta);
        }

        public Room Get(uint id)
        {
            Room room;
            _rooms.TryGetValue(id, out room);
            return room;
        }

        public Room Create(RoomCreationOptions options, P2PGroup p2pGroup)
        {
            using (_sync.Lock())
            {
                uint id = 1;
                while (true)
                {
                    if (!_rooms.ContainsKey(id))
                        break;
                    id++;
                }

                var room = new Room(this, id, options, p2pGroup);
                _rooms.TryAdd(id, room);

                Channel.Broadcast(new SDeployGameRoomAckMessage(room.Map<Room, RoomDto>()));

                return room;
            }
        }

        public void Remove(Room room)
        {
            if (room.Players.Count > 0)
                throw new RoomException("Players are still in this room");

            _rooms.Remove(room.Id);
            Channel.Broadcast(new SDisposeGameRoomAckMessage(room.Id));
        }

        #region IReadOnlyCollection

        public int Count => _rooms.Count;

        public Room this[uint id] => Get(id);

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
