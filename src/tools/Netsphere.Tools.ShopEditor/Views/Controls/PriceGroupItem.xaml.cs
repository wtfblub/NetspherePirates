using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class PriceGroupItem : View<PriceGroupItemViewModel>
    {
        public PriceGroupItem()
            : base(null, true)
        {
        }

        protected override PriceGroupItemViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopPriceGroup priceGroup)
                return new PriceGroupItemViewModel(priceGroup);

            return null;
        }
    }
}
