using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;

namespace Netsphere.Shop
{
    internal class ShopEffectGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<ShopEffect> Effects { get; set; }

        public ShopEffectGroup(ShopEffectGroupDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Effects = dto.ShopEffects.Select(e => new ShopEffect(e)).ToList();
        }
    }

    internal class ShopEffect
    {
        public int Id { get; set; }
        public uint Effect { get; set; }

        public ShopEffect(ShopEffectDto dto)
        {
            Id = dto.Id;
            Effect = dto.Effect;
        }
    }
}
