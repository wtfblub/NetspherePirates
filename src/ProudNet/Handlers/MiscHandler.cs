using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Logging;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class MiscHandler
        : IHandle<UnreliablePingMessage>,
          IHandle<SpeedHackDetectorPingMessage>,
          IHandle<ReliablePingMessage>,
          IHandle<ShutdownTcpMessage>,
          IHandle<NotifyLogMessage>,
          IHandle<NotifyNatDeviceNameDetectedMessage>,
          IHandle<ReportC2SUdpMessageTrialCountMessage>
    {
        private readonly Lazy<DateTime> _processStartTime = new Lazy<DateTime>(() => Process.GetCurrentProcess().StartTime);
        private readonly ILogger _logger;

        public MiscHandler(ILogger<MiscHandler> logger)
        {
            _logger = logger;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, UnreliablePingMessage message)
        {
            var session = context.Session;

            session.UnreliablePing = TimeSpan.FromSeconds(message.Ping).TotalMilliseconds;
            if (context.UdpEndPoint != null)
                session.LastUdpPing = DateTimeOffset.Now;

            var ts = DateTime.Now - _processStartTime.Value;
            session.Send(new UnreliablePongMessage(message.ClientTime, ts.TotalSeconds), true);
            return true;
        }

        public Task<bool> OnHandle(MessageContext context, SpeedHackDetectorPingMessage message)
        {
            context.Session.LastSpeedHackDetectorPing = DateTime.Now;
            return Task.FromResult(true);
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ReliablePingMessage message)
        {
            context.Session.Send(new ReliablePongMessage());
            return true;
        }

        [Inline]
        public Task<bool> OnHandle(MessageContext context, ShutdownTcpMessage message)
        {
            var _ = context.Session.CloseAsync();
            return Task.FromResult(true);
        }

        [Inline]
        public Task<bool> OnHandle(MessageContext context, NotifyLogMessage message)
        {
            _logger.Debug("NotifyLog {@Message}", message);
            return Task.FromResult(true);
        }

        [Inline]
        public Task<bool> OnHandle(MessageContext context, NotifyNatDeviceNameDetectedMessage message)
        {
            return Task.FromResult(true);
        }

        [Inline]
        public Task<bool> OnHandle(MessageContext context, ReportC2SUdpMessageTrialCountMessage message)
        {
            return Task.FromResult(true);
        }
    }
}
