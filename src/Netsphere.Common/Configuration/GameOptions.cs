namespace Netsphere.Common.Configuration
{
    public class GameOptions
    {
        public bool EnableTutorial { get; set; }
        public bool EnableLicenseRequirement { get; set; }
        public int StartLevel { get; set; }
        public int StartPEN { get; set; }
        public int StartAP { get; set; }
        public int StartCoins1 { get; set; }
        public int StartCoins2 { get; set; }
        public NickRestrictionOptions NickRestrictions { get; set; }
    }
}
