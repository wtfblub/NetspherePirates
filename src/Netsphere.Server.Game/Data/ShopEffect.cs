using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class ShopEffect
    {
        public int Id { get; set; }
        public uint Effect { get; set; }

        public ShopEffect(ShopEffectEntity entity)
        {
            Id = entity.Id;
            Effect = entity.Effect;
        }
    }
}
