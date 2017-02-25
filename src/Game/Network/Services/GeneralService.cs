using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class GeneralService : ProudMessageHandler
    {
        [MessageHandler(typeof(CTimeSyncReqMessage))]
        public async Task TimeSyncHandler(GameSession session, CTimeSyncReqMessage message)
        {
            await session.SendAsync(new STimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Program.AppTime.ElapsedMilliseconds
            }).ConfigureAwait(false);
        }
    }
}
