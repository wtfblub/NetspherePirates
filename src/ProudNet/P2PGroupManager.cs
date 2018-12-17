using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BlubLib.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;

namespace ProudNet
{
    public class P2PGroupManager : IReadOnlyDictionary<uint, P2PGroup>
    {
        private readonly ConcurrentDictionary<uint, P2PGroup> _groups = new ConcurrentDictionary<uint, P2PGroup>();
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly NetworkOptions _options;
        private readonly IHostIdFactory _hostIdFactory;
        private readonly ISessionManager _sessionManager;

        public P2PGroupManager(ILogger<P2PGroupManager> logger, ILoggerFactory loggerFactory,
            IOptions<NetworkOptions> options, IHostIdFactory hostIdFactory, ISessionManager sessionManager)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _options = options.Value;
            _hostIdFactory = hostIdFactory;
            _sessionManager = sessionManager;
        }

        public P2PGroup Create(bool allowDirectP2P)
        {
            var group = new P2PGroup(_loggerFactory.CreateLogger<P2PGroup>(), _hostIdFactory.New(),
                _options, _sessionManager, allowDirectP2P);
            _groups.TryAdd(group.HostId, group);
            _logger.LogDebug("Created P2PGroup({HostId}) DirectP2P={AllowDirectP2P}", group.HostId, allowDirectP2P);
            return group;
        }

        public void Remove(uint groupHostId)
        {
            if (_groups.TryRemove(groupHostId, out var group))
            {
                foreach (var member in group)
                    group.Leave(member.HostId);

                _hostIdFactory.Free(groupHostId);
            }

            _logger.LogDebug("Removed P2PGroup({HostId})", group.HostId);
        }

        public void Remove(P2PGroup group)
        {
            Remove(group.HostId);
        }

        #region IReadOnlyDictionary

        public int Count => _groups.Count;
        public IEnumerable<uint> Keys => _groups.Keys;
        public IEnumerable<P2PGroup> Values => _groups.Values;
        public P2PGroup this[uint key] => this.GetValueOrDefault(key);

        public bool ContainsKey(uint key)
        {
            return _groups.ContainsKey(key);
        }

        public bool TryGetValue(uint key, out P2PGroup value)
        {
            return _groups.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<uint, P2PGroup>> GetEnumerator()
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
