using System.IO;
using System.Net;
using BlubLib.Network.Message;
using BlubLib.Serialization;

namespace ProudNet.Message.Core
{
    internal abstract class CoreMessage : IMessage
    {
        public bool IsRelayed { get; set; }
        public uint SenderHostId { get; set; }
        public uint TargetHostId { get; set; }

        internal bool IsUdp { get; set; }
        internal IPEndPoint RemoteEndPoint { get; set; }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(CoreMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, this);
            }
        }
    }
}
