using Avalonia.Markup.Xaml;
using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class ItemsView : View<ItemsViewModel>
    {
        public ItemsView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
