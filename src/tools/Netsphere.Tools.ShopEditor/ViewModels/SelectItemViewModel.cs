using System;
using System.Linq;
using System.Reactive.Linq;
using Netsphere.Tools.ShopEditor.Services;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class SelectItemViewModel : ReactiveObject
    {
        public ReactiveCommand Select { get; }
        public ReactiveCommand Cancel { get; }
        public ReactiveProperty<string> Search { get; }
        public IReactiveList<Item> Items { get; }
        public ReactiveProperty<Item> SelectedItem { get; }

        public SelectItemViewModel()
        {
            Search = new ReactiveProperty<string>();
            Items = new ReactiveList<Item>();
            SelectedItem = new ReactiveProperty<Item>();
            Search.WhenAnyValue(x => x.Value)
                .Throttle(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(search =>
                {
                    Items.Clear();

                    // It's not possible to add an item more than once so remove existing shop items from the search
                    var items = ResourceService.Instance.Items.Where(item =>
                        ShopService.Instance.Items.All(x => x.ItemNumber != item.ItemNumber));

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        var split = search.Split(' ');
                        items = items.Where(item =>
                            split.All(word => item.Name.Contains(word, StringComparison.OrdinalIgnoreCase)));
                    }

                    Items.AddRange(items);
                });

            var canSelect = SelectedItem.WhenAnyValue(x => x.Value).Select(x => x != null);
            Select = ReactiveCommand.Create(SelectImpl, canSelect);
            Cancel = ReactiveCommand.Create(CancelImpl);
        }

        private void SelectImpl()
        {
            OverlayService.Close(SelectedItem.Value);
        }

        private void CancelImpl()
        {
            OverlayService.Close();
        }
    }
}
