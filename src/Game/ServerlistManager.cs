using System;
using System.Threading.Tasks;
using Auth.ServiceModel;
using BlubLib.DotNetty.SimpleRmi;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ExpressMapper.Extensions;
using Netsphere.Network;
using NLog;

namespace Netsphere
{
    internal class ServerlistManager : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEventLoopGroup _eventLoopGroup;
        private readonly Bootstrap _bootstrap;
        private IChannel _channel;
        private readonly ILoop _worker;
        private bool _userDisconnect;
        private bool _registered;

        public ServerlistManager()
        {
            var handler = new Handler();
            handler.Connected += Client_Connected;
            handler.Disconnected += Client_Disconnected;

            _eventLoopGroup = new MultithreadEventLoopGroup(1);
            _bootstrap = new Bootstrap()
                .Group(_eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Handler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    ch.Pipeline.AddLast(new SimpleRmiHandler())
                    .AddLast(handler);
                }));
            _worker = new TaskLoop(Config.Instance.AuthAPI.UpdateInterval, Worker);
        }

        public void Start()
        {
            _userDisconnect = false;
            _registered = false;
            _worker.Start();
        }

        public void Dispose()
        {
            _worker.Stop();
            _userDisconnect = true;
            try
            {
                if (_channel != null && _channel.Active && _registered)
                    _channel.GetProxy<IServerlistService>().Remove((byte)Config.Instance.Id);
            }
            catch
            {
                // ignored
            }

            _channel.CloseAsync().WaitEx();
            _eventLoopGroup.ShutdownGracefullyAsync().WaitEx();
        }

        private async Task Worker(TimeSpan diff)
        {
            try
            {
                if (_channel == null || !_channel.Active)
                {
                    if (!await Connect().ConfigureAwait(false))
                        return;
                }

                if (!_registered)
                {
                    await Register().ConfigureAwait(false);
                    return;
                }

                var result = await _channel.GetProxy<IServerlistService>()
                    .Update(GameServer.Instance.Map<GameServer, ServerInfoDto>())
                    .ConfigureAwait(false);

                if (!result)
                    await Register().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled exception");
            }
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            Logger.Info($"Connected to authserver on endpoint {Config.Instance.AuthAPI.EndPoint}");
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            _registered = false;
            if (_userDisconnect)
                return;

            Logger.Warn("Lost connection to authserver. Trying to reconnect on next update.");
        }

        private async Task<bool> Connect()
        {
            var endPoint = Config.Instance.AuthAPI.EndPoint;
            try
            {
                _channel = await _bootstrap.ConnectAsync(endPoint)
                    .ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                var baseException = ex.GetBaseException();
                if (baseException is ConnectException)
                {
                    Logger.Error($"Failed to connect authserver on endpoint {endPoint}. Retrying on next update.");
                    return false;
                }
                Logger.Error(baseException, $"Failed to connect authserver on endpoint {endPoint}. Retrying on next update.");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to connect authserver on endpoint {endPoint}. Retrying on next update.");
                return false;
            }

            return true;
        }

        private async Task<bool> Register()
        {
            var result = await _channel.GetProxy<IServerlistService>()
                   .Register(GameServer.Instance.Map<GameServer, ServerInfoDto>())
                   .ConfigureAwait(false);

            switch (result)
            {
                case RegisterResult.OK:
                    _registered = true;
                    return true;

                case RegisterResult.AlreadyExists:
                    Logger.Warn($"Unable to register server - Id:{Config.Instance.Id} is already registered(Invalid config?).");
                    break;
            }
            return false;
        }

        private class Handler : ChannelHandlerAdapter
        {
            public event EventHandler Connected;
            public event EventHandler Disconnected;

            public override bool IsSharable => true;

            public override void ChannelActive(IChannelHandlerContext context)
            {
                Connected?.Invoke(this, EventArgs.Empty);
                base.ChannelActive(context);
            }

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                base.ChannelInactive(context);
            }
        }
    }
}