using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BlubLib.Collections.Concurrent;
using Microsoft.Extensions.Logging;
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
                _logger.LogInformation("Creating p2pgroup for room={RoomId}...", id);
                var group = _groupManager.Create(true);
                return new Room(this, id, group);
            }

            return room;
        }

        internal bool Remove(Room room)
        {
            _logger.LogInformation("Removing p2pgroup for room={RoomId}...", room.Id);
            _groupManager.Remove(room.Group);
            return _rooms.Remove(room.Id);
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
