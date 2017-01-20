﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProudNet.Serialization.Messages;

namespace ProudNet.Server
{
    internal class P2PGroup : IP2PGroup
    {
        private readonly ConcurrentDictionary<uint, IRemotePeer> _members = new ConcurrentDictionary<uint, IRemotePeer>();
        private readonly ProudServer _server;

        public uint HostId { get; }
        public bool AllowDirectP2P { get; }
        public IReadOnlyDictionary<uint, IRemotePeer> Members => _members;

        public P2PGroup(ProudServer server, bool allowDirectP2P)
        {
            _server = server;
            HostId = _server.Configuration.HostIdFactory.New();
            AllowDirectP2P = allowDirectP2P;
        }

        public void Join(uint hostId)
        {
            var encrypted = _server.Configuration.EnableP2PEncryptedMessaging;
            Crypt crypt = null;
            if (encrypted)
                crypt = new Crypt(_server.Configuration.EncryptedMessageKeyLength);

            var session = _server.Sessions[hostId];
            var remotePeer = new RemotePeer(this, session, crypt);
            if (!_members.TryAdd(hostId, remotePeer))
                throw new ProudException($"Member {hostId} is already in P2PGroup {HostId}");

            session.P2PGroup = this;

            if (encrypted)
                session.SendAsync(new P2PGroup_MemberJoinMessage(HostId, hostId, 0, crypt.RC4.Key, AllowDirectP2P)).WaitEx();
            else
                session.SendAsync(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, 0, AllowDirectP2P)).WaitEx();

            foreach (var member in _members.Values.Where(member => member.HostId != hostId).Cast<RemotePeer>())
            {
                var memberSession = _server.Sessions[member.HostId];

                var stateA = new P2PConnectionState(member);
                var stateB = new P2PConnectionState(remotePeer);

                remotePeer.ConnectionStates[member.HostId] = stateA;
                member.ConnectionStates[remotePeer.HostId] = stateB;
                if (encrypted)
                {
                    memberSession.SendAsync(new P2PGroup_MemberJoinMessage(HostId, hostId, stateB.EventId, crypt.RC4.Key, AllowDirectP2P)).WaitEx();
                    session.SendAsync(new P2PGroup_MemberJoinMessage(HostId, member.HostId, stateA.EventId, member.Crypt.RC4.Key, AllowDirectP2P)).WaitEx();
                }
                else
                {
                    memberSession.SendAsync(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, stateB.EventId, AllowDirectP2P)).WaitEx();
                    session.SendAsync(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, member.HostId, stateA.EventId, AllowDirectP2P)).WaitEx();
                }
            }
        }

        public void Leave(uint hostId)
        {
            IRemotePeer memberToLeave;
            if (!_members.TryRemove(hostId, out memberToLeave))
                return;

            var session = _server.Sessions[hostId];
            session.P2PGroup = null;
            session.SendAsync(new P2PGroup_MemberLeaveMessage(hostId, HostId)).WaitEx();

            foreach (var member in _members.Values.Where(entry => entry.HostId != hostId).Cast<RemotePeer>())
            {
                var memberSession = _server.Sessions[member.HostId];
                memberSession.SendAsync(new P2PGroup_MemberLeaveMessage(hostId, HostId)).WaitEx();
                session.SendAsync(new P2PGroup_MemberLeaveMessage(member.HostId, HostId)).WaitEx();
                member.ConnectionStates.Remove(hostId);
            }
        }
    }
}