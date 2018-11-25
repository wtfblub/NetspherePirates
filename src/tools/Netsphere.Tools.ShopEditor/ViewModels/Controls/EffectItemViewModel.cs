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
    public class EffectItemViewModel : ReactiveObject
    {
        public ShopEffect Effect { get; }
        public ReactiveCommand Change { get; }
        public ReactiveCommand Delete { get; }

        public EffectItemViewModel(ShopEffect effect)
        {
            Effect = effect;
            Change = ReactiveCommand.CreateFromTask(ChangeImpl);
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            Effect.WhenAnyValue(x => x.Effect.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task ChangeImpl()
        {
            var selectEffect = new SelectEffectView();
            var effect = await OverlayService.Show<Effect>(selectEffect);
            if (effect == null)
                return;

            Effect.Effect.Value = effect.Id;
        }

        private async Task DeleteImpl()
        {
            try
            {
                await ShopService.Instance.Delete(Effect);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to delete effect", ex).ShowDialog();
            }
        }

        private async void UpdateImpl()
        {
            if (Effect == null)
                return;

            try
            {
                await ShopService.Instance.Update(Effect);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to update effect", ex).ShowDialog();
            }
        }
    }
}
