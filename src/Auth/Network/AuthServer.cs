using System;
using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using Netsphere.Network.Message;
using Netsphere.Network.Service;
using NLog;
using NLog.Fluent;
using ProudNet;
using ProudNet.Message;

namespace Netsphere.Network
{
    internal class AuthServer : TcpServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static AuthServer Instance { get; } = new AuthServer();

        private readonly ILoop _worker;

        public ServerManager ServerManager { get; }

        private AuthServer()
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

            _worker = new TaskLoop(TimeSpan.FromSeconds(10), Worker);
            ServerManager = new ServerManager();
        }

#if DEBUG
        private void OnUnhandledProudCoreMessage(object s, MessageReceivedEventArgs e)
        {
            Logger.Warn()
                .Message("Unhandled ProudCoreMessage {0}", e.Message.GetType().Name)
                .Write();
        }

        private void OnUnhandledProudMessage(object s, MessageReceivedEventArgs e)
        {
            var message = e.Message as ProudUnknownMessage;
            if (message == null)
            {
                Logger.Warn()
                    .Message("Unhandled ProudMessage {0}", e.Message.GetType().Name)
                    .Write();
            }
            else
            {
                Logger.Warn()
                    .Message("Unknown ProudMessage {0}: {1}", message.OpCode, message.Data.ToHexString())
                    .Write();
            }
        }
#endif

        private void OnUnhandledMessage(object sender, MessageReceivedEventArgs e)
        {
            Logger.Warn()
                .Message("Unhandled message {0}", e.Message.GetType().Name)
                .Write();
        }

        protected override void OnError(ExceptionEventArgs ex)
        {
            Logger.Error()
                .Exception(ex.Exception)
                .Write();
            base.OnError(ex);
        }

        public override void Start(IPEndPoint localEP)
        {
            _worker.Start();
            base.Start(localEP);
        }

        public override void Dispose()
        {
            _worker.Stop();
            base.Dispose();
        }

        private Task Worker(TimeSpan delta)
        {
            ServerManager.Flush();
            return Task.CompletedTask;
        }
    }
}
