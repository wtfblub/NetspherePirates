using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class ViewModel : ReactiveObject
    {
        public ReactiveProperty<bool> IsInitialized { get; }

        public ViewModel()
        {
            IsInitialized = new ReactiveProperty<bool>();
        }
    }
}
