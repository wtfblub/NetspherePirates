using Netsphere;

namespace EquipLimitExtended
{
    public class EquipLimitExtendedOptions
    {
        public EquipLimitRule[] Rules { get; set; }
    }

    public class EquipLimitRule
    {
        public string Keyword { get; set; }
        public GameRule[] GameRules { get; set; }
        public EquipLimit[] EquipLimits { get; set; }
        public EquipLimitRuleMode Mode { get; set; }
        public ItemNumber[] Items { get; set; }
    }

    public enum EquipLimitRuleMode
    {
        Whitelist,
        Blacklist
    }
}
