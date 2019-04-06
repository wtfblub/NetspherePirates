using ProudNet.Serialization;

namespace Netsphere.Network.Message.Club
{
    public interface IClubMessage
    {
    }

    public class ClubMessageFactory : MessageFactory<ClubOpCode, IClubMessage>
    {
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
            Register<ClubCreateAck2Message>(ClubOpCode.ClubCreateAck2);
            Register<ClubCloseAck2Message>(ClubOpCode.ClubCloseAck2);
            Register<ClubJoinAck2Message>(ClubOpCode.ClubJoinAck2);
            Register<ClubUnjoinAck2Message>(ClubOpCode.ClubUnjoinAck2);
            Register<ClubRestoreAck2Message>(ClubOpCode.ClubRestoreAck2);
            Register<ClubClubInfoAck2Message>(ClubOpCode.ClubClubInfoAck2);
            Register<ClubSearchAck2Message>(ClubOpCode.ClubSearchAck2);
            Register<ClubEditURLAckMessage>(ClubOpCode.ClubEditURLAck);
            Register<ClubEditIntroduceAckMessage>(ClubOpCode.ClubEditIntroduceAck);
            Register<ClubStuffListAck2Message>(ClubOpCode.ClubStuffListAck2);
            Register<ClubRankListAckMessage>(ClubOpCode.ClubRankListAck);
            Register<ClubMatchNoticeMessage>(ClubOpCode.ClubMatchNotice);

            // C2S
            Register<ClubCreateReqMessage>(ClubOpCode.ClubCreateReq);
            Register<ClubCloseReqMessage>(ClubOpCode.ClubCloseReq);
            Register<ClubJoinReqMessage>(ClubOpCode.ClubJoinReq);
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
            Register<ClubCreateReq2Message>(ClubOpCode.ClubCreateReq2);
            Register<ClubCloseReq2Message>(ClubOpCode.ClubCloseReq2);
            Register<ClubJoinReq2Message>(ClubOpCode.ClubJoinReq2);
            Register<ClubUnjoinReq2Message>(ClubOpCode.ClubUnjoinReq2);
            Register<ClubRestoreReq2Message>(ClubOpCode.ClubRestoreReq2);
            Register<ClubClubInfoReq2Message>(ClubOpCode.ClubClubInfoReq2);
            Register<ClubSearchReq2Message>(ClubOpCode.ClubSearchReq2);
            Register<ClubEditURLReqMessage>(ClubOpCode.ClubEditURLReq);
            Register<ClubEditIntroduceReqMessage>(ClubOpCode.ClubEditIntroduceReq);
            Register<ClubStuffListReq2Message>(ClubOpCode.ClubStuffListReq2);
            Register<ClubRankListReqMessage>(ClubOpCode.ClubRankListReq);
        }
    }
}
