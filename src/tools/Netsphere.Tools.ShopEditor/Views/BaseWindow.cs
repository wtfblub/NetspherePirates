using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public abstract class BaseWindow<TViewModel> : ReactiveWindow<TViewModel>
        where TViewModel : ViewModel
    {
        private bool _fixedStartPosition;

        protected BaseWindow()
        {
            FontFamily = new FontFamily(System.Drawing.SystemFonts.DefaultFont.FontFamily.Name);
            Initialized += OnInitialized;
        }

        protected void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsInitialized.Value = true;
        }
    }
}
