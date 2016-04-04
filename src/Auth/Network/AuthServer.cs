using System;
using System.Buffers;
using Auth.Network.Service;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using Netsphere.Network;
using Netsphere.Network.Message;
using NLog;
using NLog.Fluent;
using ProudNet;
using ProudNet.Message;

namespace Auth.Network
{
    internal class AuthServer : TcpServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthServer()
            : base(new ProudSessionFactory(), ArrayPool<byte>.Create(1 * 1024 * 1024, Config.Instance.MaxConnections), Config.Instance.MaxConnections)
        {
            var config = new ProudConfig(new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}"));
            var proudFilter = new ProudServerPipe(config);
#if DEBUG
            proudFilter.UnhandledProudCoreMessage += OnUnhandledProudCoreMessage;
            proudFilter.UnhandledProudMessage += OnUnhandledProudMessage;
#endif
            Pipeline.AddFirst("proudnet", proudFilter);
            Pipeline.AddLast("s4_protocol", new NetspherePipe(new AuthMessageFactory()));
            Pipeline.AddLast("s4_service", new ServicePipe())
                .Add(new AuthService())
                .UnhandledMessage += OnUnhandledMessage;
        }

#if DEBUG
        private void OnUnhandledProudCoreMessage(object s, MessageReceivedEventArgs e)
        {
            _logger.Warn()
                .Message("Unhandled ProudCoreMessage {0}", e.Message.GetType().Name)
                .Write();
        }

        private void OnUnhandledProudMessage(object s, MessageReceivedEventArgs e)
        {
            var message = e.Message as ProudUnknownMessage;
            if (message == null)
            {
                _logger.Warn()
                    .Message("Unhandled ProudMessage {0}", e.Message.GetType().Name)
                    .Write();
            }
            else
            {
                _logger.Warn()
                    .Message("Unknown ProudMessage {0}: {1}", message.OpCode, message.Data.ToHexString())
                    .Write();
            }
        }
#endif

        private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        {
            _logger.Warn()
                .Message("Unhandled message {0}", e.Message.GetType().Name)
                .Write();
        }

        protected override void OnError(ExceptionEventArgs ex)
        {
            _logger.Error()
                .Exception(ex.Exception)
                .Write();
            base.OnError(ex);
        }
    }
}
