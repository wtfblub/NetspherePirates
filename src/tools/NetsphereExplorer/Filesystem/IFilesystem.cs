using System;
using ReactiveUI;

namespace NetsphereExplorer.Filesystem
{
    internal interface IFilesystem : IReactiveObject
    {
        char Separator { get; }
        IReadOnlyReactiveList<IFolder> Folders { get; }
        IReadOnlyReactiveList<IFile> Files { get; }
        IObservable<IFilesystem> FileOrFolderChanged { get; }
        bool IsOpen { get; }

        IFolder CreateFolder(string name);
        IFile CreateFile(string name, byte[] data);

        void Save();
    }

    internal interface IFolder : IReactiveObject
    {
        IFilesystem Filesystem { get; }
        IReadOnlyReactiveList<IFolder> Folders { get; }
        IReadOnlyReactiveList<IFile> Files { get; }
        IObservable<IFolder> FileOrFolderChanged { get; }

        string Name { get; }
        string FullName { get; }
        IFolder Parent { get; }

        IFolder CreateFolder(string name);
        IFile CreateFile(string name, byte[] data);

        void Delete();
        void Delete(IFile file);
    }

    internal interface IFile : IReactiveObject
    {
        IFilesystem Filesystem { get; }
        string Name { get; }
        string FullName { get; }
        IFolder Parent { get; }

        int Length { get; }

        void Delete();
        void SetData(byte[] data);
        byte[] GetData();
    }
}
