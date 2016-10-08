using System.Runtime.InteropServices;

namespace NetsphereExplorer.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct FileGroupDescriptor
    {
        public uint cItems;
        //public FileDescriptor[] fgd;

        public FileGroupDescriptor(int cItems)
        {
            this.cItems = (uint) cItems;
        }
    }
}
