using System.Collections.Generic;
using System.Linq;
using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class ShopEffectGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<ShopEffect> Effects { get; set; }

        public ShopEffectGroup(ShopEffectGroupEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Effects = entity.ShopEffects.Select(x => new ShopEffect(x)).ToList();
        }

        public ShopEffect GetEffect(int id)
        {
            return Effects.FirstOrDefault(effect => effect.Id == id);
        }
    }
}
