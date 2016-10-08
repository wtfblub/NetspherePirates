using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace NetsphereExplorer.Native
{
    internal static class VirtualFileDrag
    {
        public static void DoDragAndDrop(Control dragSource, IEnumerable<DragFileInfo> files)
        {
            var dataObject = new DataObjectEx();
            var fileDescriptors = new List<FileDescriptor>();
            var i = 0;
            foreach (var file in files)
            {
                dataObject.SetFileContents(file.Stream, i);
                fileDescriptors.Add(new FileDescriptor
                {
                    cFileName = file.FileName,
                    nFileSizeHigh = (uint)(file.Stream.Length >> 32),
                    nFileSizeLow = (uint)(file.Stream.Length & 0xFFFFFFFF),
                    dwFlags = FileDescriptorFlags.FileSize | FileDescriptorFlags.ProgressUi
                });
                ++i;
            }

            var fileGroupDescriptor = new FileGroupDescriptor(fileDescriptors.Count);
            var descriptorSize = Marshal.SizeOf(fileGroupDescriptor) +
                                 Marshal.SizeOf<FileDescriptor>() * fileDescriptors.Count;
            using (var descriptorStream = new MemoryStream(descriptorSize))
            {
                StructToArray(fileGroupDescriptor, descriptorStream);
                foreach (var descriptor in fileDescriptors)
                    StructToArray(descriptor, descriptorStream);
                dataObject.SetData("FileGroupDescriptorW", descriptorStream);

                dragSource.DoDragDrop(dataObject, DragDropEffects.Copy);
            }
        }

        private static void StructToArray(FileGroupDescriptor @struct, Stream stream)
        {
            stream.Write(BitConverter.GetBytes(@struct.cItems), 0, sizeof(int));
        }

        private static void StructToArray(FileDescriptor @struct, Stream stream)
        {
            var size = Marshal.SizeOf(@struct);
            var data = new byte[size];
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(@struct, handle.AddrOfPinnedObject(), true);
                stream.Write(data, 0, data.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        private class DataObjectEx : System.Runtime.InteropServices.ComTypes.IDataObject
        {
            // ReSharper disable once InconsistentNaming
            private static readonly short CF_FILECONTENTS = (short)DataFormats.GetFormat("FileContents").Id;

            private readonly DataObject _innerDataObject;
            private readonly System.Runtime.InteropServices.ComTypes.IDataObject _innerComDataObject;
            private readonly IList<FileContent> _fileContents = new List<FileContent>();

            public DataObjectEx()
            {
                _innerDataObject = new DataObject();
                _innerComDataObject = _innerDataObject;
            }

            #region System.Runtime.InteropServices.ComTypes.IDataObject

            void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                if (format.cfFormat == CF_FILECONTENTS &&
                    format.dwAspect.HasFlag(DVASPECT.DVASPECT_CONTENT) &&
                    format.tymed.HasFlag(TYMED.TYMED_ISTREAM))
                {
                    FileContent fileContent;
                    if (GetFileContent(format.lindex, out fileContent))
                    {
                        medium = new STGMEDIUM { tymed = TYMED.TYMED_ISTREAM };
                        var ptr = Marshal.GetComInterfaceForObject(fileContent.ComStream, typeof(IStream));
                        Marshal.Release(ptr);
                        medium.unionmember = ptr;
                        return;
                    }
                }

                _innerComDataObject.GetData(ref format, out medium);
            }

            void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere(ref FORMATETC format,
                ref STGMEDIUM medium)
            {
                if (format.cfFormat == CF_FILECONTENTS &&
                       format.dwAspect.HasFlag(DVASPECT.DVASPECT_CONTENT) &&
                       format.tymed.HasFlag(TYMED.TYMED_ISTREAM) &&
                       medium.tymed.HasFlag(TYMED.TYMED_ISTREAM))
                {
                    FileContent fileContent;
                    if (GetFileContent(format.lindex, out fileContent))
                    {
                        medium.tymed = TYMED.TYMED_ISTREAM;
                        var ptr = Marshal.GetComInterfaceForObject(fileContent.ComStream, typeof(IStream));
                        Marshal.Release(ptr);
                        medium.unionmember = ptr;
                        return;
                    }
                }

                _innerComDataObject.GetDataHere(ref format, ref medium);
            }

            int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData(ref FORMATETC format)
            {
                if (format.cfFormat == CF_FILECONTENTS &&
                    format.dwAspect.HasFlag(DVASPECT.DVASPECT_CONTENT) &&
                    format.tymed.HasFlag(TYMED.TYMED_ISTREAM))
                {
                    FileContent fileContent;
                    if (GetFileContent(format.lindex, out fileContent))
                        return 0;
                }
                return _innerComDataObject.QueryGetData(ref format);
            }

            int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn,
                out FORMATETC formatOut)
            {
                return _innerComDataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
            }

            void System.Runtime.InteropServices.ComTypes.IDataObject.SetData(ref FORMATETC formatIn,
                ref STGMEDIUM medium, bool release)
            {
                _innerComDataObject.SetData(ref formatIn, ref medium, release);
            }

            IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
            {
                var innerEnumerator = _innerComDataObject.EnumFormatEtc(direction);
                if (direction != DATADIR.DATADIR_GET)
                    return innerEnumerator;

                var formats = new List<FORMATETC>(5 + _fileContents.Count);
                var tmp = new FORMATETC[5];
                var fetched = new int[2];
                int result;
                do
                {
                    result = innerEnumerator.Next(5, tmp, fetched);
                    for (var i = 0; i < fetched[0]; ++i)
                        formats.Add(tmp[i]);
                } while (result == 0);

                formats.AddRange(_fileContents.Select(c => c.Format));
                return new FormatEnumerator(formats);
            }

            int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf,
                IAdviseSink adviseSink, out int connection)
            {
                throw new NotImplementedException();
                //return _innerComDataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
            }

            void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise(int connection)
            {
                throw new NotImplementedException();
                //_innerComDataObject.DUnadvise(connection);
            }

            int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
            {
                throw new NotImplementedException();
                //return _innerComDataObject.EnumDAdvise(out enumAdvise);
            }

            #endregion

            #region System.Windows.IDataObject

            public object GetData(string format)
            {
                return _innerDataObject.GetData(format);
            }

            public object GetData(Type format)
            {
                return _innerDataObject.GetData(format);
            }

            public object GetData(string format, bool autoConvert)
            {
                return _innerDataObject.GetData(format, autoConvert);
            }

            public bool GetDataPresent(string format)
            {
                return _innerDataObject.GetDataPresent(format);
            }

            public bool GetDataPresent(Type format)
            {
                return _innerDataObject.GetDataPresent(format);
            }

            public bool GetDataPresent(string format, bool autoConvert)
            {
                return _innerDataObject.GetDataPresent(format, autoConvert);
            }

            public string[] GetFormats()
            {
                return _innerDataObject.GetFormats();
            }

            public string[] GetFormats(bool autoConvert)
            {
                return _innerDataObject.GetFormats(autoConvert);
            }

            public void SetData(object data)
            {
                _innerDataObject.SetData(data);
            }

            public void SetData(string format, object data)
            {
                _innerDataObject.SetData(format, data);
            }

            public void SetData(Type format, object data)
            {
                _innerDataObject.SetData(format, data);
            }

            public void SetData(string format, object data, bool autoConvert)
            {
                _innerDataObject.SetData(format, autoConvert, data);
            }

            #endregion

            public void SetFileContents(Stream stream, int index)
            {
                if (_fileContents.Any(c => c.Format.lindex == index))
                    throw new ArgumentException("Index already exists", nameof(index));

                var format = new FORMATETC
                {
                    cfFormat = CF_FILECONTENTS,
                    ptd = IntPtr.Zero,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = index,
                    tymed = TYMED.TYMED_ISTREAM
                };
                _fileContents.Add(new FileContent(format, stream));
            }

            private bool GetFileContent(int index, out FileContent fileContent)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _fileContents.Count; ++i)
                {
                    var content = _fileContents[i];
                    if (content.Format.lindex == index)
                    {
                        fileContent = content;
                        return true;
                    }
                }
                fileContent = default(FileContent);
                return false;
            }

            private class FileContent
            {
                public FORMATETC Format { get; }
                public Stream Stream { get; }
                public IStream ComStream { get; }

                public FileContent(FORMATETC format, Stream stream)
                {
                    Format = format;
                    Stream = stream;
                    ComStream = new StreamWrapper(stream);
                }
            }

            private class StreamWrapper : IStream
            {
                // ReSharper disable InconsistentNaming
                private const int STGTY_STREAM = 2;
                private const int STGM_READ = 0x00000000;
                private const int STGM_WRITE = 0x00000001;
                private const int STGM_READWRITE = 0x00000002;
                // ReSharper restore InconsistentNaming

                public Stream BaseStream { get; }

                public StreamWrapper(Stream baseStream)
                {
                    BaseStream = baseStream;
                }

                public void Read(byte[] pv, int cb, IntPtr pcbRead)
                {
                    int bytesRead;
                    var totalBytesRead = 0;
                    while (totalBytesRead < cb && (bytesRead = BaseStream.Read(pv, 0, cb)) != 0)
                        totalBytesRead += bytesRead;

                    if (pcbRead != IntPtr.Zero)
                        Marshal.WriteInt32(pcbRead, totalBytesRead);

                    if (totalBytesRead < cb)
                        Marshal.ThrowExceptionForHR(1);
                }

                public void Write(byte[] pv, int cb, IntPtr pcbWritten)
                {
                    BaseStream.Write(pv, 0, cb);
                    if (pcbWritten != IntPtr.Zero)
                        Marshal.WriteInt32(pcbWritten, cb);
                }

                public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
                {
                    var position = BaseStream.Seek(dlibMove, (SeekOrigin)dwOrigin);
                    if (plibNewPosition != IntPtr.Zero)
                        Marshal.WriteInt64(plibNewPosition, position);
                }

                public void SetSize(long libNewSize)
                {
                    BaseStream.SetLength(libNewSize);
                }

                public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
                {
                    throw new NotSupportedException();
                }

                public void Commit(int grfCommitFlags)
                {
                    throw new NotSupportedException();
                }

                public void Revert()
                {
                    throw new NotSupportedException();
                }

                public void LockRegion(long libOffset, long cb, int dwLockType)
                {
                    throw new NotSupportedException();
                }

                public void UnlockRegion(long libOffset, long cb, int dwLockType)
                {
                    throw new NotSupportedException();
                }

                public void Stat(out STATSTG pstatstg, int grfStatFlag)
                {
                    pstatstg = new STATSTG
                    {
                        type = STGTY_STREAM,
                        cbSize = BaseStream.Length,
                        grfMode = 0
                    };

                    if (BaseStream.CanRead && BaseStream.CanWrite)
                        pstatstg.grfMode |= STGM_READWRITE;
                    else if (BaseStream.CanRead)
                        pstatstg.grfMode |= STGM_READ;
                    else if (BaseStream.CanWrite)
                        pstatstg.grfMode |= STGM_WRITE;
                    else
                        throw new IOException("Stream is closed");
                }

                public void Clone(out IStream ppstm)
                {
                    throw new NotSupportedException();
                }
            }

            private class FormatEnumerator : IEnumFORMATETC
            {
                private readonly IReadOnlyList<FORMATETC> _formats;
                private int _current;

                public FormatEnumerator(IReadOnlyList<FORMATETC> formats)
                {
                    _formats = formats;
                }

                private FormatEnumerator(FormatEnumerator formatEnumerator)
                {
                    _formats = formatEnumerator._formats;
                    _current = formatEnumerator._current;
                }

                public int Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
                {
                    var count = Math.Min(celt, _formats.Count - _current);
                    for (var i = 0; i < count; ++i)
                    {
                        rgelt[i] = _formats[_current];
                        ++_current;
                    }

                    if (pceltFetched != null)
                        pceltFetched[0] = count;
                    return count == celt ? 0 : 1;
                }

                public int Skip(int celt)
                {
                    _current = _current + celt;
                    return _current < _formats.Count ? 0 : 1;
                }

                public int Reset()
                {
                    _current = 0;
                    return 0;
                }

                public void Clone(out IEnumFORMATETC newEnum)
                {
                    newEnum = new FormatEnumerator(this);
                }
            }
        }
    }

    internal struct DragFileInfo
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }

        public DragFileInfo(string fileName, Stream stream)
        {
            FileName = fileName;
            Stream = stream;
        }
    }
}
