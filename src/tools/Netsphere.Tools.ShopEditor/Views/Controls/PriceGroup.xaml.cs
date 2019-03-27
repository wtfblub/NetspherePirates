using System;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class PriceGroup : View<PriceGroupViewModel>
    {
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is ShopPriceGroup priceGroup)
                DataContext = ViewModel = new PriceGroupViewModel(priceGroup);
        }
    }
}
