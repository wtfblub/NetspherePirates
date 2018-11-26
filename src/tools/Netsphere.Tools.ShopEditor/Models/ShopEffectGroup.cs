using System.Linq;
using Netsphere.Database.Game;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopEffectGroup : ReactiveObject
    {
        public int Id { get; }
        public ReactiveProperty<string> Name { get; }
        public IReactiveList<ShopEffect> Effects { get; }

        public ShopEffectGroup(ShopEffectGroupEntity entity)
        {
            Id = entity.Id;
            Name = new ReactiveProperty<string>(entity.Name);
            Effects = new ReactiveList<ShopEffect>(entity.ShopEffects.Select(x => new ShopEffect(this, x)));
        }

        public override string ToString()
        {
            return Name.Value;
        }
    }
}
