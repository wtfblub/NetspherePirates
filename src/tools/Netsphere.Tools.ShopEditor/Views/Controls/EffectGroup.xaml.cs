using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class EffectGroup : View<EffectGroupViewModel>
    {
        public EffectGroup()
            : base(null, true)
        {
        }

        protected override EffectGroupViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopEffectGroup effectGroup)
                return new EffectGroupViewModel(effectGroup);

            return null;
        }
    }
}
