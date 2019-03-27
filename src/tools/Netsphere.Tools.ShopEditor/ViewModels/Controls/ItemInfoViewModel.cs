using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveUI;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels.Controls
{
    public class ItemInfoViewModel : ViewModel
    {
        public ShopService ShopService => ShopService.Instance;
        public ShopItemInfo ItemInfo { get; }
        public ReactiveCommand Delete { get; }

        public ItemInfoViewModel(ShopItemInfo itemInfo)
        {
            ItemInfo = itemInfo;
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            ItemInfo.WhenAnyValue(
                    x => x.PriceGroup.Value,
                    x => x.EffectGroup.Value,
                    x => x.DiscountPercentage.Value,
                    x => x.IsEnabled.Value)
                .Where(x => IsInitialized.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(ItemInfo);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete iteminfo", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            if (ItemInfo == null)
                return;

            try
            {
                await ShopService.Instance.Update(ItemInfo);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update iteminfo", ex).ShowDialog();
            }
        }
    }
}
