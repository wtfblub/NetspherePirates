using System.Collections.Concurrent;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet
{
    public interface IP2PMember
    {
        P2PGroup P2PGroup { get; }
        uint HostId { get; }

        void Send(object message);
    }

    internal interface IP2PMemberInternal : IP2PMember
    {
        Crypt Crypt { get; set; }
        ConcurrentDictionary<uint, P2PConnectionState> ConnectionStates { get; }

        void Send(ICoreMessage message, bool udp = false);

        void Send(IMessage message);
    }
}
