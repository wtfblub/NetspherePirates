using System;
using System.Threading.Tasks;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class ItemsViewModel : TabViewModel
    {
        public override string Header => "Items";

        public ShopService ShopService { get; }
        public ReactiveCommand AddItem { get; }

        public ItemsViewModel()
        {
            ShopService = ShopService.Instance;
            AddItem = ReactiveCommand.CreateFromTask(AddItemImpl);
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
    }
}
