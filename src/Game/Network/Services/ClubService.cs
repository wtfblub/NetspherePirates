using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class ClubService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CClubAddressReqMessage))]
        public async Task CClubAddressReq(GameSession session, CClubAddressReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message($"RequestId:{message.RequestId} LanguageId:{message.LanguageId} Command:{message.Command}")
                .Write();

            await session.SendAsync(new SClubAddressAckMessage("Kappa", 123))
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CClubInfoReqMessage))]
        public void CClubInfoReq()
        {
            //session.Send(new SClubInfoAckMessage(new PlayerClubInfoDto
            //{
            //    Unk1 = 0,
            //    Unk2 = 0,
            //    Unk3 = 0,
            //    Unk4 = 0,
            //    Unk5 = 0,
            //    Unk6 = 0,
            //    Unk7 = "",
            //    Unk8 = "",
            //    Unk9 = "Name?",
            //    ModeratorName = "Moderator",
            //    Unk11 = "",
            //    Unk12 = "",
            //    Unk13 = "",
            //}));
        }
    }
}
