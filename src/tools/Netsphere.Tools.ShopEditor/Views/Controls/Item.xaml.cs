using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.ViewModels.Controls;

namespace Netsphere.Tools.ShopEditor.Views.Controls
{
    public sealed class Item : View<ItemViewModel>
    {
        public Item()
            : base(null, true)
        {
        }

        protected override ItemViewModel GetViewModelFromDataContext(object dataContext)
        {
            if (dataContext is ShopItem item)
                return new ItemViewModel(item);

            return null;
        }
    }
}
