using System;
using System.Threading.Tasks;
using BlubLib.DotNetty.SimpleRmi;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Netsphere.Network;
using Serilog;
using Serilog.Core;

namespace Netsphere.API
{
    internal class APIServerHandler : ChannelHandlerAdapter
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(APIServerHandler));
        private readonly object _mutex = new object();
        private readonly ILoop _worker;
        private IChannelGroup _channels;

        public APIServerHandler()
        {
            _worker = new TaskLoop(Config.Instance.API.Timeout, Worker);
        }

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _channels = new DefaultChannelGroup(context.Executor);

            var rmi = new SimpleRmiHandler();
            rmi.AddService(new ServerlistService());
            context.Channel.Pipeline.AddBefore(context.Name, null, rmi);
            base.HandlerAdded(context);
        }

        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            var rmi = context.Channel.Pipeline.Context<SimpleRmiHandler>();
            context.Channel.Pipeline.Remove(rmi.Name);
            base.HandlerRemoved(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            lock (_mutex)
            {
                _channels.Add(context.Channel);
                if (_channels.Count == 1)
                    _worker.Start();
            }
            context.Channel.GetAttribute(ChannelAttributes.State).Set(new ChannelState());
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            lock (_mutex)
            {
                _channels.Remove(context.Channel);
                if (_channels.Count == 0)
                    _worker.Stop();
            }
            var state = context.Channel.GetAttribute(ChannelAttributes.State).Get();
            if (state.ServerId != null)
                AuthServer.Instance.ServerManager.Remove(state.ServerId.Value);
            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Error(exception, "Unhandled exception");
        }

        private async Task Worker(TimeSpan diff)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            foreach (var channel in _channels)
            {
                var state = channel.GetAttribute(ChannelAttributes.State).Get();
                var now = DateTimeOffset.Now;
                if (state.ServerId != null &&
                    now - state.ConnectionTime >= Config.Instance.API.Timeout)
                    await channel.CloseAsync();

                if (now - state.LastActivity >= Config.Instance.API.Timeout)
                    await channel.CloseAsync();
            }
        }
    }
}
