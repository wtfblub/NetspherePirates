using Avalonia;
using Netsphere.Tools.ShopEditor.Views;

namespace Netsphere.Tools.ShopEditor
{
    internal static class Program
    {
        private static void Main()
        {
            BuildAvaloniaApp().Start<ConnectView>();
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI();
        }
    }
}
