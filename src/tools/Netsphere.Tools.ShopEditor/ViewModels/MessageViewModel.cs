using System;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class MessageViewModel : ReactiveObject
    {
        private string _title;
        private string _message;
        private Exception _exception;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public Exception Exception
        {
            get => _exception;
            set => this.RaiseAndSetIfChanged(ref _exception, value);
        }

        public MessageViewModel(string title, string message, Exception ex)
        {
            Title = title;
            Message = message;
            Exception = ex;
        }
    }
}
