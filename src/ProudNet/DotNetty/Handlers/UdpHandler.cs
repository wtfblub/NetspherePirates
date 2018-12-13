using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.Serialization;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProudNet.DotNetty.Codecs;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.DotNetty.Handlers
{
    internal class UdpHandler : ChannelHandlerAdapter
    {
        private readonly BlubSerializer _serializer;
        private readonly ILogger _log;
        private readonly IInternalSessionManager<Guid> _magicNumberSessionManager;
        private readonly IInternalSessionManager<uint> _udpSessionManager;

        internal UdpSocket Socket { get; set; }

        public UdpHandler(ILogger<UdpHandler> logger, BlubSerializer serializer,
            ISessionManagerFactory sessionManagerFactory)
        {
            _serializer = serializer;
            _log = logger;
            _magicNumberSessionManager = sessionManagerFactory.GetSessionManager<Guid>(SessionManagerType.MagicNumber);
            _udpSessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.UdpId);
        }

        public override void ChannelRead(IChannelHandlerContext context, object obj)
        {
            var message = obj as UdpMessage;
            Debug.Assert(message != null);

            try
            {
                var session = _udpSessionManager.GetSession(message.SessionId);
                if (session == null)
                {
                    if (message.Content.GetByte(0) != (byte)ProudCoreOpCode.ServerHolepunch)
                    {
                        _log.LogWarning("<{EndPoint}> Expected ServerHolepunch as first udp message but got {MessageType}",
                            message.EndPoint.ToString(), (ProudCoreOpCode)message.Content.GetByte(0));
                        return;
                    }

                    var holepunch = (ServerHolepunchMessage)CoreMessageDecoder.Decode(_serializer, message.Content);
                    session = _magicNumberSessionManager.GetSession(holepunch.MagicNumber);
                    if (session == null)
                    {
                        _log.LogWarning("<{EndPoint}> Invalid holepunch magic number={MagicNumber}",
                            message.EndPoint.ToString(), holepunch.MagicNumber);
                        return;
                    }

                    if (session.UdpSocket != Socket)
                    {
                        _log.LogWarning("<{EndPoint}> Client is sending to the wrong udp socket",
                            message.EndPoint.ToString());
                        return;
                    }

                    session.UdpSessionId = message.SessionId;
                    session.UdpEndPoint = message.EndPoint;
                    _udpSessionManager.AddSession(session.UdpSessionId, session);
                    session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
                    return;
                }

                if (session.UdpSocket != Socket)
                {
                    _log.LogWarning("<{EndPoint}> Client is sending to the wrong udp socket",
                        message.EndPoint.ToString());
                    return;
                }

                session.UdpEndPoint = message.EndPoint;
                var recvContext = new MessageContext
                {
                    Session = session,
                    Message = message.Content.Retain(),
                    UdpEndPoint = message.EndPoint
                };

                session.Channel.Pipeline.Context<MessageContextDecoder>().FireChannelRead(recvContext);
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
                CoreMessageEncoder.Encode(_serializer, coreMessage, buffer);

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
