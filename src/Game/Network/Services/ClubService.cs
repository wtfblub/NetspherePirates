using BlubLib.DotNetty.Handlers.MessageHandling;
using Netsphere.Network.Message.Game;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class ClubService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(ClubService));

        [MessageHandler(typeof(CClubAddressReqMessage))]
        public void CClubAddressReq(GameSession session, CClubAddressReqMessage message)
        {
            Logger.ForAccount(session)
                .Debug("ClubAddressReq: {message}", message);

            session.SendAsync(new SClubAddressAckMessage("Kappa", 123));
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
