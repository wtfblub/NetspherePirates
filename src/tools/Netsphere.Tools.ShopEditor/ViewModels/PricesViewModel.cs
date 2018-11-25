using System;
using System.Threading.Tasks;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class PricesViewModel : TabViewModel
    {
        public override string Header => "Prices";

        public ShopService ShopService { get; }
        public ReactiveCommand AddPriceGroup { get; }

        public PricesViewModel()
        {
            ShopService = ShopService.Instance;
            AddPriceGroup = ReactiveCommand.CreateFromTask(AddPriceGroupImpl);
        }

        private async Task AddPriceGroupImpl()
        {
            try
            {
                await ShopService.Instance.NewPriceGroup();
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add price group", ex).ShowDialog();
            }
        }
    }
}
