using System;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using Netsphere.Network.Message.Auth;
using Netsphere.Network.Service;
using ProudNet;
using ProudNet.Serialization;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network
{
    internal class AuthServer : ProudServer
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(AuthServer));

        public static AuthServer Instance { get; private set; }

        private readonly ILoop _worker;

        public ServerManager ServerManager { get; }

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}");
            config.MessageFactories = new MessageFactory[] { new AuthMessageFactory() };
            config.MessageHandlers = new IMessageHandler[] { new AuthService() };
            config.Logger = Logger.ForContext(Constants.SourceContextPropertyName, "ProudNet");
            Instance = new AuthServer(config);
        }

        private AuthServer(Configuration config)
            : base(config)
        {
            _worker = new TaskLoop(TimeSpan.FromSeconds(10), Worker);
            ServerManager = new ServerManager();
        }

        protected override void OnStarted()
        {
            _worker.Start();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            _worker.Stop();
            base.OnStopping();
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Logger.Error(e.Exception, "Unhandled server exception");
            base.OnError(e);
        }

        private Task Worker(TimeSpan delta)
        {
            ServerManager.Flush();
            return Task.CompletedTask;
        }
    }
}
