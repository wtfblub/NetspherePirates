using System;
using Avalonia;
using Avalonia.Controls;
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
            : this(Activator.CreateInstance<TViewModel>())
        {
        }

        protected BaseWindow(TViewModel vm)
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = ViewModel = vm;
            FontFamily = new FontFamily(System.Drawing.SystemFonts.DefaultFont.FontFamily.Name);
            Initialized += OnInitialized;
            PositionChanged += OnPositionChanged;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsInitialized.Value = true;
        }

        private void OnPositionChanged(object sender, PointEventArgs e)
        {
            // bug Avalonia places the top left corner in the center on linux
            if (_fixedStartPosition || WindowStartupLocation != WindowStartupLocation.CenterScreen)
                return;

            var screen = Screens.Primary.Bounds;
            Position = new Point(screen.Width / 2 - Width / 2, screen.Height / 2 - Height / 2);
            _fixedStartPosition = true;
        }
    }
}
