using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPICodePack.Core.Dialogs.TaskDialogs;
using BlubLib;
using BlubLib.IO;
using NetsphereExplorer.Controls;
using NetsphereExplorer.Filesystem;
using NetsphereExplorer.Native;
using NetsphereExplorer.Views;
using ReactiveUI;

namespace NetsphereExplorer.ViewModels
{
    internal class MainViewModel : ReactiveObject
    {
        private readonly NetsphereFilesystem _filesystem;
        private readonly IWin32Window _window;
        private readonly Overlay _overlay;
        private readonly FileDialog _openResDialog;
        private TreeNode _selectedFolderNode;
        private readonly ObservableAsPropertyHelper<IFolder> _currentFolder;
        private ListViewItem[] _selectedItems;
        private TreeNode _rootFolderNode;
        private IDisposable _folderUpdate;
        private ListViewItem[] _listViewItems;

        public ReactiveCommand<Unit> Open { get; }
        public ReactiveCommand<Unit> OpenItem { get; }
        public ReactiveCommand<Unit> ExtractFolderTo { get; }
        public ReactiveCommand<Unit> AddFiles { get; }
        public ReactiveCommand<Unit> RemoveFolder { get; }
        public ReactiveCommand<Unit> ExtractItems { get; }
        public ReactiveCommand<Unit> RemoveItems { get; }
        public ReactiveCommand<Unit> Cleaner { get; }

        public ReactiveCommand<Unit> DragEnter { get; }
        public ReactiveCommand<Unit> DragDrop { get; }
        public ReactiveCommand<Unit> DragEnterExplorer { get; }
        public ReactiveCommand<Unit> DragDropExplorer { get; }
        public ReactiveCommand<Unit> DragFolder { get; }

        public TreeNode RootFolderNode
        {
            get => _rootFolderNode;
            set => this.RaiseAndSetIfChanged(ref _rootFolderNode, value);
        }
        public TreeNode SelectedFolderNode
        {
            get => _selectedFolderNode;
            set => this.RaiseAndSetIfChanged(ref _selectedFolderNode, value);
        }
        public IFolder CurrentFolder => _currentFolder.Value;
        public ListViewItem[] ListViewItems
        {
            get => _listViewItems;
            private set => this.RaiseAndSetIfChanged(ref _listViewItems, value);
        }
        public ListViewItem[] SelectedItems
        {
            get => _selectedItems;
            set => this.RaiseAndSetIfChanged(ref _selectedItems, value);
        }

        public MainViewModel(IWin32Window window, Overlay overlay)
        {
            _filesystem = new NetsphereFilesystem();
            _window = window;
            _overlay = overlay;
            _openResDialog = new OpenFileDialog
            {
                FileName = "resource.s4hd",
                Filter = "*.s4hd|"
            };
            _listViewItems = Array.Empty<ListViewItem>();

            Open = ReactiveCommand.CreateAsyncTask(_ => OpenImpl());

            var canOpenItem = this.WhenAnyValue(x => x.SelectedItems)
                .Select(x => x != null && x.Length > 0);
            OpenItem = ReactiveCommand.CreateAsyncTask(canOpenItem, _ => OpenItemImpl());
            ExtractFolderTo = ReactiveCommand.CreateAsyncTask(_ => ExtractFolderToImpl());
            AddFiles = ReactiveCommand.CreateAsyncTask(_ => AddFilesImpl());
            RemoveFolder = ReactiveCommand.CreateAsyncTask(_ => RemoveFolderImpl());

            var hasItems = this.WhenAnyValue(x => x.SelectedItems)
                .Select(items => items?.All(item => item.Tag != null) ?? false);
            ExtractItems = ReactiveCommand.CreateAsyncTask(hasItems, _ => ExtractItemsImpl());
            RemoveItems = ReactiveCommand.CreateAsyncTask(hasItems, _ => RemoveItemsImpl());

            var canClean = _filesystem.WhenAnyValue(x => x.IsOpen);
            Cleaner = ReactiveCommand.CreateAsyncTask(canClean, _ => CleanerImpl());

            DragEnter = ReactiveCommand.CreateAsyncTask(DragEnterImpl);
            DragDrop = ReactiveCommand.CreateAsyncTask(DragDropImpl);
            DragEnterExplorer = ReactiveCommand.CreateAsyncTask(DragEnterExplorerImpl);
            DragDropExplorer = ReactiveCommand.CreateAsyncTask(DragDropExplorerImpl);
            DragFolder = ReactiveCommand.CreateAsyncTask(DragFolderImpl);

            _currentFolder = this.WhenAnyValue(x => x.SelectedFolderNode)
                .Select(x => x?.Tag as IFolder)
                .ToProperty(this, x => x.CurrentFolder);

            this.WhenAnyValue(vm => vm.CurrentFolder, vm => vm.SelectedFolderNode)
                .Subscribe(tuple =>
                {
                    _folderUpdate?.Dispose();
                    _folderUpdate = tuple.Item1?.FileOrFolderChanged
                        .Where(_ => !_overlay.IsShowing)
                        .Subscribe(_ => UpdateListViewItems());
                    UpdateListViewItems();
                });

            this.WhenAnyValue(x => x._filesystem.FileOrFolderChanged)
                .Subscribe(_ =>
                {
                    if (!_filesystem.IsOpen)
                    {
                        RootFolderNode = null;
                        SelectedFolderNode = null;
                    }
                });
        }

        private void UpdateListViewItems()
        {
            if (CurrentFolder != null || _filesystem.IsOpen)
            {
                var folders = CurrentFolder?.Folders ?? _filesystem.Folders;
                var files = CurrentFolder?.Files ?? _filesystem.Files;
                var items = CurrentFolder == null
                    ? Enumerable.Empty<ListViewItem>()
                    : Enumerable.Repeat(new ListViewItem("..") { ImageKey = "folder" }, 1);
                items = items
                    .Concat(folders.Select(f => new ListViewItem(f.Name) { Tag = f, ImageKey = "folder" }))
                    .Concat(files.Select(f =>
                    {
                        var item = new ListViewItem(f.Name) { Tag = f, ImageKey = Path.GetExtension(f.Name) };
                        item.SubItems.Add(f.Length.ToFormattedSize());
                        return item;
                    }));
                ListViewItems = items.ToArray();
            }
            else
            {
                ListViewItems = Array.Empty<ListViewItem>();
            }
        }

        private async Task OpenZip(string fileName)
        {
            var view = new SimpleProgressView { Message = "Opening..." };
            _overlay.Show(view);

            try
            {
                await Task.Run(() => _filesystem.Open(fileName));
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, "Failed to open resource folder", ex);
                _overlay.Hide();
                return;
            }

            RootFolderNode = CreateFolderTree();
            _overlay.Hide();
        }

        private async Task OpenImpl()
        {
            if (_openResDialog.ShowDialog(_window) != DialogResult.OK)
                return;

            await OpenZip(_openResDialog.FileName);
        }

        private TreeNode CreateFolderTree()
        {
            var root = new TreeNode("/");
            foreach (var folder in _filesystem.Folders)
                CreateFolderTree(folder, root);
            return root;
        }

        private static void CreateFolderTree(IFolder folder, TreeNode root)
        {
            var node = new TreeNode(folder.Name) { Name = folder.FullName, Tag = folder, ImageKey = "folder" };
            root.Nodes.Add(node);
            foreach (var f in folder.Folders)
                CreateFolderTree(f, node);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task OpenItemImpl()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var item = SelectedItems.FirstOrDefault();
            if (item == null)
                return;

            if (item.Tag == null)
            {
                SelectedFolderNode = SelectedFolderNode.Parent;
            }
            else
            {
                if (item.Tag is IFolder folder)
                {
                    var newNode = SelectedFolderNode.Nodes
                        .Cast<TreeNode>()
                        .FirstOrDefault(node => node.Text.Equals(folder.Name, StringComparison.OrdinalIgnoreCase));
                    if (newNode != null)
                        SelectedFolderNode = newNode;
                }
                else
                {
                    var file = (IFile)item.Tag;
                    // ToDo handle file
                }
            }
        }

        private async Task ExtractFolderToImpl()
        {
            if (CurrentFolder == null)
                return;

            try
            {
                await FileOperationView.Extract(_window, _overlay, _filesystem, CurrentFolder);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, $"Failed to extract folder {CurrentFolder.FullName}", ex);
            }
            _overlay.Hide();
        }

        private async Task AddFilesImpl()
        {
            if (CurrentFolder == null)
                return;

            try
            {
                var result = await FileOperationView.Add(_window, _overlay, _filesystem, CurrentFolder);
                if (!result)
                    return;
                UpdateListViewItems();
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, $"Failed to add some files to {CurrentFolder.FullName}", ex);
            }
            _overlay.Hide();
        }

        private async Task RemoveFolderImpl()
        {
            if (CurrentFolder == null)
                return;

            try
            {
                var result = await FileOperationView.Delete(_window, _overlay, _filesystem, CurrentFolder);
                if (!result)
                    return;
                var tv = SelectedFolderNode.TreeView;
                var node = tv.Nodes.Find(CurrentFolder.FullName, true).FirstOrDefault();
                node?.Parent.Nodes.Remove(node);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, $"Failed to remove folder {CurrentFolder.FullName}", ex);
            }
            _overlay.Hide();
        }

        private async Task ExtractItemsImpl()
        {
            var items = SelectedItems.Where(item => item.Tag != null).Select(item => item.Tag).ToArray();
            if (items.Length == 0)
                return;

            try
            {
                await FileOperationView.Extract(_window, _overlay, _filesystem, items);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, $"Failed to extract {items.Length} items", ex);
            }
            _overlay.Hide();
        }

        private async Task RemoveItemsImpl()
        {
            var items = SelectedItems.Where(item => item.Tag != null).Select(item => item.Tag).ToArray();
            if (items.Length == 0)
                return;

            try
            {
                var result = await FileOperationView.Delete(_window, _overlay, _filesystem, items);
                if (!result)
                    return;

                foreach (var folder in items.OfType<IFolder>())
                {
                    var node = SelectedFolderNode.Nodes
                        .Cast<TreeNode>()
                        .FirstOrDefault(x => x.Text.Equals(folder.Name, StringComparison.OrdinalIgnoreCase));
                    if (node != null)
                        SelectedFolderNode.Nodes.Remove(node);
                }
                UpdateListViewItems();
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, "Failed to delete some items", ex);
            }
            _overlay.Hide();
        }

        private async Task CleanerImpl()
        {
            try
            {
                await CleanerView.Start(_window, _overlay, _filesystem.Zip);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, "Something went wrong", ex);
            }
            _overlay.Hide();
        }

        private static bool IsDragNetsphereZip(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return false;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            return files.Length == 1 && files[0].EndsWith(".s4hd", StringComparison.OrdinalIgnoreCase);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async Task DragEnterImpl(object obj)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var e = (DragEventArgs)obj;
            e.Effect = IsDragNetsphereZip(e) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private async Task DragDropImpl(object obj)
        {
            var e = (DragEventArgs)obj;
            if (!IsDragNetsphereZip(e))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            await OpenZip(files[0]);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task DragEnterExplorerImpl(object obj)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var e = (DragEventArgs)obj;
            if (IsDragNetsphereZip(e))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            e.Effect = CurrentFolder != null && e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private async Task DragDropExplorerImpl(object obj)
        {
            var e = (DragEventArgs)obj;
            if (IsDragNetsphereZip(e))
            {
                await DragDropImpl(e);
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            files = files
                .Where(file => Directory.Exists(file) || File.Exists(file))
                .ToArray();

            try
            {
                var folder = CurrentFolder;
                await FileOperationView.Add(_window, _overlay, _filesystem, folder, files);
                UpdateListViewItems();
                RootFolderNode = CreateFolderTree();
                SelectedFolderNode = RootFolderNode.TreeView.Nodes.Find(folder.FullName, true).FirstOrDefault() ?? RootFolderNode;
            }
            catch (Exception ex)
            {
                TaskDialog.Show(_window, $"Failed to add some files to {CurrentFolder.FullName}", ex);
            }
            _overlay.Hide();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task DragFolderImpl(object obj)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var e = (ItemDragEventArgs)obj;
            var node = e.Item as TreeNode;

            Control control;
            IEnumerable<object> items;
            if (node != null)
            {
                control = node.TreeView;
                items = !(node.Tag is IFolder folder)
                    ? _filesystem.Folders.Cast<object>().Concat(_filesystem.Files)
                    : Enumerable.Repeat(folder, 1);
            }
            else
            {
                if (ListViewItems.Length == 0)
                    return;
                control = SelectedItems[0].ListView;
                items = SelectedItems.Where(x => x.Tag != null).Select(x => x.Tag);
                if (!items.Any())
                    return;
            }

            var fileInfos = ProcessDrag(items, "");
            VirtualFileDrag.DoDragAndDrop(control, fileInfos.ToArray());
        }

        private static IEnumerable<DragFileInfo> ProcessDrag(IEnumerable<object> items, string path)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case IFile file:
                        yield return new DragFileInfo(Path.Combine(path, file.Name), new DragStream(file));
                        break;

                    case IFolder folder:
                        var nestedItems = ProcessDrag(folder.Files.Concat<object>(folder.Folders), Path.Combine(path, folder.Name));
                        foreach (var nestedItem in nestedItems)
                            yield return nestedItem;
                        break;
                }
            }
        }

        private class DragStream : Stream
        {
            private readonly IFile _file;
            private MemoryStream _stream;
            private bool _isDisposed;

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => _file.Length;
            public override long Position { get; set; }

            public DragStream(IFile file)
            {
                _file = file;
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_isDisposed)
                    return 0;

                CreateStreamIfNeeded();

                var bytesRead = _stream.Read(buffer, offset, count);
                if (_stream.IsEOF())
                    Dispose();
                return bytesRead;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override void Close()
            {
                _isDisposed = true;
                _stream?.Dispose();
                _stream = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                base.Close();
            }

            private void CreateStreamIfNeeded()
            {
                if (_stream == null && !_isDisposed)
                    _stream = new MemoryStream(_file.GetData());
            }
        }
    }
}
