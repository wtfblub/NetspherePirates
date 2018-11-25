using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class EffectItem : View<EffectItemViewModel>
    {
        public EffectItem()
            : base(null, true)
        {
        }

        protected override EffectItemViewModel GetViewModelFromDataContext(object dataContext)
        {
            if(dataContext is ShopEffect effect)
                return new EffectItemViewModel(effect);

            return null;
        }
    }
}
