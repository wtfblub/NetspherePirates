using System.IO;
using System.Threading.Tasks;
using BlubLib.DotNetty;
using DotNetty.Buffers;
using ProudNet.DotNetty.Codecs;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Handlers
{
    internal class MessageHandler
        : IHandle<RmiMessage>,
          IHandle<CompressedMessage>,
          IHandle<EncryptedReliableMessage>
    {
        [Firewall(typeof(MustBeInState), SessionState.Connected)]
        public Task<bool> OnHandle(MessageContext context, RmiMessage message)
        {
            var buffer = Unpooled.WrappedBuffer(message.Data);
            context.Message = buffer;
            context.ChannelHandlerContext.FireChannelRead(context);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInState), SessionState.Connected)]
        public Task<bool> OnHandle(MessageContext context, CompressedMessage message)
        {
            var decompressed = message.Data.DecompressZLib();
            var buffer = Unpooled.WrappedBuffer(decompressed);
            context.Message = buffer;
            context.ChannelHandlerContext.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(context);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInState), SessionState.Connected)]
        public Task<bool> OnHandle(MessageContext context, EncryptedReliableMessage message)
        {
            Crypt crypt;
            // TODO Decrypt P2P
            //if (message.IsRelayed)
            //{
            //    //var remotePeer = (ServerRemotePeer)session.P2PGroup?.Members.GetValueOrDefault(message.TargetHostId);
            //    //if (remotePeer == null)
            //    //    return;

            //    //encryptContext = remotePeer.EncryptContext;
            //    //if (encryptContext == null)
            //    //    throw new ProudException($"Received encrypted message but the remote peer has no encryption enabled");
            //}
            //else
            {
                crypt = context.Session.Crypt;
            }

            var buffer = context.ChannelHandlerContext.Allocator.Buffer(message.Data.Length);
            using (var src = new MemoryStream(message.Data))
            using (var dst = new WriteOnlyByteBufferStream(buffer, false))
                crypt.Decrypt(context.ChannelHandlerContext.Allocator, message.EncryptMode, src, dst, true);

            context.Message = buffer;
            context.ChannelHandlerContext.Channel.Pipeline.Context<ProudFrameDecoder>().FireChannelRead(context);
            return Task.FromResult(true);
        }
    }
}
