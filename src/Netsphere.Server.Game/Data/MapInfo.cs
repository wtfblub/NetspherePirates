using System.Collections.Generic;
using BlubLib.Configuration;

namespace Netsphere.Server.Game.Data
{
    public class MapInfo
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte MinLevel { get; set; }
        public uint ServerId { get; set; }
        public uint ChannelId { get; set; }
        public byte RespawnType { get; set; }
        public IniFile Config { get; set; }

        public IList<GameRule> GameRules { get; set; }

        public MapInfo()
        {
            GameRules = new List<GameRule>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
