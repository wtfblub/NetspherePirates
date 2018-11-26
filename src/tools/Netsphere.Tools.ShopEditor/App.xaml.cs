using Avalonia;
using Avalonia.Markup.Xaml;

namespace Netsphere.Tools.ShopEditor
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
