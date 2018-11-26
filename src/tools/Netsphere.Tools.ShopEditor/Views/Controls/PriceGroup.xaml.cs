using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class PriceGroup : View<PriceGroupViewModel>
    {
        public PriceGroup()
            : base(null, true)
        {
        }

        protected override PriceGroupViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopPriceGroup priceGroup)
                return new PriceGroupViewModel(priceGroup);

            return null;
        }
    }
}
