using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public abstract class TabViewModel : ReactiveObject
    {
        public abstract string Header { get; }
    }
}
