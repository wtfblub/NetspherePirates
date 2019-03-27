using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class PricesView : View<PricesViewModel>
    {
        public PricesView()
        {
            DataContext = ViewModel = new PricesViewModel();
            InitializeComponent();
        }
    }
}
