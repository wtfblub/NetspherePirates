using System;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class ItemInfo : View<ItemInfoViewModel>
    {
        public ItemInfo()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is ShopItemInfo item)
                DataContext = ViewModel = new ItemInfoViewModel(item);
        }
    }
}
