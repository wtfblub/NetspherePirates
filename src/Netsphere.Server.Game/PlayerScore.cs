namespace Netsphere.Server.Game
{
    public abstract class PlayerScore
    {
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint HealAssists { get; set; }
        public uint Suicides { get; set; }
        public uint Deaths { get; set; }

        public abstract uint GetTotalScore();

        public virtual void Reset()
        {
            Kills = 0;
            KillAssists = 0;
            HealAssists = 0;
            Suicides = 0;
            Deaths = 0;
        }
    }
}
