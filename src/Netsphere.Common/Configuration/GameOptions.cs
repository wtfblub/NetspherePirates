namespace Netsphere.Common.Configuration
{
    public class GameOptions
    {
        public bool EnableTutorial { get; set; }
        public bool EnableLicenseRequirement { get; set; }
        public int MaxLevel { get; set; }
        public int StartLevel { get; set; }
        public int StartPEN { get; set; }
        public int StartAP { get; set; }
        public int StartCoins1 { get; set; }
        public int StartCoins2 { get; set; }
        public NickRestrictionOptions NickRestrictions { get; set; }
        public int DurabilityLossPerDeath { get; set; }
        public int DurabilityLossPerMinute { get; set; }
        public DeathmatchOptions Deathmatch { get; set; }
    }

    public class DeathmatchOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerKillAssist { get; set; }
        public int PointsPerHealAssist { get; set; }
        public int PointsPerDeath { get; set; }
    }

    public class ExperienceRateOptions
    {
        public float ScoreFactor { get; set; }
        public float FirstPlaceBonus { get; set; }
        public float SecondPlaceBonus { get; set; }
        public float ThirdPlaceBonus { get; set; }
        public float PlayerCountFactor { get; set; }
        public float ExperiencePerMinute { get; set; }
    }
}
