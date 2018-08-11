using ProudNet.Serialization;

namespace Netsphere.Network.Message.Relay
{
    public interface IRelayMessage
    {
    }

    public class RelayMessageFactory : MessageFactory<RelayOpCode, IRelayMessage>
    {
        public RelayMessageFactory()
        {
            // S2C
            Register<SEnterLoginPlayerMessage>(RelayOpCode.SEnterLoginPlayer);
            Register<SNotifyLoginResultMessage>(RelayOpCode.SNotifyLoginResult);

            // C2S
            Register<CRequestLoginMessage>(RelayOpCode.CRequestLogin);
            Register<CNotifyP2PLogMessage>(RelayOpCode.CNotifyP2PLog);
        }
    }
}
