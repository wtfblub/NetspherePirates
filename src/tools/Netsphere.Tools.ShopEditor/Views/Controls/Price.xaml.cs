using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class Price : View<PriceViewModel>
    {
        public Price()
            : base(null, true)
        {
        }

        protected override PriceViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopPrice price)
                return new PriceViewModel(price);

            return null;
        }
    }
}
