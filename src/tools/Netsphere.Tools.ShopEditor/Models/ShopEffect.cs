using System.Linq;
using System.Reactive.Linq;
using Netsphere.Database.Game;
using Netsphere.Tools.ShopEditor.Services;
using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Models
{
    public class ShopEffect : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _name;

        public int Id { get; }
        public ShopEffectGroup EffectGroup { get; }
        public ReactiveProperty<uint> Effect { get; }
        public string Name => _name.Value;

        public ShopEffect(ShopEffectGroup effectGroup, ShopEffectEntity entity)
        {
            Id = entity.Id;
            EffectGroup = effectGroup;
            Effect = new ReactiveProperty<uint>(entity.Effect);

            _name = this.WhenAnyValue(x => x.Effect.Value)
                .Select(effectId => ResourceService.Instance.Effects.First(effect => effect.Id == effectId).Name)
                .ToProperty(this, x => x.Name);
        }
    }
}
