using System.Net;
using DotNetty.Buffers;

namespace ProudNet.Codecs
{
    internal class UdpMessage
    {
        public ushort Flag { get; set; }
        public ushort SessionId { get; set; }
        public int Length { get; set; }
        public uint Id { get; set; }
        public uint FragId { get; set; }
        public IByteBuffer Content { get; set; }

        public IPEndPoint EndPoint { get; set; }
    }
}
