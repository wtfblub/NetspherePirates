using System;
using System.Threading;
using DotNetty.Transport.Channels;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class SessionHandler : ChannelHandlerAdapter
    {
        private readonly ProudServer _server;

        public SessionHandler(ProudServer server)
        {
            _server = server;
        }

        public override async void ChannelActive(IChannelHandlerContext context)
        {
            var hostId = _server.Configuration.HostIdFactory.New();
            var session = _server.Configuration.SessionFactory.Create(hostId, context.Channel, _server);
            context.Channel.GetAttribute(ChannelAttributes.Session).Set(session);

            var log = _server.Configuration.Logger?
                .ForContext("HostId", hostId)
                .ForContext("EndPoint", context.Channel.RemoteAddress.ToString());
            log?.Debug("New incoming client({HostId}) on {EndPoint}");

            var config = new NetConfigDto
            {
                EnableServerLog = _server.Configuration.EnableServerLog,
                FallbackMethod = _server.Configuration.FallbackMethod,
                MessageMaxLength = _server.Configuration.MessageMaxLength,
                TimeoutTimeMs = _server.Configuration.IdleTimeout.TotalMilliseconds,
                DirectP2PStartCondition = _server.Configuration.DirectP2PStartCondition,
                OverSendSuspectingThresholdInBytes = _server.Configuration.OverSendSuspectingThresholdInBytes,
                EnableNagleAlgorithm = _server.Configuration.EnableNagleAlgorithm,
                EncryptedMessageKeyLength = _server.Configuration.EncryptedMessageKeyLength,
                AllowServerAsP2PGroupMember = _server.Configuration.AllowServerAsP2PGroupMember,
                EnableP2PEncryptedMessaging = _server.Configuration.EnableP2PEncryptedMessaging,
                UpnpDetectNatDevice = _server.Configuration.UpnpDetectNatDevice,
                UpnpTcpAddrPortMapping = _server.Configuration.UpnpTcpAddrPortMapping,
                EnablePingTest = _server.Configuration.EnablePingTest,
                EmergencyLogLineCount = _server.Configuration.EmergencyLogLineCount
            };
            await session.SendAsync(new NotifyServerConnectionHintMessage(config, _server.Rsa.ExportParameters(false)));

            using (var cts = new CancellationTokenSource(_server.Configuration.ConnectTimeout))
            {
                try
                {
                    await session.HandhsakeEvent.WaitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    if (!session.IsConnected)
                        return;

                    log?.Debug("Client({HostId}) handshake timeout");
                    await session.SendAsync(new ConnectServerTimedoutMessage());
                    await session.CloseAsync();
                    return;
                }
            }
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
            _server.Configuration.Logger?
                .ForContext("HostId", session.HostId)
                .ForContext("EndPoint", context.Channel.RemoteAddress.ToString())
                .Debug("Client({HostId}) disconnected");

            session.Dispose();
            _server.RemoveSession(session);
            _server.Configuration.HostIdFactory.Free(session.HostId);
            base.ChannelInactive(context);
        }
    }
}
