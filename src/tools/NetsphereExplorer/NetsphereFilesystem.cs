//using System;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using System.Windows.Forms;
//using Netsphere.Resource;
//using ReactiveUI;

//namespace NetsphereExplorer
//{
//    internal class NetsphereFilesystem : ReactiveObject
//    {
//        public static NetsphereFilesystem Instance { get; } = new NetsphereFilesystem();

//        private bool _isOpen;
//        private Folder _root;

//        public S4Zip S4Zip { get; private set; }

//        public bool IsOpen
//        {
//            get { return _isOpen; }
//            private set { this.RaiseAndSetIfChanged(ref _isOpen, value); }
//        }

//        public Folder Root
//        {
//            get { return _root; }
//            private set { this.RaiseAndSetIfChanged(ref _root, value); }
//        }

//        public void Open(S4Zip zip)
//        {
//            S4Zip = zip;
//            IsOpen = true;

//            Root = new Folder("/", null);
//            foreach (var entry in zip.Values)
//                Root.Add(entry);
//        }

//        public void Close()
//        {
//            IsOpen = false;
//            Root = null;
//            S4Zip = null;
//        }
//    }

//    internal static class ObservableCollectionExtensions
//    {
//        public static void Add(this Folder @this, S4ZipEntry entry, int startIndex = 0)
//        {
//            if (startIndex >= entry.FullName.Length)
//                return;

//            var index = entry.FullName.IndexOf("/", startIndex, StringComparison.InvariantCulture);
//            var isFile = index == -1;
//            if (isFile)
//                index = entry.FullName.Length;

//            var name = entry.FullName.Substring(startIndex, index - startIndex);
//            if (isFile)
//            {
//                @this.Files.Add(new File(@this, entry));
//                return;
//            }

//            var folder = @this.Folders.FirstOrDefault(x => x.Name == name);
//            if (folder == null)
//            {
//                folder = new Folder(name, @this);
//                @this.Folders.Add(folder);
//            }
//            folder.Add(entry, index + 1);
//        }
//    }

//    internal class Folder : ReactiveObject
//    {
//        public string Name { get; }
//        public string FullName { get; }
//        public ReactiveList<Folder> Folders { get; }
//        public ReactiveList<File> Files { get; }
//        public Folder Parent { get; }
//        public TreeNode TreeNode { get; set; }
//        public int TotalFiles { get; set; }
//        public IObservable<Folder> FileOrFolderCountChanged { get; }

//        public Folder(string name, Folder parent)
//        {
//            Name = name;
//            Folders = new ReactiveList<Folder>();
//            Files = new ReactiveList<File>();
//            Parent = parent;

//            var folder = this;
//            var sb = new StringBuilder();
//            while (folder != null)
//            {
//                if (folder.Name.Last() != '/')
//                {
//                    sb.Insert(0, folder.Name);
//                    sb.Insert(folder.Name.Length, '/');
//                }
//                folder = folder.Parent;
//            }
//            FullName = sb.ToString();

//            Folders.Changed
//                .ObserveOn(RxApp.MainThreadScheduler)
//                .Subscribe(OnCollectionChanged);

//            Files.Changed
//                .ObserveOn(RxApp.MainThreadScheduler)
//                .Subscribe(OnCollectionChanged);

//            FileOrFolderCountChanged = this.WhenAnyValue(x => x.Folders.Count, x => x.Files.Count)
//                .Select(_ => this);
//        }

//        public void Delete()
//        {
//            var files = Files.ToArray();
//            foreach(var file in files)
//                file.Delete();
//            Files.Clear();

//            var folders = Folders.ToArray();
//            foreach (var folder in folders)
//                folder.Delete();

//            Observable.Start(() =>
//            {
//                Parent?.Folders.Remove(this);
//                TreeNode?.Parent?.Nodes.Remove(TreeNode);
//            }, RxApp.MainThreadScheduler).Wait();
//        }

//        public Folder CreateFolder(string name)
//        {
//            if(Folders.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
//                throw new ArgumentException("Folder already exists", nameof(name));

//            var folder = new Folder(name, this);
//            Folders.Add(folder);
//            return folder;
//        }

//        public File CreateFile(string name, byte[] data)
//        {
//            if (Files.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
//                throw new ArgumentException("File already exists", nameof(name));

//            var entry = NetsphereFilesystem.Instance.S4Zip.CreateEntry(FullName + name, data);
//            var file = new File(this, entry);
//            Files.Add(file);
//            return file;
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
//        {
//            if (Folders.Count == 0 && Files.Count == 0)
//                Parent?.Folders.Remove(this);
//        }
//    }

//    internal class File : ReactiveObject
//    {
//        public Folder Folder { get; }
//        public S4ZipEntry Entry { get; }
//        public string Name => Entry.Name;
//        public int Size { get; private set; }

//        public File(Folder folder, S4ZipEntry entry)
//        {
//            Folder = folder;
//            Entry = entry;
//            Size = entry.Length;
//            if (Name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ||
//                Name.EndsWith(".x7", StringComparison.OrdinalIgnoreCase))
//                Size = entry.GetData().Length;
//        }

//        public void Delete()
//        {
//            Entry.Remove(true);
//            Observable.Start(() => { Folder.Files.Remove(this); }, RxApp.MainThreadScheduler).Wait();
//        }

//        public void SetData(byte[] data)
//        {
//            Entry.SetData(data);
//            Size = data.Length;
//        }

//        public override string ToString()
//        {
//            return Name;
//        }
//    }
//}