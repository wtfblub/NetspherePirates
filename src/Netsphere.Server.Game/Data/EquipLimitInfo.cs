using System;

namespace Netsphere.Server.Game.Data
{
    public class EquipLimitInfo
    {
        public int Id { get; set; }
        public ItemNumber[] Blacklist { get; set; }

        public EquipLimitInfo()
        {
            Blacklist = Array.Empty<ItemNumber>();
        }
    }
}
