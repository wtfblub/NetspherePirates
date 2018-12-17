namespace Netsphere.Server.Game
{
    public class ScoreContext
    {
        public Player Player { get; }
        public bool IsSentry => SentryId != null;
        public PlayerScore Score => Player.Score;
        internal LongPeerId SentryId { get; }

        public ScoreContext(Player player, LongPeerId sentryId)
        {
            Player = player;
            SentryId = sentryId;
        }
    }
}
