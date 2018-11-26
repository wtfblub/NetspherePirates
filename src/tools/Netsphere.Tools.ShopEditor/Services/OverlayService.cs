using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using BlubLib.Threading.Tasks;

namespace Netsphere.Tools.ShopEditor.Services
{
    public class OverlayService
    {
        private static TaskCompletionSource<object> s_result;

        public static async Task<T> Show<T>(UserControl view)
        {
            var window = Application.Current.MainWindow;
            var overlay = window.Get<Grid>("Overlay");
            if (overlay.IsVisible)
                throw new InvalidOperationException("Overlay is already active");

            overlay.IsVisible = true;
            var overlayContent = overlay.FindControl<ContentControl>("OverlayContent");
            overlayContent.Content = view;
            s_result = new TaskCompletionSource<object>();
            return (T)await s_result.Task.AnyContext();
        }

        public static void Close(object result = null)
        {
            s_result?.TrySetResult(result);
            s_result = null;
            var window = Application.Current.MainWindow;
            var overlay = window.Get<Grid>("Overlay");
            overlay.IsVisible = false;
        }
    }
}
