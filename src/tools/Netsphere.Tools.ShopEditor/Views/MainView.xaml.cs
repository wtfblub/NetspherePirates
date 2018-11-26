using Avalonia;
using Netsphere.Tools.ShopEditor.ViewModels;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class MainView : BaseWindow<MainViewModel>
    {
        public MainView()
        {
            this.WhenActivated(_ => Application.Current.MainWindow = this);
        }
    }
}
