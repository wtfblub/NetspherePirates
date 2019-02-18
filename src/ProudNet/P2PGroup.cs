using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using Logging;
using ProudNet.Configuration;
using ProudNet.Serialization.Messages;

namespace ProudNet
{
    /// <summary>
    /// A group of clients that can communicate with each other
    /// </summary>
    public class P2PGroup : IEnumerable<IP2PMember>
    {
        private readonly ConcurrentDictionary<uint, IP2PMemberInternal> _members =
            new ConcurrentDictionary<uint, IP2PMemberInternal>();
        private readonly ILogger _log;
        private readonly NetworkOptions _options;
        private readonly ISessionManager _sessionManager;

        /// <summary>
        /// Gets the host id for this p2p group
        /// </summary>
        public uint HostId { get; }

        /// <summary>
        /// Gets a value that indicates whether direct p2p is enabled between members
        /// </summary>
        public bool AllowDirectP2P { get; }

        /// <summary>
        /// Gets the number of members in this group
        /// </summary>
        public int Count => _members.Count;

        /// <summary>
        /// Gets a member
        /// Returns null if the member was not found
        /// </summary>
        /// <param name="hostId">The member host id</param>
        public IP2PMember this[uint hostId] => GetMember(hostId);

        internal P2PGroup(ILogger<P2PGroup> logger, uint hostId, NetworkOptions options,
            ISessionManager sessionManager, bool allowDirectP2P)
        {
            _log = logger;
            _options = options;
            _sessionManager = sessionManager;
            HostId = hostId;
            AllowDirectP2P = allowDirectP2P;
        }

        /// <summary>
        /// Gets a member
        /// Returns null if the member was not found
        /// </summary>
        /// <param name="hostId">The member host id</param>
        public IP2PMember GetMember(uint hostId)
        {
            return _members.GetValueOrDefault(hostId);
        }

        /// <summary>
        /// Gets a member
        /// Returns null if the member was not found
        /// </summary>
        /// <param name="hostId">The member host id</param>
        internal IP2PMemberInternal GetMemberInternal(uint hostId)
        {
            return _members.GetValueOrDefault(hostId);
        }

        public void Join(uint hostId)
        {
            var encrypted = _options.EnableP2PEncryptedMessaging;
            Crypt crypt = null;
            if (encrypted)
                crypt = new Crypt(_options.EncryptedMessageKeyLength);

            var sessionToJoin = _sessionManager.GetSession(hostId);
            IP2PMemberInternal memberToJoin = sessionToJoin;
            if (!_members.TryAdd(hostId, sessionToJoin))
                throw new ProudException($"Member {hostId} is already in P2PGroup {HostId}");

            _log.Debug("Client({HostId}) joined P2PGroup({GroupHostId})", hostId, HostId);
            sessionToJoin.P2PGroup = this;
            memberToJoin.Crypt = crypt;

            if (encrypted)
                sessionToJoin.Send(new P2PGroup_MemberJoinMessage(HostId, hostId, 0, crypt.AES.Key, AllowDirectP2P));
            else
                sessionToJoin.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, 0, AllowDirectP2P));

            foreach (var member in _members.Values.Where(member => member.HostId != hostId))
            {
                var stateA = new P2PConnectionState(member);
                var stateB = new P2PConnectionState(sessionToJoin);

                memberToJoin.ConnectionStates[member.HostId] = stateA;
                member.ConnectionStates[memberToJoin.HostId] = stateB;
                if (encrypted)
                {
                    member.Send(new P2PGroup_MemberJoinMessage(HostId, hostId, stateB.EventId, crypt.AES.Key, AllowDirectP2P));
                    sessionToJoin.Send(
                        new P2PGroup_MemberJoinMessage(
                            HostId,
                            member.HostId,
                            stateA.EventId,
                            crypt.AES.Key,
                            AllowDirectP2P
                        )
                    );
                }
                else
                {
                    member.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, hostId, stateB.EventId, AllowDirectP2P));
                    sessionToJoin.Send(
                        new P2PGroup_MemberJoin_UnencryptedMessage(
                            HostId,
                            member.HostId,
                            stateA.EventId,
                            AllowDirectP2P
                        )
                    );
                }
            }

            if (_options.AllowServerAsP2PGroupMember)
                sessionToJoin.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, Constants.HostIdServer, 0, false));

            if (_options.ServerAsP2PGroupMemberHack)
                sessionToJoin.Send(new P2PGroup_MemberJoin_UnencryptedMessage(HostId, Constants.HostIdServerHack, 0, false));
        }

        public void Leave(uint hostId)
        {
            if (!_members.TryRemove(hostId, out var memberToLeave))
                return;

            _log.Debug("Client({HostId}) left P2PGroup({GroupHostId})", hostId, HostId);
            if (memberToLeave is ProudSession session)
            {
                session.P2PGroup = null;
                session.Crypt?.Dispose();
                session.Crypt = null;
            }

            memberToLeave.Send(new P2PGroup_MemberLeaveMessage(hostId, HostId));
            memberToLeave.ConnectionStates.Clear();
            foreach (var member in _members.Values.Where(entry => entry.HostId != hostId))
            {
                member?.Send(new P2PGroup_MemberLeaveMessage(hostId, HostId));
                memberToLeave.Send(new P2PGroup_MemberLeaveMessage(member.HostId, HostId));
                member.ConnectionStates.Remove(hostId);
            }
        }

        public IEnumerator<IP2PMember> GetEnumerator()
        {
            return _members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
