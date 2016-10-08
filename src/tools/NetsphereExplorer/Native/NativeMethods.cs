using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NetsphereExplorer.Native
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        // ReSharper disable InconsistentNaming
        public const int MAX_PATH = 260;
        public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        public const uint SHGFI_SMALLICON = 0x000000001;
        public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint SHGFI_OPENICON = 0x000000002;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        // ReSharper restore InconsistentNaming

        // ReSharper disable once InconsistentNaming
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public static Icon GetFolderIcon(bool smallSize = true)
        {
            var shfi = new SHFILEINFO();
            var flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;

            flags += SHGFI_OPENICON;
            if (smallSize)
                flags += SHGFI_SMALLICON;
            else
                flags += SHGFI_LARGEICON;

            var str = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            SHGetFileInfo(str, FILE_ATTRIBUTE_DIRECTORY, ref shfi, (uint)Marshal.SizeOf(shfi), flags);

            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }

        public static Icon GetFileIcon(string name, bool smallSize = true)
        {
            var shfi = new SHFILEINFO();
            var flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;

            if (smallSize)
                flags += SHGFI_SMALLICON;
            else
                flags += SHGFI_LARGEICON;

            SHGetFileInfo(name, FILE_ATTRIBUTE_NORMAL, ref shfi, (uint)Marshal.SizeOf(shfi), flags);

            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }
    }

    // ReSharper disable once InconsistentNaming
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SIZEL
    {
        public int cx;
        public int cy;

        public SIZEL(int cx, int cy)
        {
            this.cx = cx; this.cy = cy;
        }

        public SIZEL(SIZEL o)
        {
            cx = o.cx; cy = o.cy;
        }
    }

    // ReSharper disable once InconsistentNaming
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct POINTL
    {
        public int cx;
        public int cy;

        public POINTL(int cx, int cy)
        {
            this.cx = cx; this.cy = cy;
        }

        public POINTL(SIZEL o)
        {
            cx = o.cx; cy = o.cy;
        }
    }
}
