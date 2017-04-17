using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization;

namespace Netsphere.Network.Message.Club
{
    public interface IClubMessage
    { }

    public class ClubMessageFactory : MessageFactory<ClubOpCode, IClubMessage>
    {
        static ClubMessageFactory()
        {
            Serializer.AddCompiler(new ItemNumberSerializer());
        }

        public ClubMessageFactory()
        {
            // S2C
            Register<ClubCreateAckMessage>(ClubOpCode.ClubCreateAck);
            Register<ClubCloseAckMessage>(ClubOpCode.ClubCloseAck);
            Register<ClubJoinAckMessage>(ClubOpCode.ClubJoinAck);
            Register<ClubUnjoinAckMessage>(ClubOpCode.ClubUnjoinAck);
            Register<ClubNameCheckAckMessage>(ClubOpCode.ClubNameCheckAck);
            Register<ClubRestoreAckMessage>(ClubOpCode.ClubRestoreAck);
            Register<ClubAdminInviteAckMessage>(ClubOpCode.ClubAdminInviteAck);
            Register<ClubAdminJoinCommandAckMessage>(ClubOpCode.ClubAdminJoinCommandAck);
            Register<ClubAdminGradeChangeAckMessage>(ClubOpCode.ClubAdminGradeChangeAck);
            Register<ClubAdminNoticeChangeAckMessage>(ClubOpCode.ClubAdminNoticeChangeAck);
            Register<ClubAdminInfoModifyAckMessage>(ClubOpCode.ClubAdminInfoModifyAck);
            Register<ClubAdminSubMasterAckMessage>(ClubOpCode.ClubAdminSubMasterAck);
            Register<ClubAdminSubMasterCancelAckMessage>(ClubOpCode.ClubAdminSubMasterCancelAck);
            Register<ClubAdminMasterChangeAckMessage>(ClubOpCode.ClubAdminMasterChangeAck);
            Register<ClubAdminJoinConditionModifyAckMessage>(ClubOpCode.ClubAdminJoinConditionModifyAck);
            Register<ClubAdminBoardModifyAckMessage>(ClubOpCode.ClubAdminBoardModifyAck);
            Register<ClubSearchAckMessage>(ClubOpCode.ClubSearchAck);
            Register<ClubInfoAckMessage>(ClubOpCode.ClubInfoAck);
            Register<ClubJoinWaiterInfoAckMessage>(ClubOpCode.ClubJoinWaiterInfoAck);
            Register<ClubNewJoinMemberInfoAckMessage>(ClubOpCode.ClubNewJoinMemberInfoAck);
            Register<ClubJoinConditionInfoAckMessage>(ClubOpCode.ClubJoinConditionInfoAck);
            Register<ClubUnjoinerListAckMessage>(ClubOpCode.ClubUnjoinerListAck);
            Register<ClubUnjoinSettingMemberListAckMessage>(ClubOpCode.ClubUnjoinSettingMemberListAck);
            Register<ClubGradeCountAckMessage>(ClubOpCode.ClubGradeCountAck);
            Register<ClubStuffListAckMessage>(ClubOpCode.ClubStuffListAck);
            Register<ClubNewsInfoAckMessage>(ClubOpCode.ClubNewsInfoAck);
            Register<ClubMyInfoAckMessage>(ClubOpCode.ClubMyInfoAck);
            Register<ClubBoardWriteAckMessage>(ClubOpCode.ClubBoardWriteAck);
            Register<ClubBoardReadAckMessage>(ClubOpCode.ClubBoardReadAck);
            Register<ClubBoardModifyAckMessage>(ClubOpCode.ClubBoardModifyAck);
            Register<ClubBoardDeleteAckMessage>(ClubOpCode.ClubBoardDeleteAck);
            Register<ClubBoardDeleteAllAckMessage>(ClubOpCode.ClubBoardDeleteAllAck);
            Register<ClubBoardReadFailedAckMessage>(ClubOpCode.ClubBoardReadFailedAck);

            // C2S
            Register<ClubCreateReqMessage>(ClubOpCode.ClubCreateReq);
            Register<ClubCloseReqMessage>(ClubOpCode.ClubCloseReq);
            Register<ClubUnjoinReqMessage>(ClubOpCode.ClubUnjoinReq);
            Register<ClubNameCheckReqMessage>(ClubOpCode.ClubNameCheckReq);
            Register<ClubRestoreReqMessage>(ClubOpCode.ClubRestoreReq);
            Register<ClubAdminInviteReqMessage>(ClubOpCode.ClubAdminInviteReq);
            Register<ClubAdminJoinCommandReqMessage>(ClubOpCode.ClubAdminJoinCommandReq);
            Register<ClubAdminGradeChangeReqMessage>(ClubOpCode.ClubAdminGradeChangeReq);
            Register<ClubAdminNoticeChangeReqMessage>(ClubOpCode.ClubAdminNoticeChangeReq);
            Register<ClubAdminInfoModifyReqMessage>(ClubOpCode.ClubAdminInfoModifyReq);
            Register<ClubAdminSubMasterReqMessage>(ClubOpCode.ClubAdminSubMasterReq);
            Register<ClubAdminSubMasterCancelReqMessage>(ClubOpCode.ClubAdminSubMasterCancelReq);
            Register<ClubAdminMasterChangeReqMessage>(ClubOpCode.ClubAdminMasterChangeReq);
            Register<ClubAdminJoinConditionModifyReqMessage>(ClubOpCode.ClubAdminJoinConditionModifyReq);
            Register<ClubAdminBoardModifyReqMessage>(ClubOpCode.ClubAdminBoardModifyReq);
            Register<ClubSearchReqMessage>(ClubOpCode.ClubSearchReq);
            Register<ClubInfoReqMessage>(ClubOpCode.ClubInfoReq);
            Register<ClubJoinWaiterInfoReqMessage>(ClubOpCode.ClubJoinWaiterInfoReq);
            Register<ClubNewJoinMemberInfoReqMessage>(ClubOpCode.ClubNewJoinMemberInfoReq);
            Register<ClubJoinConditionInfoReqMessage>(ClubOpCode.ClubJoinConditionInfoReq);
            Register<ClubUnjoinerListReqMessage>(ClubOpCode.ClubUnjoinerListReq);
            Register<ClubUnjoinSettingMemberListReqMessage>(ClubOpCode.ClubUnjoinSettingMemberListReq);
            Register<ClubGradeCountReqMessage>(ClubOpCode.ClubGradeCountReq);
            Register<ClubStuffListReqMessage>(ClubOpCode.ClubStuffListReq);
            Register<ClubNewsInfoReqMessage>(ClubOpCode.ClubNewsInfoReq);
            Register<ClubBoardWriteReqMessage>(ClubOpCode.ClubBoardWriteReq);
            Register<ClubBoardReadReqMessage>(ClubOpCode.ClubBoardReadReq);
            Register<ClubBoardModifyReqMessage>(ClubOpCode.ClubBoardModifyReq);
            Register<ClubBoardDeleteReqMessage>(ClubOpCode.ClubBoardDeleteReq);
            Register<ClubBoardDeleteAllReqMessage>(ClubOpCode.ClubBoardDeleteAllReq);
            Register<ClubBoardSearchNickReqMessage>(ClubOpCode.ClubBoardSearchNickReq);
            Register<ClubBoardReadOtherClubReqMessage>(ClubOpCode.ClubBoardReadOtherClubReq);
            Register<ClubBoardReadMineReqMessage>(ClubOpCode.ClubBoardReadMineReq);
        }
    }
}
