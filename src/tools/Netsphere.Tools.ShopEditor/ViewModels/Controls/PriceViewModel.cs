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
    public class PriceViewModel : ViewModel
    {
        public ShopPrice Price { get; }
        public ReactiveCommand Delete { get; }

        public PriceViewModel(ShopPrice price)
        {
            Price = price;
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            Price.WhenAnyValue(_ => _.PeriodType.Value, _ => _.Period.Value, _ => _.Price.Value,
                    _ => _.IsRefundable.Value, _ => _.Durability.Value, _ => _.IsEnabled.Value)
                .Where(x => IsInitialized.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(Price);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete price", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            if (Price == null)
                return;

            try
            {
                await ShopService.Instance.Update(Price);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update price", ex).ShowDialog();
            }
        }
    }
}
