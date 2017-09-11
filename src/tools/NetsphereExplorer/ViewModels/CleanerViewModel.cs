using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using WindowsAPICodePack.Core.Dialogs.TaskDialogs;
using BlubLib;
using Netsphere.Resource;
using ReactiveUI;

namespace NetsphereExplorer.ViewModels
{
    internal class CleanerViewModel : ReactiveObject
    {
        private readonly S4Zip _zip;
        private readonly string _tempDir;
        private int _fileCount;
        private int _progress;
        private CleanerStatus _status;

        public int FileCount
        {
            get => _fileCount;
            set => this.RaiseAndSetIfChanged(ref _fileCount, value);
        }

        public int Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public CleanerStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public CleanerViewModel(S4Zip zip)
        {
            _zip = zip;
            var tempDir = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            _tempDir = Path.Combine(Path.GetDirectoryName(zip.ZipPath), tempDir);
        }

        public Task Start()
        {
            Prepare();
            Work();
            return Cleanup();
        }

        private void Prepare()
        {
            Status = CleanerStatus.Preparing;
            FileCount = _zip.Count;

            Directory.Move(_zip.ResourcePath, _tempDir);
            if (!Directory.Exists(_zip.ResourcePath))
                Directory.CreateDirectory(_zip.ResourcePath);
        }

        private void Work()
        {
            Status = CleanerStatus.Work;
            foreach (var entry in _zip.Values)
            {
                var fileName = Path.GetFileName(entry.FileName);
                var temp = Path.Combine(_tempDir, fileName);
                File.Move(temp, entry.FileName);
                ++Progress;
            }
        }

        private async Task Cleanup()
        {
            Status = CleanerStatus.Cleanup;
            long totalLength = 0;
            var junkCount = 0;
            foreach (var file in Directory.EnumerateFiles(_tempDir))
            {
                totalLength += new FileInfo(file).Length;
                ++junkCount;
            }

            var result = await Observable.Start(() => TaskDialog.Show(Program.Window,
                        $"{junkCount} unused files were found({totalLength.ToFormattedSize()}). Do you want to delete them?",
                        buttons: TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No),
                    RxApp.MainThreadScheduler)
                .ToTask();
            if (result == TaskDialogResult.Yes)
                Directory.Delete(_tempDir, true);
            else
                Process.Start(_tempDir);
        }
    }

    internal enum CleanerStatus
    {
        Preparing,
        Work,
        Cleanup
    }
}
