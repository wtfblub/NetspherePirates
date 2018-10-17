using DotNetty.Codecs;

namespace ProudNet.DotNetty.Codecs
{
    public abstract class MessageToMessageDecoderEx<TMessage> : MessageToMessageDecoder<TMessage>
    {
        private readonly bool _throwIfWrongType;

        protected MessageToMessageDecoderEx(bool throwIfWrongType = true)
        {
            _throwIfWrongType = throwIfWrongType;
        }

        public override bool AcceptInboundMessage(object msg)
        {
            var acceptMessage = base.AcceptInboundMessage(msg);
            if (_throwIfWrongType && !acceptMessage)
                throw new UnsupportedMessageTypeException(msg, typeof(TMessage));

            return acceptMessage;
        }
    }
}
