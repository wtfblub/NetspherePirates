using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Network.Message;
using ProudNet.Message;

namespace ProudNet
{
    public interface IP2PGroup
    {
        ProudPipe Filter { get; }
        uint HostId { get; }
        IReadOnlyDictionary<uint, IRemotePeer> Members { get; }
    }

    public interface IRemotePeer
    {
        IP2PGroup Group { get; }
        uint HostId { get; }
        bool DirectP2PAllowed { get; }

        void Send(IMessage message);
        Task SendAsync(IMessage message);
    }

    public class ServerP2PGroup : IP2PGroup
    {
        private readonly ConcurrentDictionary<uint, IRemotePeer> _members = new ConcurrentDictionary<uint, IRemotePeer>();
        private readonly ProudServerPipe _filter;

        public ProudPipe Filter => _filter;
        public uint HostId { get; }
        public IReadOnlyDictionary<uint, IRemotePeer> Members => _members;

        public ServerP2PGroup(ProudServerPipe filter, uint hostId)
        {
            _filter = filter;
            HostId = hostId;
        }

        public void Join(uint hostId, bool directP2P)
        {
            var encrypted = Filter.Config.EnableP2PEncryptedMessaging;
            EncryptContext encryptContext = null;
            if (encrypted)
                encryptContext = new EncryptContext(Filter.Config.EncryptedMessageKeyLength);

            var remotePeer = new ServerRemotePeer(this, hostId, directP2P, encryptContext);
            if (!_members.TryAdd(hostId, remotePeer))
                throw new ProudException($"Member {hostId} is already in P2PGroup {HostId}");

            var session = _filter.SessionLookupByHostId.GetValueOrDefault(hostId);
            if (session != null)
            {
                session.P2PGroup = this;

                if (encrypted)
                    session.Send(new P2PGroup_MemberJoinMessage(HostId, hostId, 0, encryptContext.RC4.Key, directP2P));
                else
                    session.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, 0, directP2P));
            }

            foreach (var member in _members.Values.Where(member => member.HostId != hostId).Cast<ServerRemotePeer>())
            {
                var memberSession = _filter.SessionLookupByHostId.GetValueOrDefault(member.HostId);

                var stateA = new P2PConnectionState(member);
                var stateB = new P2PConnectionState(remotePeer);

                remotePeer.ConnectionStates[member.HostId] = stateA;
                member.ConnectionStates[remotePeer.HostId] = stateB;
                if (encrypted)
                {
                    memberSession?.Send(new P2PGroup_MemberJoinMessage(HostId, hostId, stateB.EventId, encryptContext.RC4.Key, directP2P));
                    session?.Send(new P2PGroup_MemberJoinMessage(HostId, member.HostId, stateA.EventId, member.EncryptContext.RC4.Key, directP2P));
                }
                else
                {
                    memberSession?.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, stateB.EventId, directP2P));
                    session?.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, member.HostId, stateA.EventId, directP2P));
                }
            }
        }

        public void Leave(uint hostId)
        {
            IRemotePeer memberToLeave;
            if (!_members.TryRemove(hostId, out memberToLeave))
                return;

            var session = _filter.SessionLookupByHostId.GetValueOrDefault(hostId);
            if (session != null)
            {
                session.P2PGroup = null;
                session.Send(new P2PGroup_MemberLeaveMessage(hostId, HostId));
            }

            foreach (var member in _members.Values.Where(entry => entry.HostId != hostId).Cast<ServerRemotePeer>())
            {
                var memberSession = _filter.SessionLookupByHostId.GetValueOrDefault(member.HostId);
                memberSession?.Send(new P2PGroup_MemberLeaveMessage(hostId, HostId));
                session?.Send(new P2PGroup_MemberLeaveMessage(member.HostId, HostId));

                member.ConnectionStates.Remove(hostId);
            }
        }
    }

    internal class ServerRemotePeer : IRemotePeer
    {
        public IP2PGroup Group { get; }
        public uint HostId { get; }
        public bool DirectP2PAllowed { get; }
        internal EncryptContext EncryptContext { get; }
        internal ConcurrentDictionary<uint, P2PConnectionState> ConnectionStates { get; }

        public ServerRemotePeer(IP2PGroup @group, uint hostId, bool directP2PAllowed, EncryptContext encryptContext)
        {
            Group = @group;
            HostId = hostId;
            DirectP2PAllowed = directP2PAllowed;
            EncryptContext = encryptContext;
            ConnectionStates = new ConcurrentDictionary<uint, P2PConnectionState>();
        }

        public void Send(IMessage message)
        {
            var filter = (ProudServerPipe) Group.Filter;
            var session = filter.SessionLookupByHostId.GetValueOrDefault(HostId);
            session?.Send(message);
        }

        public Task SendAsync(IMessage message)
        {
            var filter = (ProudServerPipe)Group.Filter;
            var session = filter.SessionLookupByHostId.GetValueOrDefault(HostId);
            return session?.SendAsync(message) ?? Task.CompletedTask;
        }
    }

    internal class P2PConnectionState
    {
        public ServerRemotePeer RemotePeer { get; }
        public uint EventId { get; }
        public bool IsJoined { get; set; }
        public bool JitTriggered { get; set; }
        public bool HolepunchSuccess { get; set; }

        public P2PConnectionState(ServerRemotePeer remotePeer)
        {
            RemotePeer = remotePeer;
            EventId = (uint)Guid.NewGuid().GetHashCode();
        }
    }

    public class ServerP2PGroupManager : IReadOnlyDictionary<uint, ServerP2PGroup>
    {
        private readonly ConcurrentDictionary<uint, ServerP2PGroup> _groups = new ConcurrentDictionary<uint, ServerP2PGroup>();
        private readonly ProudServerPipe _filter;

        internal ServerP2PGroupManager(ProudServerPipe filter)
        {
            _filter = filter;
        }

        public ServerP2PGroup Create()
        {
            var group = new ServerP2PGroup(_filter, _filter.GetNextHostId());
            _groups.TryAdd(group.HostId, group);
            return group;
        }

        public async Task<ServerP2PGroup> CreateAsync()
        {
            var group = new ServerP2PGroup(_filter, await _filter.GetNextHostIdAsync().ConfigureAwait(false));
            _groups.TryAdd(group.HostId, group);
            return group;
        }

        public void Remove(uint groupHostId)
        {
            ServerP2PGroup group;

            if (_groups.TryRemove(groupHostId, out group))
            {
                foreach (var member in group.Members)
                    group.Leave(member.Key);

                _filter.FreeHostId(group.HostId);
            }
        }

        public void Remove(ServerP2PGroup group)
        {
            if (_groups.Remove(group.HostId))
            {
                foreach (var member in group.Members)
                    group.Leave(member.Key);

                _filter.FreeHostId(group.HostId);
            }
        }

        #region IReadOnlyDictionary

        public int Count => _groups.Count;

        public IEnumerable<uint> Keys => _groups.Keys;

        public IEnumerable<ServerP2PGroup> Values => _groups.Values;

        public bool ContainsKey(uint key)
        {
            return _groups.ContainsKey(key);
        }

        public bool TryGetValue(uint key, out ServerP2PGroup value)
        {
            return _groups.TryGetValue(key, out value);
        }

        public ServerP2PGroup this[uint key]
        {
            get
            {
                ServerP2PGroup group;
                TryGetValue(key, out group);
                return group;
            }
        }

        public IEnumerator<KeyValuePair<uint, ServerP2PGroup>> GetEnumerator()
        {
            return _groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
