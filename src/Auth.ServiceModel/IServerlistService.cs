﻿using System.Threading.Tasks;
using BlubLib.DotNetty.SimpleRmi;

namespace Auth.ServiceModel
{
    [RmiContract]
    public interface IServerlistService
    {
        [Rmi]
        Task<RegisterResult> Register(ServerInfoDto serverInfo);

        [Rmi]
        Task<bool> Update(ServerInfoDto serverInfo);

        [Rmi]
        Task<bool> Remove(byte id);
    }
}
