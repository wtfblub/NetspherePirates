using System;
using System.Security.Cryptography;
using System.Threading;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class SessionHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _log;
        private readonly NetworkOptions _networkOptions;
        private readonly RSACryptoServiceProvider _rsa;
        private readonly IHostIdFactory _hostIdFactory;
        private readonly ISessionFactory _sessionFactory;
        private readonly IInternalSessionManager<uint> _sessionManager;
        private readonly IServiceProvider _serviceProvider;

        public SessionHandler(ILogger<SessionHandler> logger, IOptions<NetworkOptions> networkOptions,
            RSACryptoServiceProvider rsa, IHostIdFactory hostIdFactory,
            ISessionFactory sessionFactory, ISessionManagerFactory sessionManagerFactory,
            IServiceProvider serviceProvider)
        {
            _log = logger;
            _networkOptions = networkOptions.Value;
            _rsa = rsa;
            _hostIdFactory = hostIdFactory;
            _sessionFactory = sessionFactory;
            _sessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.HostId);
            _serviceProvider = serviceProvider;
        }

        public override async void ChannelActive(IChannelHandlerContext context)
        {
            var hostId = _hostIdFactory.New();
            var session = _sessionFactory.Create(_serviceProvider.GetService<ILogger<ProudSession>>(), hostId, context.Channel);
            context.Channel.GetAttribute(ChannelAttributes.Session).Set(session);

            _log?.LogDebug("New incoming client({HostId}) on {EndPoint}", hostId, context.Channel.RemoteAddress.ToString());

            var config = new NetConfigDto
            {
                EnableServerLog = _networkOptions.EnableServerLog,
                FallbackMethod = _networkOptions.FallbackMethod,
                MessageMaxLength = _networkOptions.MessageMaxLength,
                TimeoutTimeMs = _networkOptions.IdleTimeout.TotalMilliseconds,
                DirectP2PStartCondition = _networkOptions.DirectP2PStartCondition,
                OverSendSuspectingThresholdInBytes = _networkOptions.OverSendSuspectingThresholdInBytes,
                EnableNagleAlgorithm = _networkOptions.EnableNagleAlgorithm,
                EncryptedMessageKeyLength = _networkOptions.EncryptedMessageKeyLength,
                AllowServerAsP2PGroupMember = _networkOptions.AllowServerAsP2PGroupMember,
                EnableP2PEncryptedMessaging = _networkOptions.EnableP2PEncryptedMessaging,
                UpnpDetectNatDevice = _networkOptions.UpnpDetectNatDevice,
                UpnpTcpAddrPortMapping = _networkOptions.UpnpTcpAddrPortMapping,
                EnablePingTest = _networkOptions.EnablePingTest,
                EmergencyLogLineCount = _networkOptions.EmergencyLogLineCount
            };
            await session.SendAsync(new NotifyServerConnectionHintMessage(config, _rsa.ExportParameters(false)));

            using (var cts = new CancellationTokenSource(_networkOptions.ConnectTimeout))
            {
                try
                {
                    await session.HandhsakeEvent.WaitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    if (!session.IsConnected)
                        return;

                    _log.LogDebug("Client({HostId} - {EndPoint}) handshake timeout", hostId, context.Channel.RemoteAddress.ToString());
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
            _log.LogDebug("Client({HostId} - {EndPoint}) disconnected", session.HostId, context.Channel.RemoteAddress.ToString());

            session.Dispose();
            _sessionManager.RemoveSession(session.HostId);
            _hostIdFactory.Free(session.HostId);
            base.ChannelInactive(context);
        }
    }
}
