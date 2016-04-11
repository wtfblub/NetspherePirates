using System;
using System.Net;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    internal class RoomCreationOptions
    {
        public string Name { get; set; }
        public MatchKey MatchKey { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public ushort ScoreLimit { get; set; }
        public string Password { get; set; }
        public bool IsFriendly { get; set; }
        public bool IsBalanced { get; set; }
        public byte MinLevel { get; set; }
        public byte MaxLevel { get; set; }
        public byte ItemLimit { get; set; }
        public bool IsNoIntrusion { get; set; }

        public IPEndPoint ServerEndPoint { get; set; }
    }
}
