using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Netsphere.Tools.ShopEditor.ViewModels;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class ConnectView : BaseWindow<ConnectViewModel>
    {
        private Button Connect => this.Get<Button>(nameof(Connect));
        private Button Exit => this.Get<Button>(nameof(Exit));
        private TextBox ResourcePath => this.Get<TextBox>(nameof(ResourcePath));

        public ConnectView()
        {
            DataContext = ViewModel = new ConnectViewModel();
            this.WhenActivated(_ =>
            {
                _(this.BindCommand(ViewModel, x => x.Connect, x => x.Connect));
                _(this.BindCommand(ViewModel, x => x.Exit, x => x.Exit));
                _(ViewModel.Connect.IsExecuting.Select(x => !x).BindTo(this, x => x.IsEnabled));

                _(Observable.FromEventPattern<PointerReleasedEventArgs>(x => ResourcePath.PointerReleased += x,
                        x => ResourcePath.PointerReleased -= x)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel, x => x.SelectResourceFile));
            });

            InitializeComponent();
        }
    }
}
