using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Netsphere.Tools.ShopEditor.ViewModels;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public class MessageView : BaseWindow<MessageViewModel>
    {
        private TextBlock Message => this.Get<TextBlock>(nameof(Message));
        private TextBox Exception => this.Get<TextBox>(nameof(Exception));
        private Button Confirm => this.Get<Button>(nameof(Confirm));

        public MessageView(string title, string message, Exception ex)
        {
            DataContext = ViewModel = new MessageViewModel(title, message, ex);
            this.WhenActivated(_ =>
            {
                _(this.OneWayBind(ViewModel, x => x.Title, x => x.Title));

                _(this.WhenAnyValue(x => x.ViewModel.Title)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, x => x.Title));

                _(this.WhenAnyValue(x => x.ViewModel.Message)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(this, x => x.Message.Text));

                _(this.WhenAnyValue(x => x.ViewModel.Exception)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(x => x != null)
                    .BindTo(this, x => x.Exception.IsVisible));

                _(this.WhenAnyValue(x => x.ViewModel.Exception)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(x => x.ToString())
                    .BindTo(this, x => x.Exception.Text));
            });

            InitializeComponent();
            Confirm.Click += (_, __) => Close();
        }
    }
}
