using System.Linq;
using Avalonia.Controls;
using Netsphere.Tools.ShopEditor.Views;
using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public TabItem[] TabPages { get; }
        public ReactiveProperty<int> CurrentIndex { get; }

        public MainViewModel()
        {
            var views = new IViewFor[]
            {
                new PricesView(),
                new EffectsView(),
                new ItemsView()
            };

            TabPages = views.Select(x => new TabItem
            {
                Header = ((TabViewModel)x.ViewModel).Header,
                Content = x
            }).ToArray();
            CurrentIndex = new ReactiveProperty<int>(0);
        }
    }
}
