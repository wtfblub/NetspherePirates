using System;

namespace Netsphere.Server.Game.Data
{
    public class EquipLimitInfo
    {
        public EquipLimit Rule { get; set; }
        public ItemLicense[] Whitelist { get; set; }

        public EquipLimitInfo()
        {
            Whitelist = Array.Empty<ItemLicense>();
        }
    }
}
