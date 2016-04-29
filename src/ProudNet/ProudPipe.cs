using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlubLib.IO;
using BlubLib.Network;
using BlubLib.Network.Message;
using BlubLib.Network.Pipes;
using NLog;
using NLog.Fluent;
using ProudNet.Message;
using ProudNet.Message.Core;
using ProudNet.Services;

namespace ProudNet
{
    public abstract class ProudPipe : Pipe
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IList<IMessageHandler> _coreServices = new List<IMessageHandler>();
        private readonly IList<IMessageHandler> _services = new List<IMessageHandler>();

        public ProudConfig Config { get; }

        #region Events

#if DEBUG
        public event EventHandler<MessageReceivedEventArgs> UnhandledProudCoreMessage;
        public event EventHandler<MessageReceivedEventArgs> UnhandledProudMessage;

        protected virtual void OnUnhandledProudCoreMessage(MessageReceivedEventArgs e)
        {
            UnhandledProudCoreMessage?.Invoke(this, e);
        }

        protected virtual void OnUnhandledProudMessage(MessageReceivedEventArgs e)
        {
            UnhandledProudMessage?.Invoke(this, e);
        }
#endif

        protected virtual void core_UserMessageReceived(MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(e);
        }

        protected virtual void core_ProudMessageReceived(MessageReceivedEventArgs e)
        {
            var handled = false;
            foreach (var service in _services)
            {
                if (service.OnMessageReceived(Service, e))
                    handled = true;
            }

#if DEBUG
            if (!handled)
                OnUnhandledProudMessage(e);
#endif
        }

        #endregion

        protected ProudPipe(ProudConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var service = new ProudCoreService(this);
            service.ProudMessageReceived += core_ProudMessageReceived;
            AddCoreService(service);

            Config = config;
        }
        
        public override void OnDisconnected(SessionEventArgs e)
        {
            Logger.Debug()
                .Message("Client {0} disconnected", ((ProudSession)e.Session).HostId)
                .Write();

            base.OnDisconnected(e);
        }

        public override void OnMessageReceived(MessageReceivedEventArgs e)
        {
            //if (!(e.Message is CoreMessage))
            //    throw new ProudException("Didn't receive a ProudNet core message");

            var handled = false;
            foreach (var service in _coreServices)
            {
                if (service.OnMessageReceived(Service, e))
                    handled = true;
            }

            if (!handled)
                OnUnhandledProudCoreMessage(e);
        }
        
        public override void OnAttached(IService service, string name)
        {
            base.OnAttached(service, name);
            service.Pipeline.AddBefore(name, "proudnet_protocol", new ProtocolPipe(new ProudProtocol()));
        }

        public override void OnDetached()
        {
            Service.Pipeline.Remove("proudnet_protocol");
            base.OnDetached();
        }

        internal void NextOnMessageReceived(MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(e);
        }

        internal Task ProcessAndSendMessage(MessageEventArgs e, EncryptContext context)
        {
            var message = (ProudMessage) e.Message;
            var data = message.ToArray();
            CoreMessage coreMessage = new RmiMessage(data);

            if (message.Compress)
            {
                data = coreMessage.ToArray();
                coreMessage = new CompressedMessage(data.Length, data.CompressZLib());
            }

            if (message.Encrypt)
            {
                data = coreMessage.ToArray();
                using (var w = new PooledMemoryStream(Service.ArrayPool).ToBinaryWriter(false))
                {
                    w.Write(context.EncryptCounter);
                    w.Write(data);

                    data = w.ToArray();
                }

                data = context.Encrypt(data);
                coreMessage = new EncryptedReliableMessage(data);
            }

            e.Message = coreMessage;
            return base.OnSendMessage(e);
        }

        protected void AddCoreService(IMessageHandler service)
        {
            _coreServices.Add(service);
        }

        protected void AddService(IMessageHandler service)
        {
            _services.Add(service);
        }
    }
}
