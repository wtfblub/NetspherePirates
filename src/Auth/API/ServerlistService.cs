using System;
using System.Threading.Tasks;
using Auth.ServiceModel;
using BlubLib.DotNetty.SimpleRmi;
using Netsphere.Network;

namespace Netsphere.API
{
    internal class ServerlistService : RmiService, IServerlistService
    {
        public async Task<RegisterResult> Register(ServerInfoDto serverInfo)
        {
            UpdateLastActivity();
            return AuthServer.Instance.ServerManager.Add(serverInfo)
                    ? RegisterResult.OK
                    : RegisterResult.AlreadyExists;
        }

        public async Task<bool> Update(ServerInfoDto serverInfo)
        {
            UpdateLastActivity();
            return AuthServer.Instance.ServerManager.Update(serverInfo);
        }

        public async Task<bool> Remove(byte id)
        {
            return AuthServer.Instance.ServerManager.Remove(id);
        }

        private void UpdateLastActivity()
        {
            var state = CurrentContext.Channel.GetAttribute(ChannelAttributes.State).Get();
            state.LastActivity = DateTimeOffset.Now;
        }
    }
}
