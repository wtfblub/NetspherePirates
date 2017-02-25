﻿using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization;

namespace Netsphere.Network.Message.Game
{
    public interface IGameMessage
    { }

    public class GameMessageFactory : MessageFactory<GameOpCode, IGameMessage>
    {
        static GameMessageFactory()
        {
            Serializer.AddCompiler(new MatchKeySerializer());
            Serializer.AddCompiler(new LongPeerIdSerializer());
            Serializer.AddCompiler(new CharacterStyleSerializer());
        }

        public GameMessageFactory()
        {
            // S2C
            Register<SLoginAckMessage>(GameOpCode.SLoginAck);
            Register<SBeginAccountInfoAckMessage>(GameOpCode.SBeginAccountInfoAck);
            Register<SOpenCharacterInfoAckMessage>(GameOpCode.SOpenCharacterInfoAck);
            Register<SCharacterEquipInfoAckMessage>(GameOpCode.SCharacterEquipInfoAck);
            Register<SInventoryInfoAckMessage>(GameOpCode.SInventoryInfoAck);
            Register<SSuccessDeleteCharacterAckMessage>(GameOpCode.SSuccessDeleteCharacterAck);
            Register<SSuccessSelectCharacterAckMessage>(GameOpCode.SSuccessSelectCharacterAck);
            Register<SSuccessCreateCharacterAckMessage>(GameOpCode.SSuccessCreateCharacterAck);
            Register<SServerResultInfoAckMessage>(GameOpCode.SServerResultInfoAck);
            Register<SCreateNickAckMessage>(GameOpCode.SCreateNickAck);
            Register<SCheckNickAckMessage>(GameOpCode.SCheckNickAck);
            Register<SUseItemAckMessage>(GameOpCode.SUseItemAck);
            Register<SInventoryActionAckMessage>(GameOpCode.SInventoryActionAck);
            Register<SIdsInfoAckMessage>(GameOpCode.SIdsInfoAck);
            Register<SEnteredPlayerAckMessage>(GameOpCode.SEnteredPlayerAck);
            Register<SEnteredPlayerClubInfoAckMessage>(GameOpCode.SEnteredPlayerClubInfoAck);
            Register<SEnteredPlayerListAckMessage>(GameOpCode.SEnteredPlayerListAck);
            Register<SEnteredPlayerClubInfoListAckMessage>(GameOpCode.SEnteredPlayerClubInfoListAck);
            Register<SSuccessEnterRoomAckMessage>(GameOpCode.SSuccessEnterRoomAck);
            Register<SLeavePlayerAckMessage>(GameOpCode.SLeavePlayerAck);
            Register<SJoinTunnelPlayerAckMessage>(GameOpCode.SJoinTunnelPlayerAck);
            Register<STimeSyncAckMessage>(GameOpCode.STimeSyncAck);
            Register<SPlayTogetherSignAckMessage>(GameOpCode.SPlayTogetherSignAck);
            Register<SPlayTogetherInfoAckMessage>(GameOpCode.SPlayTogetherInfoAck);
            Register<SPlayTogetherSignInfoAckMessage>(GameOpCode.SPlayTogetherSignInfoAck);
            Register<SPlayTogetherCancelAckMessage>(GameOpCode.SPlayTogetherCancelAck);
            Register<SChangeGameRoomAckMessage>(GameOpCode.SChangeGameRoomAck);
            Register<SNewShopUpdateRequestAckMessage>(GameOpCode.SNewShopUpdateRequestAck);
            Register<SLogoutAckMessage>(GameOpCode.SLogoutAck);
            Register<SPlayTogetherKickAckMessage>(GameOpCode.SPlayTogetherKickAck);
            Register<SChannelListInfoAckMessage>(GameOpCode.SChannelListInfoAck);
            Register<SChannelDeployPlayerAckMessage>(GameOpCode.SChannelDeployPlayerAck);
            Register<SChannelDisposePlayerAckMessage>(GameOpCode.SChannelDisposePlayerAck);
            Register<SGameRoomListAckMessage>(GameOpCode.SGameRoomListAck);
            Register<SDeployGameRoomAckMessage>(GameOpCode.SDeployGameRoomAck);
            Register<SDisposeGameRoomAckMessage>(GameOpCode.SDisposeGameRoomAck);
            Register<SGamePingAverageAckMessage>(GameOpCode.SGamePingAverageAck);
            Register<SBuyItemAckMessage>(GameOpCode.SBuyItemAck);
            Register<SRepairItemAckMessage>(GameOpCode.SRepairItemAck);
            Register<SItemDurabilityInfoAckMessage>(GameOpCode.SItemDurabilityInfoAck);
            Register<SRefundItemAckMessage>(GameOpCode.SRefundItemAck);
            Register<SRefreshCashInfoAckMessage>(GameOpCode.SRefreshCashInfoAck);
            Register<SAdminActionAckMessage>(GameOpCode.SAdminActionAck);
            Register<SAdminShowWindowAckMessage>(GameOpCode.SAdminShowWindowAck);
            Register<SNoticeMessageAckMessage>(GameOpCode.SNoticeMessageAck);
            Register<SCharacterSlotInfoAckMessage>(GameOpCode.SCharacterSlotInfoAck);
            Register<SRefreshInvalidEquipItemAckMessage>(GameOpCode.SRefreshInvalidEquipItemAck);
            Register<SClearInvalidateItemAckMessage>(GameOpCode.SClearInvalidateItemAck);
            Register<SRefreshItemTimeInfoAckMessage>(GameOpCode.SRefreshItemTimeInfoAck);
            Register<SEnableAccountStatusAckMessage>(GameOpCode.SEnableAccountStatusAck);
            Register<SActiveEquipPresetAckMessage>(GameOpCode.SActiveEquipPresetAck);
            Register<SMyLicenseInfoAckMessage>(GameOpCode.SMyLicenseInfoAck);
            Register<SLicensedAckMessage>(GameOpCode.SLicensedAck);
            Register<SCoinEventAckMessage>(GameOpCode.SCoinEventAck);
            Register<SCombiCompensationAckMessage>(GameOpCode.SCombiCompensationAck);
            Register<SClubInfoAckMessage>(GameOpCode.SClubInfoAck);
            Register<SClubHistoryAckMessage>(GameOpCode.SClubHistoryAck);
            Register<SEquipedBoostItemAckMessage>(GameOpCode.SEquipedBoostItemAck);
            Register<SGetClubInfoAckMessage>(GameOpCode.SGetClubInfoAck);
            Register<STaskInfoAckMessage>(GameOpCode.STaskInfoAck);
            Register<STaskUpdateAckMessage>(GameOpCode.STaskUpdateAck);
            Register<STaskRequestAckMessage>(GameOpCode.STaskRequestAck);
            Register<SExchangeItemAckMessage>(GameOpCode.SExchangeItemAck);
            Register<STaskIngameUpdateAckMessage>(GameOpCode.STaskIngameUpdateAck);
            Register<STaskRemoveAckMessage>(GameOpCode.STaskRemoveAck);
            Register<SRandomShopChanceInfoAckMessage>(GameOpCode.SRandomShopChanceInfoAck);
            Register<SRandomShopItemInfoAckMessage>(GameOpCode.SRandomShopItemInfoAck);
            Register<SRandomShopInfoAckMessage>(GameOpCode.SRandomShopInfoAck);
            Register<SSetCoinAckMessage>(GameOpCode.SSetCoinAck);
            Register<SApplyEsperChipItemAckMessage>(GameOpCode.SApplyEsperChipItemAck);
            Register<SArcadeRewardInfoAckMessage>(GameOpCode.SArcadeRewardInfoAck);
            Register<SArcadeMapScoreAckMessage>(GameOpCode.SArcadeMapScoreAck);
            Register<SArcadeStageScoreAckMessage>(GameOpCode.SArcadeStageScoreAck);
            Register<SMixedTeamBriefingInfoAckMessage>(GameOpCode.SMixedTeamBriefingInfoAck);
            Register<SSetGameMoneyAckMessage>(GameOpCode.SSetGameMoneyAck);
            Register<SUseCapsuleAckMessage>(GameOpCode.SUseCapsuleAck);
            Register<SHGWKickAckMessage>(GameOpCode.SHGWKickAck);
            Register<SClubJoinAckMessage>(GameOpCode.SClubJoinAck);
            Register<SClubUnJoinAckMessage>(GameOpCode.SClubUnJoinAck);
            Register<SNewShopUpdateCheckAckMessage>(GameOpCode.SNewShopUpdateCheckAck);
            Register<SNewShopUpdateInfoAckMessage>(GameOpCode.SNewShopUpdateInfoAck);
            Register<SUseChangeNickItemAckMessage>(GameOpCode.SUseChangeNickItemAck);
            Register<SUseResetRecordItemAckMessage>(GameOpCode.SUseResetRecordItemAck);
            Register<SUseCoinFillingItemAckMessage>(GameOpCode.SUseCoinFillingItemAck);
            Register<SDiscardItemAckMessage>(GameOpCode.SDiscardItemAck);
            Register<SDeleteItemInventoryAckMessage>(GameOpCode.SDeleteItemInventoryAck);
            Register<SClubAddressAckMessage>(GameOpCode.SClubAddressAck);
            Register<SSmallLoudSpeakerAckMessage>(GameOpCode.SSmallLoudSpeakerAck);
            Register<SIngameEquipCheckAckMessage>(GameOpCode.SIngameEquipCheckAck);
            Register<SUseCoinRandomShopChanceAckMessage>(GameOpCode.SUseCoinRandomShopChanceAck);
            Register<SChangeNickCancelAckMessage>(GameOpCode.SChangeNickCancelAck);
            Register<SEventRewardAckMessage>(GameOpCode.SEventRewardAck);

            // C2S
            Register<CCreateCharacterReqMessage>(GameOpCode.CCreateCharacterReq);
            Register<CSelectCharacterReqMessage>(GameOpCode.CSelectCharacterReq);
            Register<CDeleteCharacterReqMessage>(GameOpCode.CDeleteCharacterReq);
            Register<CLoginReqMessage>(GameOpCode.CLoginReq);
            Register<CQuickStartReqMessage>(GameOpCode.CQuickStartReq);
            Register<CMakeRoomReqMessage>(GameOpCode.CMakeRoomReq);
            Register<CCreateNickReqMessage>(GameOpCode.CCreateNickReq);
            Register<CCheckNickReqMessage>(GameOpCode.CCheckNickReq);
            Register<CUseItemReqMessage>(GameOpCode.CUseItemReq);
            Register<CJoinTunnelInfoReqMessage>(GameOpCode.CJoinTunnelInfoReq);
            Register<CTimeSyncReqMessage>(GameOpCode.CTimeSyncReq);
            Register<CGameArgPingReqMessage>(GameOpCode.CGameArgPingReq);
            Register<CAdminShowWindowReqMessage>(GameOpCode.CAdminShowWindowReq);
            Register<CClubInfoReqMessage>(GameOpCode.CClubInfoReq);
            Register<CIngameEquipCheckReqMessage>(GameOpCode.CIngameEquipCheckReq);
            Register<CUseCoinRandomShopChanceReqMessage>(GameOpCode.CUseCoinRandomShopChanceReq);
            Register<CChannelEnterReqMessage>(GameOpCode.CChannelEnterReq);
            Register<CChannelLeaveReqMessage>(GameOpCode.CChannelLeaveReq);
            Register<CGetChannelInfoReqMessage>(GameOpCode.CGetChannelInfoReq);
            Register<CGameRoomEnterReqMessage>(GameOpCode.CGameRoomEnterReq);
            Register<CGetPlayerInfoReqMessage>(GameOpCode.CGetPlayerInfoReq);
            Register<CBuyItemReqMessage>(GameOpCode.CBuyItemReq);
            Register<CRepairItemReqMessage>(GameOpCode.CRepairItemReq);
            Register<CRefundItemReqMessage>(GameOpCode.CRefundItemReq);
            Register<CAdminActionReqMessage>(GameOpCode.CAdminActionReq);
            Register<CActiveEquipPresetReqMessage>(GameOpCode.CActiveEquipPresetReq);
            Register<CLicensedReqMessage>(GameOpCode.CLicensedReq);
            Register<CClubNoticeChangeReqMessage>(GameOpCode.CClubNoticeChangeReq);
            Register<CGetClubInfoReqMessage>(GameOpCode.CGetClubInfoReq);
            Register<CGetClubInfoByNameReqMessage>(GameOpCode.CGetClubInfoByNameReq);
            Register<CGetInventoryItemReqMessage>(GameOpCode.CGetInventoryItemReq);
            Register<CTaskNotifyReqMessage>(GameOpCode.CTaskNotifyReq);
            Register<CTaskRequestReqMessage>(GameOpCode.CTaskRequestReq);
            Register<CRandomShopRollingStartReqMessage>(GameOpCode.CRandomShopRollingStartReq);
            Register<CRandomShopItemGetReqMessage>(GameOpCode.CRandomShopItemGetReq);
            Register<CRandomShopItemSaleReqMessage>(GameOpCode.CRandomShopItemSaleReq);
            Register<CExerciseLicenceReqMessage>(GameOpCode.CExerciseLicenceReq);
            Register<CUseCoinReqGSMessage>(GameOpCode.CUseCoinReqGS);
            Register<CApplyEsperChipItemReqMessage>(GameOpCode.CApplyEsperChipItemReq);
            Register<CBadUserReqMessage>(GameOpCode.CBadUserReq);
            Register<CClubJoinReqMessage>(GameOpCode.CClubJoinReq);
            Register<CClubUnJoinReqMessage>(GameOpCode.CClubUnJoinReq);
            Register<CNewShopUpdateCheckReqMessage>(GameOpCode.CNewShopUpdateCheckReq);
            Register<CUseChangeNickNameItemReqMessage>(GameOpCode.CUseChangeNickNameItemReq);
            Register<CUseResetRecordItemReqMessage>(GameOpCode.CUseResetRecordItemReq);
            Register<CUseCoinFillingItemReqMessage>(GameOpCode.CUseCoinFillingItemReq);
            Register<CGetUserInfoListReqMessage>(GameOpCode.CGetUserInfoListReq);
            Register<CFindUserReqMessage>(GameOpCode.CFindUserReq);
            Register<CDiscardItemReqMessage>(GameOpCode.CDiscardItemReq);
            Register<CUseCapsuleReqMessage>(GameOpCode.CUseCapsuleReq);
            Register<CSaveConfigPermissionNotifyReqMessage>(GameOpCode.CSaveConfigPermissionNotifyReq);
            Register<CClubAddressReqMessage>(GameOpCode.CClubAddressReq);
            Register<CSmallLoudSpeakerReqMessage>(GameOpCode.CSmallLoudSpeakerReq);
            Register<CClubHistoryReqMessage>(GameOpCode.CClubHistoryReq);
            Register<CChangeNickCancelReqMessage>(GameOpCode.CChangeNickCancelReq);
            Register<CEnableAccountStatusAckMessage>(GameOpCode.CEnableAccountStatusAck);
        }
    }
}
