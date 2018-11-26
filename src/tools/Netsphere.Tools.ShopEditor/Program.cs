using Avalonia;
using Netsphere.Tools.ShopEditor.Views;

namespace Netsphere.Tools.ShopEditor
{
    internal static class Program
    {
        private static void Main()
        {
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .Start<ConnectView>();

        }
    }
}
