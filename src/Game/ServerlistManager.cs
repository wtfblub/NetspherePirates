using System;
using System.Buffers;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Threading.Tasks;
using Auth.ServiceModel;
using BlubLib.Network;
using BlubLib.Network.SimpleRmi;
using BlubLib.Network.Transport.Sockets;
using BlubLib.Threading;
using BlubLib.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Network;
using NLog;
using TcpClient = BlubLib.Network.Transport.Sockets.TcpClient;

namespace Netsphere
{
    internal class ServerlistManager : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISocketClient _client;
        private readonly ILoop _worker;
        private bool _userDisconnect;
        private bool _registered;

        public ServerlistManager()
        {
            _client = new TcpClient(new SessionFactory(), ArrayPool<byte>.Shared);
            _client.Pipeline.AddFirst("rmi", new SimpleRmiPipe());
            _worker = new TaskLoop(Config.Instance.AuthAPI.UpdateInterval, Worker);

            _client.Connected += Client_Connected;
            _client.Disconnected += Client_Disconnected;
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
                if (_client.IsConnected && _registered)
                    _client.GetProxy<IServerlistService>("rmi").Remove((byte) Config.Instance.Id);
            }
            catch
            {
                // ignored
            }

            _client.Dispose();
        }

        private async Task Worker(TimeSpan diff)
        {
            if (!_client.IsConnected)
            {
                if (!await Connect().ConfigureAwait(false))
                    return;
            }

            if (!_registered)
            {
                await Register().ConfigureAwait(false);
                return;
            }

            var result = await _client.GetProxy<IServerlistService>("rmi")
                .Update(GameServer.Instance.Map<GameServer, ServerInfoDto>())
                .ConfigureAwait(false);

            if (!result)
                await Register().ConfigureAwait(false);
        }

        private void Client_Connected(object sender, SessionEventArgs e)
        {
            Logger.Info($"Connected to authserver on endpoint {Config.Instance.AuthAPI.EndPoint}");
        }

        private void Client_Disconnected(object sender, SessionEventArgs e)
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
                await _client.ConnectAsync(endPoint)
                    .ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                if(ex.SocketErrorCode != SocketError.ConnectionRefused)
                    Logger.Error(ex, $"Failed to connect authserver on endpoint {endPoint}. Retrying on next update.");
                else
                    Logger.Error($"Failed to connect authserver on endpoint {endPoint}. Retrying on next update.");
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
            var result = await _client.GetProxy<IServerlistService>("rmi")
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
    }
}