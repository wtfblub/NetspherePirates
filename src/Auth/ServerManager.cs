using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Netsphere.Network.Data.Auth;
using Serilog;

namespace Netsphere
{
    internal class ServerManager : IEnumerable<ServerInfoDto>
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext<ServerManager>();

        private readonly ConcurrentDictionary<ushort, ServerEntry> _serverList = new ConcurrentDictionary<ushort, ServerEntry>();

        public bool Add(Auth.ServiceModel.ServerInfoDto serverInfo)
        {
            var game = new ServerInfoDto
            {
                IsEnabled = true,
                Id = serverInfo.Id,
                GroupId = serverInfo.Id,
                Type = ServerType.Game,
                Name = serverInfo.Name,
                PlayerLimit = serverInfo.PlayerLimit,
                PlayerOnline = serverInfo.PlayerOnline,
                EndPoint = serverInfo.EndPoint
            };
            var chat = new ServerInfoDto
            {
                IsEnabled = true,
                Id = serverInfo.Id,
                GroupId = serverInfo.Id,
                Type = ServerType.Chat,
                Name = serverInfo.Name,
                PlayerLimit = serverInfo.PlayerLimit,
                PlayerOnline = serverInfo.PlayerOnline,
                EndPoint = serverInfo.ChatEndPoint
            };

            if (_serverList.TryAdd(serverInfo.Id, new ServerEntry(game, chat)))
            {
                Logger.Information($"Added server {serverInfo.Name}({serverInfo.Id})");
                return true;
            }
            return false;
        }

        public bool Update(Auth.ServiceModel.ServerInfoDto serverInfo)
        {
            ServerEntry entry;
            if (!_serverList.TryGetValue(serverInfo.Id, out entry))
                return false;

            entry.Game.PlayerLimit = serverInfo.PlayerLimit;
            entry.Game.PlayerOnline = serverInfo.PlayerOnline;

            entry.Chat.PlayerLimit = serverInfo.PlayerLimit;
            entry.Chat.PlayerOnline = serverInfo.PlayerOnline;

            entry.LastUpdate = DateTimeOffset.Now;

            return true;
        }

        public void Flush()
        {
            foreach (var pair in _serverList)
            {
                var diff = DateTimeOffset.Now - pair.Value.LastUpdate;
                if (diff >= Config.Instance.API.Timeout)
                    Remove(pair.Key);
            }
        }

        public bool Remove(ushort id)
        {
            ServerEntry entry;
            if (_serverList.TryRemove(id, out entry))
            {
                Logger.Information($"Removed server {entry.Game.Name}({entry.Game.GroupId})");
                return true;
            }
            return false;
        }

        public IEnumerator<ServerInfoDto> GetEnumerator()
        {
            return _serverList.Values
                .Select(entry => entry.Game)
                .Concat(_serverList.Values.Select(entry => entry.Chat))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal class ServerEntry
        {
            public ServerInfoDto Game { get; set; }
            public ServerInfoDto Chat { get; set; }
            public DateTimeOffset LastUpdate { get; set; }

            public ServerEntry(ServerInfoDto game, ServerInfoDto chat)
            {
                Game = game;
                Chat = chat;
                LastUpdate = DateTimeOffset.Now;
            }
        }
    }
}
