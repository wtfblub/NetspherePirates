using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels.Controls
{
    public class PriceGroupViewModel : ViewModel
    {
        public ShopPriceGroup PriceGroup { get; }
        public ReactiveCommand AddPrice { get; }
        public ReactiveCommand Delete { get; }

        public PriceGroupViewModel(ShopPriceGroup priceGroup)
        {
            PriceGroup = priceGroup;
            AddPrice = ReactiveCommand.CreateFromTask(AddPriceImpl);
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            PriceGroup.WhenAnyValue(x => x.Name.Value, x => x.PriceType.Value)
                .Where(x => IsInitialized.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task AddPriceImpl()
        {
            try
            {
                await ShopService.Instance.NewPrice(PriceGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add price", ex).ShowDialog();
            }
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(PriceGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete price group", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            try
            {
                await ShopService.Instance.Update(PriceGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update price group", ex).ShowDialog();
            }
        }
    }
}
