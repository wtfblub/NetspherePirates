using System;
using System.Linq;
using System.Reactive.Linq;
using Netsphere.Tools.ShopEditor.Services;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveUI.Legacy;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class SelectPreviewViewModel : ViewModel
    {
        public ReactiveCommand Select { get; }
        public ReactiveCommand Cancel { get; }
        public ReactiveProperty<string> Search { get; }
        public IReactiveList<EffectMatch> Effects { get; }
        public ReactiveProperty<EffectMatch> SelectedEffect { get; }

        public SelectPreviewViewModel()
        {
            Search = new ReactiveProperty<string>();
            Effects = new ReactiveList<EffectMatch>();
            SelectedEffect = new ReactiveProperty<EffectMatch>();
            Search.WhenAnyValue(x => x.Value)
                .Throttle(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(search =>
                {
                    Effects.Clear();
                    if (string.IsNullOrWhiteSpace(search))
                    {
                        Effects.AddRange(ResourceService.Instance.Previews);
                        return;
                    }

                    var split = search.Split(' ');
                    var effects = ResourceService.Instance.Previews
                        .Where(effect => split.All(word => effect.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
                        .ToArray();
                    Effects.AddRange(effects);
                });

            var canSelect = SelectedEffect.WhenAnyValue(x => x.Value).Select(x => x != null);
            Select = ReactiveCommand.Create(SelectImpl, canSelect);
            Cancel = ReactiveCommand.Create(CancelImpl);
        }

        private void SelectImpl()
        {
            OverlayService.Close(SelectedEffect.Value);
        }

        private void CancelImpl()
        {
            OverlayService.Close();
        }
    }
}
