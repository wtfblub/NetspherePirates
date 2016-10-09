using System;
using System.Threading.Tasks;
using Auth.ServiceModel;
using BlubLib.Network.SimpleRmi;
using Netsphere.Network;
using Newtonsoft.Json;

namespace Netsphere.API
{
    internal class ServerlistService : RmiService, IServerlistService
    {
        public async Task<RegisterResult> Register(ServerInfoDto serverInfo)
        {
            return AuthServer.Instance.ServerManager.Add(serverInfo)
                    ? RegisterResult.OK
                    : RegisterResult.AlreadyExists;
        }

        public async Task<bool> Update(ServerInfoDto serverInfo)
        {
            ((APISession)CurrentSession).LastActivity = DateTimeOffset.Now;
            return AuthServer.Instance.ServerManager.Update(serverInfo);
        }

        public async Task<bool> Remove(byte id)
        {
            return AuthServer.Instance.ServerManager.Remove(id);
        }
    }
}
