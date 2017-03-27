using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ProudNet.Codecs;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class UdpHandler : ChannelHandlerAdapter
    {
        private readonly UdpSocket _socket;
        private readonly ProudServer _server;

        public UdpHandler(UdpSocket socket, ProudServer server)
        {
            _socket = socket;
            _server = server;
        }

        public override void ChannelRead(IChannelHandlerContext context, object obj)
        {
            var message = obj as UdpMessage;
            Debug.Assert(message != null);

            try
            {
                var session = _server.SessionsByUdpId.GetValueOrDefault(message.SessionId);
                if (session == null)
                {
                    if (message.Content.GetByte(0) != (byte)ProudCoreOpCode.ServerHolepunch)
                        throw new ProudException(
                            $"Expected {ProudCoreOpCode.ServerHolepunch} as first udp message but got {(ProudCoreOpCode)message.Content.GetByte(0)}");

                    var holepunch = (ServerHolepunchMessage)CoreMessageDecoder.Decode(message.Content);

                    session = _server.Sessions.Values.FirstOrDefault(x =>
                        x.HolepunchMagicNumber.Equals(holepunch.MagicNumber));
                    if (session == null)
                        return;

                    if (session.UdpSocket != _socket)
                        return;

                    session.UdpSessionId = message.SessionId;
                    session.UdpEndPoint = message.EndPoint;
                    _server.SessionsByUdpId[session.UdpSessionId] = session;

                    session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
                    return;
                }

                if (session.UdpSocket != _socket)
                    return;

                session.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(message.Content.Retain());
            }
            finally
            {
                message.Content.Release();
            }
        }

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            var sendContext = message as SendContext;
            Debug.Assert(sendContext != null);
            var coreMessage = sendContext.Message as ICoreMessage;
            Debug.Assert(coreMessage != null);

            var buffer = context.Allocator.Buffer();
            try
            {
                CoreMessageEncoder.Encode(coreMessage, buffer);

                return base.WriteAsync(context, new UdpMessage
                {
                    Flag = 43981,
                    Content = buffer,
                    EndPoint = sendContext.UdpEndPoint
                });
            }
            catch (Exception ex)
            {
                buffer.Release();
                ex.Rethrow();
                throw;
            }
        }
    }
}
