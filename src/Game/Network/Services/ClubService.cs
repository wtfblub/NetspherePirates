using BlubLib.Network.Pipes;
using Netsphere.Network.Message.Game;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class ClubService : MessageHandler
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CClubAddressReqMessage))]
        public void CClubAddressReq(GameSession session, CClubAddressReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message("RequestId:{0} LanguageId:{1} Command:{2}", message.RequestId, message.LanguageId, message.Command)
                .Write();

            session.Send(new SClubAddressAckMessage("Kappa", 123));
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
