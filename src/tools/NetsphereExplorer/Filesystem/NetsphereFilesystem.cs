using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Netsphere.Resource;
using ReactiveUI;

namespace NetsphereExplorer.Filesystem
{
    internal class NetsphereFilesystem : ReactiveObject, IFilesystem
    {
        private readonly ReactiveList<IFolder> _folders;
        private readonly ReactiveList<IFile> _files;
        private S4Zip _zip;
        private bool _isOpen;

        public char Separator => '/';
        public IReadOnlyReactiveList<IFolder> Folders => _folders;
        public IReadOnlyReactiveList<IFile> Files => _files;
        public IObservable<IFilesystem> FileOrFolderChanged { get; }
        public bool IsOpen
        {
            get { return _isOpen; }
            private set { this.RaiseAndSetIfChanged(ref _isOpen, value); }
        }

        public S4Zip Zip
        {
            get { return _zip; }
            private set { this.RaiseAndSetIfChanged(ref _zip, value); }
        }

        public NetsphereFilesystem()
        {
            _folders = new ReactiveList<IFolder>();
            _files = new ReactiveList<IFile>();
            // TODO File/Folder changes like name or size
            FileOrFolderChanged = this.WhenAnyValue(x => x.Folders.Count, x => x.Files.Count, x => x.Zip)
                .Select(_ => this);
        }

        public void Open(string fileName)
        {
            _folders.Clear();
            _files.Clear();
            Zip = S4Zip.OpenZip(fileName);

            foreach (var entry in Zip.Values)
                Add(entry);

            IsOpen = true;
        }

        public IFolder CreateFolder(string name)
        {
            if (Folders.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Folder already exists", nameof(name));

            var folder = new NetsphereFolder(name, this);
            _folders.Add(folder);
            return folder;
        }

        public IFile CreateFile(string name, byte[] data)
        {
            if (Files.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("File already exists", nameof(name));

            var entry = Zip.CreateEntry(name, data);
            var file = new NetsphereFile(entry, this);
            _files.Add(file);
            return file;
        }

        public void Save()
        {
            Zip.Save();
        }

        private void Add(S4ZipEntry entry)
        {
            NetsphereFolder parent = null;
            var startIndex = 0;
            while (true)
            {
                if (startIndex >= entry.FullName.Length)
                    return;

                var index = entry.FullName.IndexOf("/", startIndex, StringComparison.InvariantCulture);
                var isFile = index == -1;
                if (isFile)
                    index = entry.FullName.Length;

                var name = entry.FullName.Substring(startIndex, index - startIndex);
                if (isFile)
                {
                    var file = new NetsphereFile(entry, parent);
                    if (parent == null)
                        _files.Add(file);
                    else
                        parent.CreateFile(entry);
                    return;
                }

                IEnumerable<IFolder> folders = parent != null ? parent.Folders : Folders;
                var folder = folders.FirstOrDefault(x => x.Name == name) ??
                    (parent == null ? CreateFolder(name) : parent.CreateFolder(name));
                parent = (NetsphereFolder) folder;
                startIndex = index + 1;
            }
        }
    }

    internal class NetsphereFolder : ReactiveObject, IFolder
    {
        private readonly ReactiveList<IFolder> _folders;
        private readonly ReactiveList<IFile> _files;

        public IFilesystem Filesystem { get; }
        public IReadOnlyReactiveList<IFolder> Folders => _folders;
        public IReadOnlyReactiveList<IFile> Files => _files;
        public IObservable<IFolder> FileOrFolderChanged { get; }
        public string Name { get; }
        public string FullName => GetFullName();
        public IFolder Parent { get; }

        public NetsphereFolder(string name, IFolder parent)
        {
            _folders = new ReactiveList<IFolder>();
            _files = new ReactiveList<IFile>();

            // TODO File/Folder changes like name or size
            FileOrFolderChanged = this.WhenAnyValue(x => x.Folders.Count, x => x.Files.Count)
                .Select(_ => this);

            Filesystem = parent.Filesystem;
            Name = name;
            Parent = parent;
        }

        public NetsphereFolder(string name, IFilesystem filesystem)
        {
            _folders = new ReactiveList<IFolder>();
            _files = new ReactiveList<IFile>();

            // TODO File/Folder changes like name or size
            FileOrFolderChanged = this.WhenAnyValue(x => x.Folders.Count, x => x.Files.Count)
                .Select(_ => this);

            Filesystem = filesystem;
            Name = name;
            Parent = null;
        }

        public IFolder CreateFolder(string name)
        {
            if (Folders.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Folder already exists", nameof(name));

            var folder = new NetsphereFolder(name, this);
            _folders.Add(folder);
            return folder;
        }

        public IFile CreateFile(string name, byte[] data)
        {
            if (Files.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("File already exists", nameof(name));

            var fs = (NetsphereFilesystem)Filesystem;
            var entry = fs.Zip.CreateEntry(FullName + name, data);
            var file = new NetsphereFile(entry, this);
            _files.Add(file);
            return file;
        }

        internal IFile CreateFile(S4ZipEntry entry)
        {
            if (Files.Any(x => x.Name.Equals(entry.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("File already exists", nameof(entry));

            var file = new NetsphereFile(entry, this);
            _files.Add(file);
            return file;
        }

        public void Delete()
        {
            var files = Files.ToArray();
            foreach (var file in files)
                file.Delete();
            _files.Clear();

            var folders = Folders.ToArray();
            foreach (var folder in folders)
                folder.Delete();

            ((NetsphereFolder)Parent)?._folders.Remove(this);
        }

        public void Delete(IFile file)
        {
            _files.Remove(file);
        }

        private string GetFullName()
        {
            IFolder folder = this;
            var sb = new StringBuilder();
            while (folder != null)
            {
                if (folder.Name.Last() != Filesystem.Separator)
                {
                    sb.Insert(0, folder.Name);
                    sb.Insert(folder.Name.Length, Filesystem.Separator);
                }
                folder = folder.Parent;
            }
            return sb.ToString();
        }
    }

    internal class NetsphereFile : ReactiveObject, IFile
    {
        private S4ZipEntry _entry;
        private int _length;
        private string _name;

        public IFilesystem Filesystem { get; }
        public string Name
        {
            get { return _name; }
            private set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public string FullName => Parent.FullName + Name;
        public IFolder Parent { get; }
        public int Length
        {
            get { return _length; }
            private set { this.RaiseAndSetIfChanged(ref _length, value); }
        }

        public NetsphereFile(S4ZipEntry entry, IFolder parent)
        {
            Filesystem = parent.Filesystem;
            Parent = parent;
            Initialize(entry);
        }

        public NetsphereFile(S4ZipEntry entry, IFilesystem filesystem)
        {
            Filesystem = filesystem;
            Initialize(entry);
        }

        public void Delete()
        {
            _entry.Remove(true);
            Parent.Delete(this);
        }

        public void SetData(byte[] data)
        {
            _entry.SetData(data);
            Length = data.Length;
        }

        public byte[] GetData()
        {
            return _entry.GetData();
        }

        private void Initialize(S4ZipEntry entry)
        {
            _entry = entry;
            Name = entry.Name;
            Length = entry.Length;
            if (Name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ||
                Name.EndsWith(".x7", StringComparison.OrdinalIgnoreCase))
                Length = entry.GetData().Length;
        }
    }
}
