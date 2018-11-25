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
    public class SelectEffectViewModel : ReactiveObject
    {
        public ReactiveCommand Select { get; }
        public ReactiveCommand Cancel { get; }
        public ReactiveProperty<string> Search { get; }
        public IReactiveList<Effect> Effects { get; }
        public ReactiveProperty<Effect> SelectedEffect { get; }

        public SelectEffectViewModel()
        {
            Search = new ReactiveProperty<string>();
            Effects = new ReactiveList<Effect>();
            SelectedEffect = new ReactiveProperty<Effect>();
            Search.WhenAnyValue(x => x.Value)
                .Throttle(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(search =>
                {
                    Effects.Clear();
                    if (string.IsNullOrWhiteSpace(search))
                    {
                        Effects.AddRange(ResourceService.Instance.Effects);
                        return;
                    }

                    var split = search.Split(' ');
                    var effects = ResourceService.Instance.Effects
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
