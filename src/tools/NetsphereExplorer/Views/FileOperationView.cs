using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPICodePack.Core.Dialogs.TaskDialogs;
using BlubLib.Native;
using NetsphereExplorer.Controls;
using NetsphereExplorer.Filesystem;
using NetsphereExplorer.ViewModels;
using ReactiveUI;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;
using View = BlubLib.GUI.Controls.View;

namespace NetsphereExplorer.Views
{
    internal partial class FileOperationView : View, IViewFor<FileOperationViewModel>
    {
        private static readonly FolderBrowserDialog s_folderBrowser = new FolderBrowserDialog();
        private static readonly OpenFileDialog s_fileBrowser = new OpenFileDialog { Multiselect = true };

        public FileOperationViewModel ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (FileOperationViewModel)value; }
        }

        public FileOperationView(IFilesystem filesystem)
        {
            InitializeComponent();
            ViewModel = new FileOperationViewModel(filesystem);

            this.WhenActivated(d =>
            {
                d(this.BindCommand(ViewModel, vm => vm.PauseOrResume, v => v.btnPause));
                d(this.BindCommand(ViewModel, vm => vm.Cancel, v => v.btnCancel));

                d(this.WhenAnyValue(x => x.ViewModel.FileCount, x => x.ViewModel.Mode)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => FormatTitle())
                    .BindTo(lblTitle, x => x.Text));

                d(this.WhenAnyValue(x => x.ViewModel.Destination)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(Path.GetFileName)
                    .Subscribe(str =>
                        {
                            if (str == null)
                                lblDestination.Visible = false;
                            else
                                lblDestination.Text = str;
                        }));

                d(this.WhenAnyValue(x => x.ViewModel.Destination)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(destination => toolTip.SetToolTip(lblDestination, destination)));

                d(lblDestination.Events()
                    .LinkClicked
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => Process.Start(ViewModel.Destination)));

                d(this.WhenAnyValue(x => x.ViewModel.Status, x => x.ViewModel.Progress, x => x.ViewModel.FileCount)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => FormatProgressText())
                    .BindTo(lblProgress, x => x.Text));

                d(this.WhenAnyValue(x => x.ViewModel.Status)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(UpdateProgressBarStyle));

                d(this.WhenAnyValue(x => x.ViewModel.Status)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(status => status == OperationStatus.Paused ?
                        Properties.Resources.control_180 : Properties.Resources.control_pause)
                    .BindTo(btnPause, x => x.Image));

                d(this.WhenAnyValue(x => x.ViewModel.Progress)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(_ => (int)(ViewModel.Progress / (float)ViewModel.FileCount * 100))
                    .BindTo(pbProgress, x => x.Value));
            });
        }

        private string FormatTitle()
        {
            switch (ViewModel.Mode)
            {
                case OperationMode.Copy:
                    return $"Copying {ViewModel.FileCount} items to";

                case OperationMode.Delete:
                    return $"Removing {ViewModel.FileCount} items";

                case OperationMode.Add:
                    return $"Adding {ViewModel.FileCount} items";
            }
            return null;
        }

        private string FormatProgressText()
        {
            switch (ViewModel.Status)
            {
                case OperationStatus.Discovery:
                    return $"Discovered {ViewModel.FileCount} items...";

                case OperationStatus.Work:
                    return $"{(int)(ViewModel.Progress / (float)ViewModel.FileCount * 100)}% complete";

                case OperationStatus.Paused:
                    return $"Paused - {(int)(ViewModel.Progress / (float)ViewModel.FileCount * 100)}% complete";

                case OperationStatus.Cancel:
                    return "Canceling...";
            }
            return null;
        }

        private void UpdateProgressBarStyle(OperationStatus status)
        {
            switch (status)
            {
                case OperationStatus.Discovery:
                    pbProgress.Style = ProgressBarStyle.Marquee;
                    pbProgress.State = ProgressBarState.Normal;
                    break;

                case OperationStatus.Work:
                    pbProgress.Style = ProgressBarStyle.Continuous;
                    pbProgress.State = ProgressBarState.Normal;
                    break;

                case OperationStatus.Paused:
                    pbProgress.Style = ProgressBarStyle.Continuous;
                    pbProgress.State = ProgressBarState.Paused;
                    break;

                case OperationStatus.Cancel:
                    pbProgress.Style = ProgressBarStyle.Continuous;
                    pbProgress.State = ProgressBarState.Error;
                    break;
            }
        }

        public static Task Extract(IWin32Window window, Overlay overlay, IFilesystem filesystem, IFolder folder)
        {
            if (s_folderBrowser.ShowDialog(window) != DialogResult.OK)
                return Task.CompletedTask;

            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            return Task.Run(() => view.ViewModel.StartExtract(folder, s_folderBrowser.SelectedPath));
        }

        public static Task Extract(IWin32Window window, Overlay overlay, IFilesystem filesystem, object[] items)
        {
            if (s_folderBrowser.ShowDialog(window) != DialogResult.OK)
                return Task.CompletedTask;

            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            return Task.Run(() => view.ViewModel.StartExtract(items, s_folderBrowser.SelectedPath));
        }

        public static async Task<bool> Delete(IWin32Window window, Overlay overlay, IFilesystem filesystem, IFolder folder)
        {
            var result = TaskDialog.Show(window, $"Are you sure you want to delete {folder.FullName}?",
                buttons: TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No);

            if (result == TaskDialogResult.No)
                return false;

            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            await Task.Run(() => view.ViewModel.StartDelete(folder));
            return true;
        }

        public static async Task<bool> Delete(IWin32Window window, Overlay overlay, IFilesystem filesystem, object[] items)
        {
            var result = TaskDialog.Show(window, $"Are you sure you want to delete {items.Length} items?",
                buttons: TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No);

            if (result == TaskDialogResult.No)
                return false;

            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            await Task.Run(() => view.ViewModel.StartDelete(items));
            return true;
        }

        public static async Task<bool> Add(IWin32Window window, Overlay overlay, IFilesystem filesystem, IFolder folder)
        {
            if (s_fileBrowser.ShowDialog(window) != DialogResult.OK)
                return false;

            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            await Task.Run(() => view.ViewModel.StartAdd(folder, s_fileBrowser.FileNames));
            return true; 
        }

        public static Task Add(IWin32Window window, Overlay overlay, IFilesystem filesystem, IFolder folder, string[] files)
        {
            var view = new FileOperationView(filesystem);
            overlay.Show(view);
            return Task.Run(() => view.ViewModel.StartAdd(folder, files));
        }
    }
}
