using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProudNet.Server
{
    public class P2PGroupManager : IReadOnlyDictionary<uint, IP2PGroup>
    {
        private readonly ConcurrentDictionary<uint, IP2PGroup> _groups = new ConcurrentDictionary<uint, IP2PGroup>();
        private readonly ProudServer _server;

        internal P2PGroupManager(ProudServer server)
        {
            _server = server;
        }

        public IP2PGroup Create(bool allowDirectP2P)
        {
            var group = new P2PGroup(_server, allowDirectP2P);
            _groups.TryAdd(group.HostId, group);
            return group;
        }

        public void Remove(uint groupHostId)
        {
            IP2PGroup group;
            if (_groups.TryRemove(groupHostId, out group))
            {
                foreach (var member in group.Members)
                    group.Leave(member.Key);

                _server.Configuration.HostIdFactory.Free(groupHostId);
            }
        }

        public void Remove(IP2PGroup group) => Remove(group.HostId);

        #region IReadOnlyDictionary

        public int Count => _groups.Count;
        public IEnumerable<uint> Keys => _groups.Keys;
        public IEnumerable<IP2PGroup> Values => _groups.Values;

        public bool ContainsKey(uint key) => _groups.ContainsKey(key);
        public bool TryGetValue(uint key, out IP2PGroup value) => _groups.TryGetValue(key, out value);
        public IP2PGroup this[uint key] => this.GetValueOrDefault(key);
        public IEnumerator<KeyValuePair<uint, IP2PGroup>> GetEnumerator() => _groups.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}