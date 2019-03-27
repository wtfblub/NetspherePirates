using System;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class Item : View<ItemViewModel>
    {
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is ShopItem item)
                DataContext = ViewModel = new ItemViewModel(item);
        }
    }
}
