using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public abstract class BaseWindow<TViewModel> : Window, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private readonly IObservable<Unit> _activated;
        private readonly IObservable<Unit> _deactivated;
        private TViewModel _viewModel;
        private bool _fixedStartPosition;

        public TViewModel ViewModel
        {
            get => _viewModel;
            set => DataContext = _viewModel = value;
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        IObservable<Unit> ICanActivate.Activated => _activated;
        IObservable<Unit> ICanActivate.Deactivated => _deactivated;

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
            ViewModel = vm;
            FontFamily = new FontFamily(System.Drawing.SystemFonts.DefaultFont.FontFamily.Name);
            _activated = Observable.FromEventPattern(x => Activated += x, x => Activated -= x).Select(_ => Unit.Default);
            _deactivated = Observable.FromEventPattern(x => Deactivated += x, x => Deactivated -= x).Select(_ => Unit.Default);
            PositionChanged += OnPositionChanged;
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
