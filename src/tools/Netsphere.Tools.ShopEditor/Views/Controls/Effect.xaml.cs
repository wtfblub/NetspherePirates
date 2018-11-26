using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class Effect : View<EffectViewModel>
    {
        public Effect()
            : base(null, true)
        {
        }

        protected override EffectViewModel GetViewModelFromDataContext(object dataContext)
        {
            if(dataContext is ShopEffect effect)
                return new EffectViewModel(effect);

            return null;
        }
    }
}
