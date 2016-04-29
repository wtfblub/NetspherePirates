using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.Game
{
    internal static class GameMapper
    {
        private static readonly Dictionary<GameOpCode, Type> s_typeLookup = new Dictionary<GameOpCode, Type>();
        private static readonly Dictionary<Type, GameOpCode> s_opCodeLookup = new Dictionary<Type, GameOpCode>();

        static GameMapper()
        {
            // S2C
            Create<SLoginAckMessage>(GameOpCode.SLoginAck);
            Create<SBeginAccountInfoAckMessage>(GameOpCode.SBeginAccountInfoAck);
            Create<SOpenCharacterInfoAckMessage>(GameOpCode.SOpenCharacterInfoAck);
            Create<SCharacterEquipInfoAckMessage>(GameOpCode.SCharacterEquipInfoAck);
            Create<SInventoryInfoAckMessage>(GameOpCode.SInventoryInfoAck);
            Create<SSuccessDeleteCharacterAckMessage>(GameOpCode.SSuccessDeleteCharacterAck);
            Create<SSuccessSelectCharacterAckMessage>(GameOpCode.SSuccessSelectCharacterAck);
            Create<SSuccessCreateCharacterAckMessage>(GameOpCode.SSuccessCreateCharacterAck);
            Create<SServerResultInfoAckMessage>(GameOpCode.SServerResultInfoAck);
            Create<SCreateNickAckMessage>(GameOpCode.SCreateNickAck);
            Create<SCheckNickAckMessage>(GameOpCode.SCheckNickAck);
            Create<SUseItemAckMessage>(GameOpCode.SUseItemAck);
            Create<SInventoryActionAckMessage>(GameOpCode.SInventoryActionAck);
            Create<SIdsInfoAckMessage>(GameOpCode.SIdsInfoAck);
            Create<SEnteredPlayerAckMessage>(GameOpCode.SEnteredPlayerAck);
            Create<SEnteredPlayerClubInfoAckMessage>(GameOpCode.SEnteredPlayerClubInfoAck);
            Create<SEnteredPlayerListAckMessage>(GameOpCode.SEnteredPlayerListAck);
            Create<SEnteredPlayerClubInfoListAckMessage>(GameOpCode.SEnteredPlayerClubInfoListAck);
            Create<SSuccessEnterRoomAckMessage>(GameOpCode.SSuccessEnterRoomAck);
            Create<SLeavePlayerAckMessage>(GameOpCode.SLeavePlayerAck);
            Create<SJoinTunnelPlayerAckMessage>(GameOpCode.SJoinTunnelPlayerAck);
            Create<STimeSyncAckMessage>(GameOpCode.STimeSyncAck);
            Create<SPlayTogetherSignAckMessage>(GameOpCode.SPlayTogetherSignAck);
            Create<SPlayTogetherInfoAckMessage>(GameOpCode.SPlayTogetherInfoAck);
            Create<SPlayTogetherSignInfoAckMessage>(GameOpCode.SPlayTogetherSignInfoAck);
            Create<SPlayTogetherCancelAckMessage>(GameOpCode.SPlayTogetherCancelAck);
            Create<SChangeGameRoomAckMessage>(GameOpCode.SChangeGameRoomAck);
            Create<SNewShopUpdateRequestAckMessage>(GameOpCode.SNewShopUpdateRequestAck);
            Create<SLogoutAckMessage>(GameOpCode.SLogoutAck);
            Create<SPlayTogetherKickAckMessage>(GameOpCode.SPlayTogetherKickAck);
            Create<SChannelListInfoAckMessage>(GameOpCode.SChannelListInfoAck);
            Create<SChannelDeployPlayerAckMessage>(GameOpCode.SChannelDeployPlayerAck);
            Create<SChannelDisposePlayerAckMessage>(GameOpCode.SChannelDisposePlayerAck);
            Create<SGameRoomListAckMessage>(GameOpCode.SGameRoomListAck);
            Create<SDeployGameRoomAckMessage>(GameOpCode.SDeployGameRoomAck);
            Create<SDisposeGameRoomAckMessage>(GameOpCode.SDisposeGameRoomAck);
            Create<SGamePingAverageAckMessage>(GameOpCode.SGamePingAverageAck);
            Create<SBuyItemAckMessage>(GameOpCode.SBuyItemAck);
            Create<SRepairItemAckMessage>(GameOpCode.SRepairItemAck);
            Create<SItemDurabilityInfoAckMessage>(GameOpCode.SItemDurabilityInfoAck);
            Create<SRefundItemAckMessage>(GameOpCode.SRefundItemAck);
            Create<SRefreshCashInfoAckMessage>(GameOpCode.SRefreshCashInfoAck);
            Create<SAdminActionAckMessage>(GameOpCode.SAdminActionAck);
            Create<SAdminShowWindowAckMessage>(GameOpCode.SAdminShowWindowAck);
            Create<SNoticeMessageAckMessage>(GameOpCode.SNoticeMessageAck);
            Create<SCharacterSlotInfoAckMessage>(GameOpCode.SCharacterSlotInfoAck);
            Create<SRefreshInvalidEquipItemAckMessage>(GameOpCode.SRefreshInvalidEquipItemAck);
            Create<SClearInvalidateItemAckMessage>(GameOpCode.SClearInvalidateItemAck);
            Create<SRefreshItemTimeInfoAckMessage>(GameOpCode.SRefreshItemTimeInfoAck);
            Create<SEnableAccountStatusAckMessage>(GameOpCode.SEnableAccountStatusAck);
            Create<SActiveEquipPresetAckMessage>(GameOpCode.SActiveEquipPresetAck);
            Create<SMyLicenseInfoAckMessage>(GameOpCode.SMyLicenseInfoAck);
            Create<SLicensedAckMessage>(GameOpCode.SLicensedAck);
            Create<SCoinEventAckMessage>(GameOpCode.SCoinEventAck);
            Create<SCombiCompensationAckMessage>(GameOpCode.SCombiCompensationAck);
            Create<SClubInfoAckMessage>(GameOpCode.SClubInfoAck);
            Create<SClubHistoryAckMessage>(GameOpCode.SClubHistoryAck);
            Create<SEquipedBoostItemAckMessage>(GameOpCode.SEquipedBoostItemAck);
            Create<SGetClubInfoAckMessage>(GameOpCode.SGetClubInfoAck);
            Create<STaskInfoAckMessage>(GameOpCode.STaskInfoAck);
            Create<STaskUpdateAckMessage>(GameOpCode.STaskUpdateAck);
            Create<STaskRequestAckMessage>(GameOpCode.STaskRequestAck);
            Create<SExchangeItemAckMessage>(GameOpCode.SExchangeItemAck);
            Create<STaskIngameUpdateAckMessage>(GameOpCode.STaskIngameUpdateAck);
            Create<STaskRemoveAckMessage>(GameOpCode.STaskRemoveAck);
            Create<SRandomShopChanceInfoAckMessage>(GameOpCode.SRandomShopChanceInfoAck);
            Create<SRandomShopItemInfoAckMessage>(GameOpCode.SRandomShopItemInfoAck);
            Create<SRandomShopInfoAckMessage>(GameOpCode.SRandomShopInfoAck);
            Create<SSetCoinAckMessage>(GameOpCode.SSetCoinAck);
            Create<SApplyEsperChipItemAckMessage>(GameOpCode.SApplyEsperChipItemAck);
            Create<SArcadeRewardInfoAckMessage>(GameOpCode.SArcadeRewardInfoAck);
            Create<SArcadeMapScoreAckMessage>(GameOpCode.SArcadeMapScoreAck);
            Create<SArcadeStageScoreAckMessage>(GameOpCode.SArcadeStageScoreAck);
            Create<SMixedTeamBriefingInfoAckMessage>(GameOpCode.SMixedTeamBriefingInfoAck);
            Create<SSetGameMoneyAckMessage>(GameOpCode.SSetGameMoneyAck);
            Create<SUseCapsuleAckMessage>(GameOpCode.SUseCapsuleAck);
            Create<SHGWKickAckMessage>(GameOpCode.SHGWKickAck);
            Create<SClubJoinAckMessage>(GameOpCode.SClubJoinAck);
            Create<SClubUnJoinAckMessage>(GameOpCode.SClubUnJoinAck);
            Create<SNewShopUpdateCheckAckMessage>(GameOpCode.SNewShopUpdateCheckAck);
            Create<SNewShopUpdateInfoAckMessage>(GameOpCode.SNewShopUpdateInfoAck);
            Create<SUseChangeNickItemAckMessage>(GameOpCode.SUseChangeNickItemAck);
            Create<SUseResetRecordItemAckMessage>(GameOpCode.SUseResetRecordItemAck);
            Create<SUseCoinFillingItemAckMessage>(GameOpCode.SUseCoinFillingItemAck);
            Create<SDiscardItemAckMessage>(GameOpCode.SDiscardItemAck);
            Create<SDeleteItemInventoryAckMessage>(GameOpCode.SDeleteItemInventoryAck);
            Create<SClubAddressAckMessage>(GameOpCode.SClubAddressAck);
            Create<SSmallLoudSpeakerAckMessage>(GameOpCode.SSmallLoudSpeakerAck);
            Create<SIngameEquipCheckAckMessage>(GameOpCode.SIngameEquipCheckAck);
            Create<SUseCoinRandomShopChanceAckMessage>(GameOpCode.SUseCoinRandomShopChanceAck);
            Create<SChangeNickCancelAckMessage>(GameOpCode.SChangeNickCancelAck);
            Create<SEventRewardAckMessage>(GameOpCode.SEventRewardAck);

            // C2S
            Create<CCreateCharacterReqMessage>(GameOpCode.CCreateCharacterReq);
            Create<CSelectCharacterReqMessage>(GameOpCode.CSelectCharacterReq);
            Create<CDeleteCharacterReqMessage>(GameOpCode.CDeleteCharacterReq);
            Create<CLoginReqMessage>(GameOpCode.CLoginReq);
            Create<CQuickStartReqMessage>(GameOpCode.CQuickStartReq);
            Create<CMakeRoomReqMessage>(GameOpCode.CMakeRoomReq);
            Create<CCreateNickReqMessage>(GameOpCode.CCreateNickReq);
            Create<CCheckNickReqMessage>(GameOpCode.CCheckNickReq);
            Create<CUseItemReqMessage>(GameOpCode.CUseItemReq);
            Create<CJoinTunnelInfoReqMessage>(GameOpCode.CJoinTunnelInfoReq);
            Create<CTimeSyncReqMessage>(GameOpCode.CTimeSyncReq);
            Create<CGameArgPingReqMessage>(GameOpCode.CGameArgPingReq);
            Create<CAdminShowWindowReqMessage>(GameOpCode.CAdminShowWindowReq);
            Create<CClubInfoReqMessage>(GameOpCode.CClubInfoReq);
            Create<CIngameEquipCheckReqMessage>(GameOpCode.CIngameEquipCheckReq);
            Create<CUseCoinRandomShopChanceReqMessage>(GameOpCode.CUseCoinRandomShopChanceReq);
            Create<CChannelEnterReqMessage>(GameOpCode.CChannelEnterReq);
            Create<CChannelLeaveReqMessage>(GameOpCode.CChannelLeaveReq);
            Create<CGetChannelInfoReqMessage>(GameOpCode.CGetChannelInfoReq);
            Create<CGameRoomEnterReqMessage>(GameOpCode.CGameRoomEnterReq);
            Create<CGetPlayerInfoReqMessage>(GameOpCode.CGetPlayerInfoReq);
            Create<CBuyItemReqMessage>(GameOpCode.CBuyItemReq);
            Create<CRepairItemReqMessage>(GameOpCode.CRepairItemReq);
            Create<CRefundItemReqMessage>(GameOpCode.CRefundItemReq);
            Create<CAdminActionReqMessage>(GameOpCode.CAdminActionReq);
            Create<CActiveEquipPresetReqMessage>(GameOpCode.CActiveEquipPresetReq);
            Create<CLicensedReqMessage>(GameOpCode.CLicensedReq);
            Create<CClubNoticeChangeReqMessage>(GameOpCode.CClubNoticeChangeReq);
            Create<CGetClubInfoReqMessage>(GameOpCode.CGetClubInfoReq);
            Create<CGetClubInfoByNameReqMessage>(GameOpCode.CGetClubInfoByNameReq);
            Create<CGetInventoryItemReqMessage>(GameOpCode.CGetInventoryItemReq);
            Create<CTaskNotifyReqMessage>(GameOpCode.CTaskNotifyReq);
            Create<CTaskRequestReqMessage>(GameOpCode.CTaskRequestReq);
            Create<CRandomShopRollingStartReqMessage>(GameOpCode.CRandomShopRollingStartReq);
            Create<CRandomShopItemGetReqMessage>(GameOpCode.CRandomShopItemGetReq);
            Create<CRandomShopItemSaleReqMessage>(GameOpCode.CRandomShopItemSaleReq);
            Create<CExerciseLicenceReqMessage>(GameOpCode.CExerciseLicenceReq);
            Create<CUseCoinReqGSMessage>(GameOpCode.CUseCoinReqGS);
            Create<CApplyEsperChipItemReqMessage>(GameOpCode.CApplyEsperChipItemReq);
            Create<CBadUserReqMessage>(GameOpCode.CBadUserReq);
            Create<CClubJoinReqMessage>(GameOpCode.CClubJoinReq);
            Create<CClubUnJoinReqMessage>(GameOpCode.CClubUnJoinReq);
            Create<CNewShopUpdateCheckReqMessage>(GameOpCode.CNewShopUpdateCheckReq);
            Create<CUseChangeNickNameItemReqMessage>(GameOpCode.CUseChangeNickNameItemReq);
            Create<CUseResetRecordItemReqMessage>(GameOpCode.CUseResetRecordItemReq);
            Create<CUseCoinFillingItemReqMessage>(GameOpCode.CUseCoinFillingItemReq);
            Create<CGetUserInfoListReqMessage>(GameOpCode.CGetUserInfoListReq);
            Create<CFindUserReqMessage>(GameOpCode.CFindUserReq);
            Create<CDiscardItemReqMessage>(GameOpCode.CDiscardItemReq);
            Create<CUseCapsuleReqMessage>(GameOpCode.CUseCapsuleReq);
            Create<CSaveConfigPermissionNotifyReqMessage>(GameOpCode.CSaveConfigPermissionNotifyReq);
            Create<CClubAddressReqMessage>(GameOpCode.CClubAddressReq);
            Create<CSmallLoudSpeakerReqMessage>(GameOpCode.CSmallLoudSpeakerReq);
            Create<CClubHistoryReqMessage>(GameOpCode.CClubHistoryReq);
            Create<CChangeNickCancelReqMessage>(GameOpCode.CChangeNickCancelReq);
            Create<CEnableAccountStatusAckMessage>(GameOpCode.CEnableAccountStatusAck);
        }

        public static void Create<T>(GameOpCode opCode)
            where T : GameMessage, new()
        {
            var type = typeof(T);
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static GameMessage GetMessage(GameOpCode opCode, BinaryReader r)
        {
            var type = s_typeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new NetsphereBadOpCodeException(opCode);

            return (GameMessage)Serializer.Deserialize(r, type);
        }

        public static GameOpCode GetOpCode<T>()
            where T : GameMessage
        {
            return GetOpCode(typeof(T));
        }

        public static GameOpCode GetOpCode(Type type)
        {
            return s_opCodeLookup[type];
        }
    }
}
