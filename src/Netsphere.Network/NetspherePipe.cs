using System.IO;
using BlubLib.IO;
using BlubLib.Network;
using Netsphere.Network.Message;
using ProudNet.Message;

namespace Netsphere.Network
{
    public class NetspherePipe : Pipe
    {
        private readonly INetsphereMessageFactory _messageFactory;

        public NetspherePipe(INetsphereMessageFactory messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public override void OnMessageReceived(MessageReceivedEventArgs e)
        {
            var proudMessage = (ProudMessage) e.Message;
            using (var ms = new PooledMemoryStream(Service.ArrayPool))
            using (var r = ms.ToBinaryReader(true))
            {
                e.Message.Serialize(ms);
                ms.Position = 0;

                var opCode = r.ReadUInt16();
                var leagueMessage = _messageFactory.GetMessage(e.Session, opCode, r);
                leagueMessage.IsRelayed = proudMessage.IsRelayed;
                leagueMessage.SenderHostId = proudMessage.SenderHostId;
                leagueMessage.TargetHostId = proudMessage.TargetHostId;

                if (!r.IsEOF())
#if DEBUG
                    throw new NetsphereBadFormatException(leagueMessage.GetType(), ms.ToSegment());
#else
                    throw new NetsphereBadFormatException(leagueMessage.GetType());
#endif

                e.Message = leagueMessage;
                base.OnMessageReceived(e);
            }
        }
    }
}
