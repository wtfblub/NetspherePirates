using System.IO;
using BlubLib.Network.Message;
using BlubLib.Serialization;

namespace ProudNet.Message
{
    public abstract class ProudMessage : IMessage
    {
        public bool Encrypt { get; set; }
        public bool Compress { get; set; }

        public bool IsRelayed { get; set; }
        public uint SenderHostId { get; set; }
        public uint TargetHostId { get; set; }

        protected ProudMessage()
            : this(true, false)
        { }

        protected ProudMessage(bool encrypt, bool compress)
        {
            Encrypt = encrypt;
            Compress = compress;
        }

        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(ProudMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public class ProudUnknownMessage : ProudMessage
    {
        public ProudOpCode OpCode { get; }
        public byte[] Data { get; }

        public ProudUnknownMessage(ProudOpCode opCode, byte[] data)
        {
            OpCode = opCode;
            Data = data;
        }

        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(OpCode);
                w.Write(Data);
            }
        }
    }

    internal class ProudRmiMessage : ProudMessage
    {
        private readonly byte[] _data;

        public ProudRmiMessage(byte[] data)
        {
            _data = data;
        }

        public override void Serialize(Stream stream)
        {
            stream.Write(_data, 0, _data.Length);
        }
    }
}
