using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class PriceItem : View<PriceItemViewModel>
    {
        public PriceItem()
            : base(null, true)
        {
        }

        protected override PriceItemViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopPrice price)
                return new PriceItemViewModel(price);

            return null;
        }
    }
}
