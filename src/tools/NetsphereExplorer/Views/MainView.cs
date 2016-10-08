using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using NetsphereExplorer.Native;
using NetsphereExplorer.ViewModels;
using ReactiveUI;

namespace NetsphereExplorer.Views
{
    internal partial class MainView : Form, IViewFor<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public MainView()
        {
            InitializeComponent();
            using (var icon = NativeMethods.GetFolderIcon())
                imageList.Images.Add("folder", icon.ToBitmap());
            lvExplorer.ContextMenu = cmExplorer;
            ViewModel = new MainViewModel(this, overlay);

            this.WhenActivated(d =>
            {
                d(this.BindCommand(ViewModel, vm => vm.Open, v => v.miOpen));
                d(this.BindCommand(ViewModel, vm => vm.ExtractFolderTo, v => v.miExtractFolder));
                d(this.BindCommand(ViewModel, vm => vm.AddFiles, v => v.miAddToFolder));
                d(this.BindCommand(ViewModel, vm => vm.RemoveFolder, v => v.miRemoveFolder));
                d(this.BindCommand(ViewModel, vm => vm.ExtractItems, v => v.miExtractItems));
                d(this.BindCommand(ViewModel, vm => vm.RemoveItems, v => v.miRemoveItems));
                d(this.BindCommand(ViewModel, vm => vm.AddFiles, v => v.miAddFiles));

                d(this.WhenAnyValue(x => x.ViewModel.RootFolderNode)
                    .Subscribe(node => { tvFolders.ContextMenu = node != null ? cmFolders : null; }));

                d(this.WhenAnyValue(x => x.ViewModel.SelectedItems)
                    .Subscribe(items => { lvExplorer.ContextMenu = items?.Length > 0 ? cmExplorerSelection : cmExplorer; }));

                d(this.WhenAnyValue(x => x.ViewModel.RootFolderNode)
                    .Subscribe(node =>
                    {
                        tvFolders.SuspendLayout();
                        tvFolders.Nodes.Clear();
                        if (node != null)
                            tvFolders.Nodes.Add(node);
                        tvFolders.ResumeLayout();

                        tvFolders.SelectedNode = node;
                    }));

                d(tvFolders.Events()
                    .AfterSelect
                    .Select(e => e.Node)
                    .BindTo(ViewModel, vm => vm.SelectedFolderNode));

                d(this.WhenAnyValue(x => x.ViewModel.SelectedFolderNode)
                    .BindTo(tvFolders, x => x.SelectedNode));

                d(lvExplorer.Events()
                    .SelectedIndexChanged
                    .Throttle(TimeSpan.FromMilliseconds(1))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Select(e => lvExplorer.SelectedItems.Cast<ListViewItem>().ToArray())
                    .BindTo(ViewModel, vm => vm.SelectedItems));

                d(lvExplorer.Events()
                    .DoubleClick
                    .InvokeCommand(ViewModel, vm => vm.OpenItem));

                d(this.WhenAnyValue(x => x.ViewModel.ListViewItems)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(UpdateExplorer));

                d(lvExplorer.Events()
                    .KeyUp
                    .Where(e =>
                    {
                        if (e.KeyCode != Keys.Delete)
                            return false;
                        e.Handled = true;
                        return true;
                    })
                    .InvokeCommand(ViewModel, vm => vm.RemoveItems));

                d(tvFolders.Events()
                    .KeyUp
                    .Where(e =>
                    {
                        if (e.KeyCode != Keys.Delete)
                            return false;
                        e.Handled = true;
                        return true;
                    })
                    .InvokeCommand(ViewModel, vm => vm.RemoveFolder));

                d(this.Events()
                    .DragEnter
                    .InvokeCommand(ViewModel, vm => vm.DragEnter));

                d(this.Events()
                    .DragDrop
                    .InvokeCommand(ViewModel, vm => vm.DragDrop));

                d(lvExplorer.Events()
                    .DragEnter
                    .InvokeCommand(ViewModel, vm => vm.DragEnterExplorer));

                d(lvExplorer.Events()
                    .DragDrop
                    .InvokeCommand(ViewModel, vm => vm.DragDropExplorer));

                d(tvFolders.Events()
                    .ItemDrag
                    .InvokeCommand(ViewModel, vm => vm.DragFolder));

                d(lvExplorer.Events()
                    .ItemDrag
                    .InvokeCommand(ViewModel, vm => vm.DragFolder));
            });
        }

        private void UpdateExplorer(ListViewItem[] _)
        {
            lvExplorer.SuspendLayout();
            lvExplorer.Items.Clear();
            foreach (var item in ViewModel.ListViewItems)
            {
                if (!string.IsNullOrWhiteSpace(item.ImageKey) &&
                    !item.ImageKey.Equals("folder", StringComparison.OrdinalIgnoreCase) &&
                    !imageList.Images.ContainsKey(item.ImageKey))
                {
                    using (var icon = NativeMethods.GetFileIcon(item.ImageKey))
                        imageList.Images.Add(item.ImageKey, icon.ToBitmap());
                }
                lvExplorer.Items.Add(item);
            }
            lvExplorer.ResumeLayout();
        }
    }
}
