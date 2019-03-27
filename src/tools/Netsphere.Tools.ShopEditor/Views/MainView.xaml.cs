using Avalonia;
using Netsphere.Tools.ShopEditor.ViewModels;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class MainView : BaseWindow<MainViewModel>
    {
        public MainView()
        {
            DataContext = ViewModel = new MainViewModel();
            this.WhenActivated(_ => Application.Current.MainWindow = this);
            InitializeComponent();
        }
    }
}
