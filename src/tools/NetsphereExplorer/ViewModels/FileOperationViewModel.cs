using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WindowsAPICodePack.Core.Dialogs.TaskDialogs;
using BlubLib.Threading.Tasks;
using NetsphereExplorer.Filesystem;
using ReactiveUI;

namespace NetsphereExplorer.ViewModels
{
    internal class FileOperationViewModel : ReactiveObject
    {
        private readonly AsyncAutoResetEvent _resumeEvent = new AsyncAutoResetEvent();
        private readonly IFilesystem _filesystem;
        private string _destination;
        private OperationMode _mode;
        private OperationStatus _status;
        private int _fileCount;
        private int _progress;

        public ReactiveCommand<Unit> PauseOrResume { get; }
        public ReactiveCommand<Unit> Cancel { get; }

        public string Destination
        {
            get { return _destination; }
            set { this.RaiseAndSetIfChanged(ref _destination, value); }
        }

        public OperationMode Mode
        {
            get { return _mode; }
            set { this.RaiseAndSetIfChanged(ref _mode, value); }
        }

        public OperationStatus Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        public int FileCount
        {
            get { return _fileCount; }
            set { this.RaiseAndSetIfChanged(ref _fileCount, value); }
        }

        public int Progress
        {
            get { return _progress; }
            set { this.RaiseAndSetIfChanged(ref _progress, value); }
        }

        public FileOperationViewModel(IFilesystem filesystem)
        {
            _filesystem = filesystem;
#pragma warning disable 1998
            PauseOrResume = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (Status == OperationStatus.Paused)
                    _resumeEvent.Set();
                else
                    Status = OperationStatus.Paused;
            });
            Cancel = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                Status = OperationStatus.Cancel;
                _resumeEvent.Set();
            });
#pragma warning restore 1998
        }

        public async Task StartExtract(IFolder folder, string destination)
        {
            Destination = destination;
            Mode = OperationMode.Copy;
            Status = OperationStatus.Discovery;
            FileCount = 0;
            Progress = 0;

            await Discovery(folder);
            if (Status == OperationStatus.Cancel)
                return;

            Status = OperationStatus.Work;
            await Extract(folder, Destination);
        }

        public async Task StartExtract(object[] items, string destination)
        {
            Destination = destination;
            Mode = OperationMode.Copy;
            Status = OperationStatus.Discovery;
            FileCount = 0;
            Progress = 0;

            await Discovery(items);
            if (Status == OperationStatus.Cancel)
                return;

            Status = OperationStatus.Work;
            await Extract(items, Destination);
        }

        public async Task StartDelete(IFolder folder)
        {
            Destination = null;
            Mode = OperationMode.Delete;
            Status = OperationStatus.Discovery;
            FileCount = 0;
            Progress = 0;

            await Discovery(folder);
            if (Status == OperationStatus.Cancel)
                return;

            Status = OperationStatus.Work;
            try
            {
                await Delete(folder);
            }
            finally
            {
                _filesystem.Save();
            }
        }

        public async Task StartDelete(object[] items)
        {
            Destination = null;
            Mode = OperationMode.Delete;
            Status = OperationStatus.Discovery;
            FileCount = 0;
            Progress = 0;

            await Discovery(items);
            if (Status == OperationStatus.Cancel)
                return;

            Status = OperationStatus.Work;
            try
            {
                await Delete(items);
            }
            finally
            {
                _filesystem.Save();
            }
        }

        public async Task StartAdd(IFolder folder, string[] files)
        {
            Destination = null;
            Mode = OperationMode.Add;
            Status = OperationStatus.Discovery;
            FileCount = 0;
            Progress = 0;

            await Discovery(files);
            if (Status == OperationStatus.Cancel)
                return;
            Status = OperationStatus.Work;
            try
            {
                await Add(folder, files);
            }
            finally
            {
                _filesystem.Save();
            }
        }

        private async Task Discovery(IFolder folder)
        {
            FileCount += folder.Files.Count;

            if (Status == OperationStatus.Cancel)
                return;

            if (Status == OperationStatus.Paused)
            {
                await _resumeEvent.WaitAsync();
                if (Status == OperationStatus.Cancel)
                    return;
                Status = OperationStatus.Discovery;
            }

            foreach (var f in folder.Folders)
                await Discovery(f);
        }

        private async Task Discovery(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Discovery;
                }

                var file = item as IFile;
                if (file != null)
                    FileCount++;
                else
                    await Discovery((IFolder)item);
            }
        }

        private async Task Discovery(string[] files)
        {
            foreach (var file in files)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Discovery;
                }

                if (Directory.Exists(file))
                    FileCount += Directory.EnumerateFiles(file, "*", SearchOption.AllDirectories).Count();
                else
                    FileCount++;
            }
        }
        private async Task Extract(IFolder folder, string path)
        {
            path = Path.Combine(path, folder.Name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var file in folder.Files)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Work;
                }

                File.WriteAllBytes(Path.Combine(path, file.Name), file.GetData());
                ++Progress;
            }

            foreach (var f in folder.Folders)
            {
                if (Status == OperationStatus.Cancel)
                    return;
                await Extract(f, path);
            }
        }

        private async Task Extract(IEnumerable<object> items, string path)
        {
            foreach (var item in items)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Work;
                }

                var file = item as IFile;
                if (file != null)
                {
                    File.WriteAllBytes(Path.Combine(path, file.Name), file.GetData());
                    ++Progress;
                }
                else
                {
                    await Extract((IFolder)item, path);
                }
            }
        }

        private async Task Delete(IFolder folder)
        {
            var files = folder.Files.ToArray();
            foreach (var file in files)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Work;
                }

                file.Delete();
                ++Progress;
            }

            var folders = folder.Folders.ToArray();
            foreach (var f in folders)
            {
                if (Status == OperationStatus.Cancel)
                    return;
                await Delete(f);
            }
            folder.Delete();
        }

        private async Task Delete(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Work;
                }

                var file = item as IFile;
                if (file != null)
                {
                    file.Delete();
                    ++Progress;
                }
                else
                {
                    await Delete((IFolder)item);
                }
            }
        }

        private async Task Add(IFolder folder, IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (Status == OperationStatus.Cancel)
                    return;

                if (Status == OperationStatus.Paused)
                {
                    await _resumeEvent.WaitAsync();
                    if (Status == OperationStatus.Cancel)
                        return;
                    Status = OperationStatus.Work;
                }

                var di = new DirectoryInfo(file);
                if (di.Exists)
                {
                    var subFolder = folder.Folders.FirstOrDefault(x => x.Name.Equals(di.Name, StringComparison.OrdinalIgnoreCase)) ??
                                    folder.CreateFolder(di.Name);

                    var subFiles = Directory.EnumerateDirectories(di.FullName, "*", SearchOption.TopDirectoryOnly)
                        .Concat(Directory.EnumerateFiles(di.FullName, "*", SearchOption.TopDirectoryOnly));
                    await Add(subFolder, subFiles);
                    continue;
                }

                var fileName = Path.GetFileName(file);
                var data = File.ReadAllBytes(file);
                var oldFile = folder.Files.FirstOrDefault(x => x.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (oldFile == null)
                {
                    folder.CreateFile(fileName, data);
                }
                else
                {
                    // TODO Need get window somehow - Time to use IoC?
                    // TODO "Yes All" and "No All" would be cool
                    var result = await Observable.Start(() => TaskDialog.Show(Program.Window,
                                $"{oldFile.Parent.FullName + oldFile.Name} already exists. Do you want to replace it?",
                                buttons: TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No),
                            RxApp.MainThreadScheduler)
                        .ToTask();
                    if (result == TaskDialogResult.Yes)
                        oldFile.SetData(data);
                }

                ++Progress;
            }
        }

    }

    internal enum OperationMode
    {
        Copy,
        Delete,
        Add
    }

    internal enum OperationStatus
    {
        Discovery,
        Paused,
        Cancel,
        Work
    }
}
