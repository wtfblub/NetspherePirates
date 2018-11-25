using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class EffectGroupItem : View<EffectGroupItemViewModel>
    {
        public EffectGroupItem()
            : base(null, true)
        {
        }

        protected override EffectGroupItemViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopEffectGroup effectGroup)
                return new EffectGroupItemViewModel(effectGroup);

            return null;
        }
    }
}
