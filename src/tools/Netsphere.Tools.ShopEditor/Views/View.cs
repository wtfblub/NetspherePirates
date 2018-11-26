using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Views
{
    public abstract class View<TViewModel> : UserControl, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        public static readonly StyledProperty<TViewModel> ViewModelProperty =
            AvaloniaProperty.Register<View<TViewModel>, TViewModel>(nameof(ViewModel));

        private readonly IObservable<Unit> _activated;
        private readonly IObservable<Unit> _deactivated;

        public TViewModel ViewModel
        {
            get => GetValue(ViewModelProperty);
            set
            {
                SetValue(ViewModelProperty, value);
                if (!FixDataContext)
                    DataContext = value;
            }
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        IObservable<Unit> ICanActivate.Activated => _activated;
        IObservable<Unit> ICanActivate.Deactivated => _deactivated;

        protected bool FixDataContext { get; set; }

        protected View(bool fixDataContext = false)
            : this(Activator.CreateInstance<TViewModel>(), fixDataContext)
        {
        }

        protected View(TViewModel vm, bool fixDataContext = false)
        {
            AvaloniaXamlLoader.Load(this);
            FixDataContext = fixDataContext;
            ViewModel = vm;
            _activated = Observable.FromEventPattern<LogicalTreeAttachmentEventArgs>(x => AttachedToLogicalTree += x,
                x => AttachedToLogicalTree -= x).Select(_ => Unit.Default);
            _deactivated = Observable.FromEventPattern<LogicalTreeAttachmentEventArgs>(x => DetachedFromLogicalTree += x,
                x => DetachedFromLogicalTree -= x).Select(_ => Unit.Default);
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (!FixDataContext)
                return;

            // Attached properties are not working and I'm not sure why so this is a temporary solution
            // Also DataContext has some strange behaviour - Avalonia is in beta for a reason I guess
            var vm = GetViewModelFromDataContext(DataContext);
            if (vm != null)
                DataContext = ViewModel = vm;
        }

        protected virtual TViewModel GetViewModelFromDataContext(object dataContext)
        {
            return dataContext as TViewModel;
        }
    }
}
