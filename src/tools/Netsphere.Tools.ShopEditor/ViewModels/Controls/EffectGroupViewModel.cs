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
    public class EffectGroupViewModel : ReactiveObject
    {
        public ShopEffectGroup EffectGroup { get; }
        public ReactiveCommand AddEffect { get; }
        public ReactiveCommand Delete { get; }

        public EffectGroupViewModel(ShopEffectGroup effectGroup)
        {
            EffectGroup = effectGroup;
            AddEffect = ReactiveCommand.CreateFromTask(AddEffectImpl);
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            EffectGroup.WhenAnyValue(x => x.Name.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task AddEffectImpl()
        {
            try
            {
                await ShopService.Instance.NewEffect(EffectGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add effect", ex).ShowDialog();
            }
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(EffectGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete effect group", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            try
            {
                await ShopService.Instance.Update(EffectGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update effect group", ex).ShowDialog();
            }
        }
    }
}
