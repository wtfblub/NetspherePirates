using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class SelectEffectView : View<SelectEffectViewModel>
    {
        public SelectEffectView(bool showEffectMatch = false)
        {
            DataContext = ViewModel = new SelectEffectViewModel(showEffectMatch);
            InitializeComponent();
        }
    }
}
