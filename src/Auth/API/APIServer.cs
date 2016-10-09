using System;
using System.Buffers;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlubLib.Network;
using BlubLib.Network.SimpleRmi;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using Netsphere.Network;
using NLog;

namespace Netsphere.API
{
    internal class APIServer : TcpServer
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ILoop _worker;

        public APIServer()
            : base(new APISessionFactory(), ArrayPool<byte>.Create(1 * 1024 * 1024, 10), 10)
        {
            Pipeline.AddFirst("rmi", new SimpleRmiPipe())
                .AddService(new ServerlistService());

            _worker = new TaskLoop(Config.Instance.API.Timeout, Worker);
        }

        protected override void OnConnected(SessionEventArgs e)
        {
            ((APISession)e.Session).ConnectionTime = DateTimeOffset.Now;
            base.OnConnected(e);
        }

        protected override void OnDisconnected(SessionEventArgs e)
        {
            var session = (APISession)e.Session;
            if (session.ServerId != null)
                AuthServer.Instance.ServerManager.Remove(session.ServerId.Value);

            base.OnDisconnected(e);
        }

        protected override void OnError(ExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Unhandled server exception");
            base.OnError(e);
        }

        public override void Start(IPAddress address, int port)
        {
            base.Start(address, port);
            _worker.Start();
        }

        public override void Stop()
        {
            _worker.Stop();
            base.Stop();
        }

        private Task Worker(TimeSpan diff)
        {
            foreach (var session in Sessions.Cast<APISession>())
            {
                var now = DateTimeOffset.Now;
                if (session.ServerId != null &&
                    now - session.ConnectionTime >= Config.Instance.API.Timeout)
                    session.Dispose();

                if (now - session.LastActivity >= Config.Instance.API.Timeout)
                    session.Dispose();
            }

            return Task.CompletedTask;
        }
    }

    internal class APISession : Session
    {
        public byte? ServerId { get; set; }
        public DateTimeOffset ConnectionTime { get; set; }
        public DateTimeOffset LastActivity { get; set; }

        public APISession(IService service, ITransport transport)
            : base(service, transport)
        { }
    }

    internal class APISessionFactory : ISessionFactory
    {
        public ISession GetSession(IService service, ITransport transport)
        {
            return new APISession(service, transport);
        }
    }
}
