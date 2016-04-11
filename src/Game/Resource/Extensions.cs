using System.Collections.Generic;
using System.Linq;

namespace Netsphere.Resource
{
    public static class ResourceExtensions
    {
        public static DefaultItem Get(this IEnumerable<DefaultItem> defaultItems, CharacterGender gender, CostumeSlot slot, byte variation)
        {
            return defaultItems.FirstOrDefault(item =>
                      item.Gender == gender && item.Variation == variation &&
                      item.ItemNumber.SubCategory == (byte)slot);
        } 
    }
}
