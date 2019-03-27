using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public abstract class View<TViewModel> : ReactiveUserControl<TViewModel>
        where TViewModel : ViewModel
    {
        protected View()
        {
            Initialized += OnInitialized;
        }

        protected void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsInitialized.Value = true;
        }
    }
}
