using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Netsphere.Resource;
using NetsphereExplorer.Controls;
using NetsphereExplorer.ViewModels;
using ReactiveUI;
using View = BlubLib.GUI.Controls.View;

namespace NetsphereExplorer.Views
{
    internal partial class CleanerView : View, IViewFor<CleanerViewModel>
    {
        public CleanerViewModel ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CleanerViewModel)value;
        }

        public CleanerView(S4Zip zip)
        {
            InitializeComponent();
            ViewModel = new CleanerViewModel(zip);

            this.WhenActivated(d =>
            {
                d(this.WhenAnyValue(x => x.ViewModel.FileCount, x => x.ViewModel.Status)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => FormatTitle())
                    .BindTo(lblTitle, x => x.Text));

                d(this.WhenAnyValue(x => x.ViewModel.Status, x => x.ViewModel.Progress, x => x.ViewModel.FileCount)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => FormatProgressText())
                    .BindTo(lblProgress, x => x.Text));

                d(this.WhenAnyValue(x => x.ViewModel.Progress)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => (int)(ViewModel.Progress / (float)ViewModel.FileCount * 100))
                    .BindTo(pbProgress, x => x.Value));

                d(this.WhenAnyValue(x => x.ViewModel.Status)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => _ == CleanerStatus.Work ? ProgressBarStyle.Continuous : ProgressBarStyle.Marquee)
                    .BindTo(pbProgress, x => x.Style));
            });
        }

        private string FormatTitle()
        {
            switch (ViewModel.Status)
            {
                case CleanerStatus.Preparing:
                    return "Preparing...";

                case CleanerStatus.Work:
                    return "Looking for unused files...";

                case CleanerStatus.Cleanup:
                    return "Deleting unused files...";
            }
            return null;
        }

        private string FormatProgressText()
        {
            switch (ViewModel.Status)
            {
                case CleanerStatus.Preparing:
                    return "Preparing...";

                case CleanerStatus.Work:
                    return $"{(int)(ViewModel.Progress / (float)ViewModel.FileCount * 100)}% complete";

                case CleanerStatus.Cleanup:
                    return "Deleting unused files...";
            }
            return null;
        }

        public static Task Start(IWin32Window window, Overlay overlay, S4Zip zip)
        {
            var view = new CleanerView(zip);
            overlay.Show(view);
            return Task.Run(view.ViewModel.Start);
        }
    }
}
