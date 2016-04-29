using System;
using System.IO;
using BlubLib.Network;
using BlubLib.Network.Pipes;
using ProudNet.Message;
using ProudNet.Message.Core;

namespace ProudNet.Services
{
    internal class ProudCoreService : MessageHandler
    {
        private readonly ProudPipe _filter;

        #region Events

        public event Action<MessageReceivedEventArgs> ProudMessageReceived;

        protected virtual void OnProudMessageReceived(MessageReceivedEventArgs e)
        {
            ProudMessageReceived?.Invoke(e);
        }

        #endregion

        public ProudCoreService(ProudPipe filter)
        {
            _filter = filter;
        }

        [MessageHandler(typeof(RmiMessage))]
        public void RmiMessage(ProudSession session, RmiMessage message, MessageReceivedEventArgs e)
        {
            var rmiId = BitConverter.ToUInt16(message.Data, 0);
            if (rmiId >= 60000)
            {
                using (var r = message.Data.ToBinaryReader())
                {
                    var opCode = r.ReadEnum<ProudOpCode>();
                    var proudMessage = ProudMapper.GetMessage(opCode, r);

                    if (!r.IsEOF())
#if DEBUG
                    {
                        r.BaseStream.Position = 0;
                        throw new ProudBadFormatException(proudMessage.GetType(), r.ReadToEnd());
                    }
#else
                    throw new ProudBadFormatException(proudMessage.GetType());
#endif
                    e.Message = proudMessage;
                    OnProudMessageReceived(e);
                }
                return;
            }

            e.Message = new ProudRmiMessage(message.Data);
            _filter.NextOnMessageReceived(e);
        }

        [MessageHandler(typeof(CompressedMessage))]
        public void CompressedMessage(IService service, CompressedMessage message, MessageReceivedEventArgs e)
        {
            var decompressed = message.Data.DecompressZLib();

            ProudCoreOpCode opCode;
            byte[] data;
            using (var r = decompressed.ToBinaryReader())
            {
                opCode = r.ReadEnum<ProudCoreOpCode>();
                data = r.ReadToEnd();
            }
            switch (opCode)
            {
                case ProudCoreOpCode.Rmi:
                    var rmi = new RmiMessage(data)
                    {
                        IsRelayed = message.IsRelayed,
                        SenderHostId = message.SenderHostId,
                        TargetHostId = message.TargetHostId
                    };
                    e.Message = rmi;
                    _filter.OnMessageReceived(e);
                    break;

                default:
                    throw new ProudException("Invalid opCode inside CompressedMessage: " + opCode);
            }
        }
    }
}
