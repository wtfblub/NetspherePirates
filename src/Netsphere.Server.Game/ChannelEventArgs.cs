namespace Netsphere.Server.Game
{
    public class ChannelEventArgs
    {
        public Channel Channel { get; }
        public Player Player { get; }

        public ChannelEventArgs(Channel channel, Player player)
        {
            Channel = channel;
            Player = player;
        }
    }
}
