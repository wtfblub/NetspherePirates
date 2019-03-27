using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class SelectItemView : View<SelectItemViewModel>
    {
        public SelectItemView()
        {
            DataContext = ViewModel = new SelectItemViewModel();
            InitializeComponent();
        }
    }
}
