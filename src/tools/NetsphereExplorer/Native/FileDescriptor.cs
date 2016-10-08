using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NetsphereExplorer.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct FileDescriptor
    {
        public FileDescriptorFlags dwFlags;
        public Guid clsid;
        public SIZEL sizel;
        public POINTL pointl;
        public FileAttributes dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeMethods.MAX_PATH)]
        public string cFileName;
    }
}
