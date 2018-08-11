using System.Collections.Generic;
using BlubLib.Collections.Generic;

namespace ProudNet
{
    internal enum SessionManagerType
    {
        HostId,
        UdpId,
        MagicNumber
    }

    internal interface ISessionManagerFactory
    {
        IInternalSessionManager<TKey> GetSessionManager<TKey>(SessionManagerType sessionManagerType);
    }

    internal class SessionManagerFactory : ISessionManagerFactory
    {
        private readonly IReadOnlyDictionary<SessionManagerType, object> _sessionManagers;

        public SessionManagerFactory(IReadOnlyDictionary<SessionManagerType, object> sessionManagers)
        {
            _sessionManagers = sessionManagers;
        }

        public IInternalSessionManager<TKey> GetSessionManager<TKey>(SessionManagerType sessionManagerType)
        {
            return (IInternalSessionManager<TKey>)_sessionManagers.GetValueOrDefault(sessionManagerType);
        }
    }
}
