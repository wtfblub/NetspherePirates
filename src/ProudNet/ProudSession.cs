using System;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Threading.Tasks;
using ProudNet.Message;
using ProudNet.Message.Core;

namespace ProudNet
{
    public class ProudSession : Session
    {
        public double UnreliablePing { get; internal set; }
        public uint HostId { get; internal set; }
        public IP2PGroup P2PGroup { get; internal set; }
        public bool UdpEnabled { get; internal set; }
        public IPEndPoint UdpEndPoint { get; internal set; }
        public IPEndPoint UdpLocalEndPoint { get; internal set; }

        internal EncryptContext EncryptContext { get; set; }
        internal DateTime LastSpeedHackDetectorPing { get; set; }
        internal AsyncManualResetEvent ReadyEvent { get; set; }
        internal UdpServerSocket UdpSocket { get; set; }
        internal ushort UdpSessionId { get; set; }

        public ProudSession(IIOService service, IIOProcessor processor)
            : base(service, processor)
        {
            ReadyEvent = new AsyncManualResetEvent();
        }

        public override async Task SendAsync(IMessage message)
        {
            var coreMessage = message as CoreMessage;
            if (coreMessage != null)
            {
                if (UdpEnabled)
                {
                    if (message is UnreliableRelay2Message ||
                        message is PeerUdp_ServerHolepunchAckMessage ||
                        message is UnreliablePongMessage)
                    {

                        await UdpSocket.SendAsync(this, coreMessage)
                            .ConfigureAwait(false);
                        return;
                    }
                }
                var pipe = Service.Pipeline.Get("proudnet_protocol");
                await pipe.OnSendMessage(new MessageEventArgs(this, message))
                    .ConfigureAwait(false);
                return;
            }

            await base.SendAsync(message)
                .ConfigureAwait(false);
        }

        public override void Close()
        {
            Send(new ShutdownTcpAckMessage());

            base.Close();

            if (EncryptContext != null)
            {
                EncryptContext.Dispose();
                EncryptContext = null;
            }

            ReadyEvent.Reset();
        }
    }
}
