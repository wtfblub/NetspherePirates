using System;
using Netsphere.Game.Systems;

// ReSharper disable once CheckNamespace
namespace Netsphere
{
    internal class ChannelPlayerJoinedEventArgs : EventArgs
    {
        public Channel Channel { get; }
        public Player Player { get; }

        public ChannelPlayerJoinedEventArgs(Channel channel, Player player)
        {
            Channel = channel;
            Player = player;
        }
    }

    internal class ChannelPlayerLeftEventArgs : EventArgs
    {
        public Channel Channel { get; }
        public Player Player { get; }

        public ChannelPlayerLeftEventArgs(Channel channel, Player player)
        {
            Channel = channel;
            Player = player;
        }
    }

    internal class ChannelMessageEventArgs : EventArgs
    {
        public Channel Channel { get; }
        public Player Player { get; }
        public string Message { get; }

        public ChannelMessageEventArgs(Channel channel, Player player, string message)
        {
            Channel = channel;
            Player = player;
            Message = message;
        }
    }

    internal class RoomPlayerEventArgs : EventArgs
    {
        public Player Player { get; }

        public RoomPlayerEventArgs(Player plr)
        {
            Player = plr;
        }
    }

    internal class TeamChangedEventArgs : EventArgs
    {
        public PlayerTeam From { get; }
        public PlayerTeam To { get; }
        public Player Player { get; }

        public TeamChangedEventArgs(PlayerTeam @from, PlayerTeam to, Player player)
        {
            From = @from;
            To = to;
            Player = player;
        }
    }
}
