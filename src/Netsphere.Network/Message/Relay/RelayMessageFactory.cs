using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization;

namespace Netsphere.Network.Message.Relay
{
    public interface IRelayMessage
    { }

    public class RelayMessageFactory : MessageFactory<RelayOpCode, IRelayMessage>
    {
        static RelayMessageFactory()
        {
            Serializer.AddCompiler(new PeerIdSerializer());
        }

        public RelayMessageFactory()
        {
            // S2C
            Register<SEnterLoginPlayerMessage>(RelayOpCode.SEnterLoginPlayer);
            Register<SNotifyLoginResultMessage>(RelayOpCode.SNotifyLoginResult);

            // C2S
            Register<CRequestLoginMessage>(RelayOpCode.CRequestLogin);
        }
    }
}