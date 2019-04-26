using System;
using System.Threading.Tasks;
using Avalonia;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class EffectsViewModel : TabViewModel
    {
        public override string Header => "Effects";

        public ShopService ShopService { get; }
        public ReactiveCommand AddEffectGroup { get; }

        public EffectsViewModel()
        {
            ShopService = ShopService.Instance;
            AddEffectGroup = ReactiveCommand.CreateFromTask(AddEffectGroupImpl);
        }

        private async Task AddEffectGroupImpl()
        {
            try
            {
                await ShopService.Instance.NewEffectGroup();
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add effect group", ex).ShowDialog(Application.Current.MainWindow);
            }
        }
    }
}
