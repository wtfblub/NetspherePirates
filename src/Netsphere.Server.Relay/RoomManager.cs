using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BlubLib.Collections.Concurrent;
using Logging;
using ProudNet;

namespace Netsphere.Server.Relay
{
    public class RoomManager : IReadOnlyCollection<Room>
    {
        private readonly ILogger _logger;
        private readonly P2PGroupManager _groupManager;
        private readonly ConcurrentDictionary<uint, Room> _rooms;

        public int Count => _rooms.Count;
        public Room this[uint id] => _rooms.GetValueOrDefault(id);

        public static event EventHandler<RoomEventArgs> RoomCreated;
        public static event EventHandler<RoomEventArgs> RoomRemoved;

        private static void OnRoomCreated(RoomManager roomManager, Room room)
        {
            RoomCreated?.Invoke(roomManager, new RoomEventArgs(room));
        }

        private static void OnRoomRemoved(RoomManager roomManager, Room room)
        {
            RoomRemoved?.Invoke(roomManager, new RoomEventArgs(room));
        }

        public RoomManager(ILogger<RoomManager> logger, P2PGroupManager groupManager)
        {
            _logger = logger;
            _groupManager = groupManager;
            _rooms = new ConcurrentDictionary<uint, Room>();
        }

        public Room GetOrCreate(uint id)
        {
            var room = _rooms.GetValueOrDefault(id);
            if (room == null)
            {
                _logger.Information("Creating p2pgroup for room={RoomId}...", id);
                var group = _groupManager.Create(true);
                room = new Room(this, id, group);
                _rooms[id] = room;
                OnRoomCreated(this, room);
            }

            return room;
        }

        internal bool Remove(Room room)
        {
            _logger.Information("Removing p2pgroup for room={RoomId}...", room.Id);
            _groupManager.Remove(room.Group);
            if (_rooms.Remove(room.Id))
            {
                OnRoomRemoved(this, room);
                return true;
            }

            return false;
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
