using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels.Controls
{
    public class ItemViewModel : ViewModel
    {
        public ShopItem Item { get; }
        public ReactiveCommand AddItemInfo { get; }
        public ReactiveCommand Delete { get; }

        public ItemViewModel(ShopItem item)
        {
            Item = item;
            AddItemInfo = ReactiveCommand.CreateFromTask(AddItemInfoImpl);
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);

            Item.WhenAnyValue(
                    x => x.ItemNumber,
                    x => x.RequiredGender.Value,
                    x => x.RequiredLicense.Value,
                    x => x.Colors.Value,
                    x => x.UniqueColors.Value,
                    x => x.RequiredLevel.Value,
                    x => x.LevelLimit.Value,
                    x => x.RequiredMasterLevel.Value,
                    x => x.IsOneTimeUse.Value,
                    x => x.IsDestroyable.Value,
                    (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10) => default(object))
                .Where(x => IsInitialized.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task AddItemInfoImpl()
        {
            try
            {
                // TODO Select price and effect group
                await ShopService.Instance.NewItemInfo(Item);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add iteminfo", ex).ShowDialog();
            }
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(Item);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete item", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            try
            {
                await ShopService.Instance.Update(Item);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update item", ex).ShowDialog();
            }
        }
    }
}
