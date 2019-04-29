using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class ItemsViewModel : TabViewModel
    {
        private const int ItemsPerPage = 20;
        private readonly ObservableAsPropertyHelper<string> _pageString;

        public override string Header => "Items";

        public ReactiveCommand AddItem { get; }
        public ReactiveCommand NextPage { get; }
        public ReactiveCommand PrevPage { get; }
        public IReactiveList<ShopItem> Items { get; }
        public ReactiveProperty<int> CurrentPage { get; }
        public ReactiveProperty<int> PageCount { get; }
        public string PageString => _pageString.Value;
        public ReactiveProperty<string> Search { get; }

        public ItemsViewModel()
        {
            AddItem = ReactiveCommand.CreateFromTask(AddItemImpl);
            Items = new ReactiveList<ShopItem>();
            CurrentPage = new ReactiveProperty<int>(1);
            PageCount = new ReactiveProperty<int>(ShopService.Instance.Items.Count);
            Search = new ReactiveProperty<string>("");

            ShopService.Instance.Items.Changed.Subscribe(_ =>
            {
                PageCount.Value = GetFilteredItems().Count() / ItemsPerPage;
                UpdateItems();
            });
            
            _pageString = this.WhenAnyValue(x => x.CurrentPage.Value, x => x.PageCount.Value)
                .Select(x => $"{x.Item1} / {x.Item2}")
                .ToProperty(this, x => x.PageString);

            this.WhenAnyValue(x => x.PageCount.Value)
                .Subscribe(x =>
                {
                    if (CurrentPage.Value > x)
                        CurrentPage.Value = x;

                    UpdateItems();
                });

            this.WhenAnyValue(x => x.Search.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    PageCount.Value = GetFilteredItems().Count() / ItemsPerPage;
                    UpdateItems();
                });

            var canNextPage = this.WhenAnyValue(x => x.CurrentPage.Value, x => x.PageCount.Value)
                .Select(x => x.Item1 < x.Item2);

            var canPrevPage = this.WhenAnyValue(x => x.CurrentPage.Value)
                .Select(x => x > 1);

            NextPage = ReactiveCommand.CreateFromTask(NextPageImpl, canNextPage);
            PrevPage = ReactiveCommand.CreateFromTask(PrevPageImpl, canPrevPage);
        }

        private async Task AddItemImpl()
        {
            try
            {
                var selectItem = new SelectItemView();
                var item = await OverlayService.Show<Item>(selectItem);
                if (item != null)
                    await ShopService.Instance.NewItem(item.ItemNumber);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add item", ex).ShowDialog(Application.Current.MainWindow);
            }
        }

        private async Task NextPageImpl()
        {
            CurrentPage.Value++;
            UpdateItems();
        }

        private async Task PrevPageImpl()
        {
            CurrentPage.Value--;
            UpdateItems();
        }

        private void UpdateItems()
        {
            Items.Clear();
            Items.AddRange(GetFilteredItems().Skip(ItemsPerPage * (CurrentPage.Value - 1)).Take(ItemsPerPage));
        }

        private IEnumerable<ShopItem> GetFilteredItems()
        {
            return ShopService.Instance.Items
                .Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(Search.Value))
                        return true;

                    var words = Search.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return words.All(word => x.DisplayName.Contains(word, StringComparison.OrdinalIgnoreCase));
                });
        }
    }
}
