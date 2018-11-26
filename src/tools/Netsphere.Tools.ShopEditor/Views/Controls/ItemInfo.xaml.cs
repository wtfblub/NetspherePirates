using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class ItemInfo : View<ItemInfoViewModel>
    {
        public ItemInfo()
            : base(null, true)
        {
        }

        protected override ItemInfoViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopItemInfo itemInfo)
                return new ItemInfoViewModel(itemInfo);

            return null;
        }
    }
}
