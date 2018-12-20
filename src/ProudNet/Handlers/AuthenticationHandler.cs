using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class AuthenticationHandler
        : IHandle<NotifyCSEncryptedSessionKeyMessage>,
          IHandle<NotifyServerConnectionRequestDataMessage>
    {
        private readonly IInternalSessionManager<uint> _sessionManager;
        private readonly NetworkOptions _networkOptions;
        private readonly RSACryptoServiceProvider _rsa;

        public AuthenticationHandler(
            ISessionManagerFactory sessionManagerFactory,
            IOptions<NetworkOptions> networkOptions,
            RSACryptoServiceProvider rsa)
        {
            _sessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.HostId);
            _networkOptions = networkOptions.Value;
            _rsa = rsa;
        }

        [Firewall(typeof(MustBeInState), SessionState.Handshake)]
        public async Task<bool> OnHandle(MessageContext context, NotifyCSEncryptedSessionKeyMessage message)
        {
            var session = context.Session;

            session.Logger.Verbose("Handshake:NotifyCSEncryptedSessionKey");
            var secureKey = _rsa.Decrypt(message.SecureKey, true);
            session.Crypt = new Crypt(secureKey);
            session.State = SessionState.HandshakeKeyExchanged;
            await session.SendAsync(new NotifyCSSessionKeySuccessMessage());
            return true;
        }

        [Firewall(typeof(MustBeInState), SessionState.HandshakeKeyExchanged)]
        public async Task<bool> OnHandle(MessageContext context, NotifyServerConnectionRequestDataMessage message)
        {
            var session = context.Session;

            session.Logger.Verbose("Handshake:NotifyServerConnectionRequestData");
            if (message.InternalNetVersion != Constants.NetVersion ||
                message.Version != _networkOptions.Version)
            {
                // ReSharper disable RedundantAnonymousTypePropertyName
                session.Logger.Warning(
                    "Protocol version mismatch - Client={@ClientVersion} Server={@ServerVersion}",
                    new { NetVersion = message.InternalNetVersion, Version = message.Version },
                    new { NetVersion = Constants.NetVersion, Version = _networkOptions.Version });
                // ReSharper restore RedundantAnonymousTypePropertyName

                await session.SendAsync(new NotifyProtocolVersionMismatchMessage());
                var _ = session.CloseAsync();
            }

            _sessionManager.AddSession(session.HostId, session);
            session.HandhsakeEvent.Set();
            session.State = SessionState.Connected;
            await session.SendAsync(new NotifyServerConnectSuccessMessage(
                session.HostId, _networkOptions.Version, session.RemoteEndPoint));
            return true;
        }
    }
}
