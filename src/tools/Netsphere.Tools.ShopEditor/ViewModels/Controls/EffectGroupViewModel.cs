using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Netsphere.Tools.ShopEditor.Models;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Views;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels.Controls
{
    public class EffectGroupViewModel : ViewModel
    {
        public ShopEffectGroup EffectGroup { get; }
        public ReactiveProperty<string> PreviewEffectName { get; }
        public ReactiveCommand ChangePreviewEffect { get; }
        public ReactiveCommand AddEffect { get; }
        public ReactiveCommand Delete { get; }

        public EffectGroupViewModel(ShopEffectGroup effectGroup)
        {
            EffectGroup = effectGroup;
            var effect = ResourceService.Instance.Previews.FirstOrDefault(x => x.Id == EffectGroup.PreviewEffect.Value);
            PreviewEffectName = new ReactiveProperty<string>(effect?.Name ?? "None");
            ChangePreviewEffect = ReactiveCommand.CreateFromTask(ChangePreviewEffectImpl);
            AddEffect = ReactiveCommand.CreateFromTask(AddEffectImpl);
            Delete = ReactiveCommand.CreateFromTask(DeleteImpl);
            EffectGroup.WhenAnyValue(x => x.Name.Value, x => x.PreviewEffect.Value)
                .Where(x => IsInitialized.Value)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateImpl());
        }

        private async Task ChangePreviewEffectImpl()
        {
            var selectEffect = new SelectPreviewView();
            var effect = await OverlayService.Show<EffectMatch>(selectEffect);
            if (effect == null)
                return;

            EffectGroup.PreviewEffect.Value = effect.Id;
            PreviewEffectName.Value = effect.Name;
        }

        private async Task AddEffectImpl()
        {
            try
            {
                await ShopService.Instance.NewEffect(EffectGroup);
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to add effect", ex).ShowDialog(Application.Current.MainWindow);
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
                await new MessageView("Error", "Unable to delete effect group", ex).ShowDialog(Application.Current.MainWindow);
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
                await new MessageView("Error", "Unable to update effect group", ex).ShowDialog(Application.Current.MainWindow);
            }
        }
    }
}
