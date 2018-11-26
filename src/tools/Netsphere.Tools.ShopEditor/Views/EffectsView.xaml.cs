using Avalonia.Markup.Xaml;
using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class EffectsView : View<EffectsViewModel>
    {
        public EffectsView()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
