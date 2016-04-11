using BlubLib.Network;
using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.P2P;
using NLog;

namespace Netsphere.Network.Services
{
    internal class RelayService : Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IService _peerService = new PeerService();

        [MessageHandler(typeof(PacketMessage))]
        public void Packet(IIOService service, RelaySession session, PacketMessage message, MessageReceivedEventArgs e)
        {
            if (session.Player?.Room == null)
                return;

            var peerMessages = P2PMapper.GetMessage(message.IsCompressed ? message.Data.DecompressLZO(2048) : message.Data);
            foreach (var peerMessage in peerMessages)
            {
                if (!_peerService.OnMessageReceived(service, new MessageReceivedEventArgs(session, peerMessage, e.DeferralManager)))
                {
                    //_logger.Debug()
                    //    .Account(session)
                    //    .Message("Unhandled PeerMessage: {0}", peerMessage.GetType().Name)
                    //    .Write();
                }
            }
        }
    }
}
