using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class SelectPreviewView : View<SelectPreviewViewModel>
    {
        public SelectPreviewView()
        {
            DataContext = ViewModel = new SelectPreviewViewModel();
            InitializeComponent();
        }
    }
}
