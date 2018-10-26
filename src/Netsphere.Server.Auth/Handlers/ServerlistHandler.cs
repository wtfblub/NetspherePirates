using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Caching;
using Microsoft.Extensions.Logging;
using Netsphere.Common;
using Netsphere.Common.Caching;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Rules;
using ProudNet;

namespace Netsphere.Server.Auth.Handlers
{
    internal class ServerlistHandler : IHandle<CServerListReqMessage>
    {
        private readonly ILogger _logger;
        private readonly ICacheClient _cacheClient;

        public ServerlistHandler(ILogger<ServerlistHandler> logger, ICacheClient cacheClient)
        {
            _logger = logger;
            _cacheClient = cacheClient;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CServerListReqMessage message)
        {
            var session = context.Session;

            var servers = await _cacheClient.GetSetAsync<ServerInfo>(Constants.Cache.ServerlistKey);
            _logger.LogDebug("Cache result HasValue={HasValue} Value={@Value}",
                servers.HasValue, servers.HasValue ? servers.Value : Array.Empty<ServerInfo>());

            if (servers.HasValue)
                await session.SendAsync(new SServerListAckMessage(Map(servers.Value).ToArray()));
            else
                await session.SendAsync(new SServerListAckMessage());

            return true;

            IEnumerable<ServerInfoDto> Map(IEnumerable<ServerInfo> x)
            {
                var groupId = 0;
                foreach (var server in x)
                {
                    ++groupId;
                    yield return new ServerInfoDto
                    {
                        IsEnabled = true,
                        Id = (uint)server.GameId,
                        Type = ServerType.Game,
                        Name = server.Name,
                        PlayerLimit = (ushort)server.Limit,
                        PlayerOnline = (ushort)server.Online,
                        EndPoint = server.GameEndPoint,
                        GroupId = (ushort)groupId
                    };

                    yield return new ServerInfoDto
                    {
                        IsEnabled = true,
                        Id = (uint)server.ChatId,
                        Type = ServerType.Chat,
                        Name = server.Name,
                        PlayerLimit = (ushort)server.Limit,
                        PlayerOnline = (ushort)server.Online,
                        EndPoint = server.ChatEndPoint,
                        GroupId = (ushort)groupId
                    };
                }
            }
        }
    }
}
