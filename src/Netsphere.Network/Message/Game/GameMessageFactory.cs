using BlubLib.Serialization;
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
            Register<LoginReguestAckMessage>(GameOpCode.LoginReguestAck);
            Register<PlayerAccountInfoAckMessage>(GameOpCode.PlayerAccountInfoAck);
            Register<CharacterCurrentInfoAckMessage>(GameOpCode.CharacterCurrentInfoAck);
            Register<CharacterCurrentItemInfoAckMessage>(GameOpCode.CharacterCurrentItemInfoAck);
            Register<ItemInventoryInfoAckMessage>(GameOpCode.ItemInventoryInfoAck);
            Register<CharacterDeleteAckMessage>(GameOpCode.CharacterDeleteAck);
            Register<CharacterSelectAckMessage>(GameOpCode.CharacterSelectAck);
            Register<CSuccessCreateCharacterAckMessage>(GameOpCode.CSuccessCreateCharacterAck);
            Register<ServerResultAckMessage>(GameOpCode.ServerResultAck);
            Register<NickCheckAckMessage>(GameOpCode.NickCheckAck);
            Register<ItemUseItemAckMessage>(GameOpCode.ItemUseItemAck);
            Register<ItemUpdateInventoryAckMessage>(GameOpCode.ItemUpdateInventoryAck);
            Register<RoomCurrentCharacterSlotAckMessage>(GameOpCode.RoomCurrentCharacterSlotAck);
            Register<RoomEnterPlayerInfoAckMessage>(GameOpCode.RoomEnterPlayerInfoAck);
            Register<RoomEnterClubInfoAckMessage>(GameOpCode.RoomEnterClubInfoAck);
            Register<RoomPlayerInfoListForEnterPlayerAckMessage>(GameOpCode.RoomPlayerInfoListForEnterPlayerAck);
            Register<RoomClubInfoListForEnterPlayerAckMessage>(GameOpCode.RoomClubInfoListForEnterPlayerAck);
            Register<RoomEnterRoomInfoAckMessage>(GameOpCode.RoomEnterRoomInfoAck);
            Register<RoomLeavePlayerInfoAckMessage>(GameOpCode.RoomLeavePlayerInfoAck);
            Register<TimeSyncAckMessage>(GameOpCode.TimeSyncAck);
            Register<RoomChangeRoomInfoAckMessage>(GameOpCode.RoomChangeRoomInfoAck);
            Register<NewShopUpdateEndAckMessage>(GameOpCode.NewShopUpdateEndAck);
            Register<ChannelListInfoAckMessage>(GameOpCode.ChannelListInfoAck);
            Register<RoomDeployAckMessage>(GameOpCode.RoomDeployAck);
            Register<RoomDisposeAckMessage>(GameOpCode.RoomDisposeAck);
            Register<PlayerInfoAckMessage>(GameOpCode.PlayerInfoAck);
            Register<ItemBuyItemAckMessage>(GameOpCode.ItemBuyItemAck);
            Register<ItemRepairItemAckMessage>(GameOpCode.ItemRepairItemAck);
            Register<ItemDurabilityItemAckMessage>(GameOpCode.ItemDurabilityItemAck);
            Register<ItemRefundItemAckMessage>(GameOpCode.ItemRefundItemAck);
            Register<MoneyRefreshCashInfoAckMessage>(GameOpCode.MoneyRefreshCashInfoAck);
            Register<AdminActionAckMessage>(GameOpCode.AdminActionAck);
            Register<AdminShowWindowAckMessage>(GameOpCode.AdminShowWindowAck);
            Register<NoticeAdminMessageAckMessage>(GameOpCode.NoticeAdminMessageAck);
            Register<CharacterCurrentSlotInfoAckMessage>(GameOpCode.CharacterCurrentSlotInfoAck);
            Register<ItemRefreshInvalidEquipItemAckMessage>(GameOpCode.ItemRefreshInvalidEquipItemAck);
            Register<ItemClearInvalidEquipItemAckMessage>(GameOpCode.ItemClearInvalidEquipItemAck);
            Register<CharacterAvatarEquipPresetAckMessage>(GameOpCode.CharacterAvatarEquipPresetAck);
            Register<LicenseMyInfoAckMessage>(GameOpCode.LicenseMyInfoAck);
            Register<ClubInfoAckMessage>(GameOpCode.ClubInfoAck);
            Register<ClubHistoryAckMessage>(GameOpCode.ClubHistoryAck);
            Register<ItemEquipBoostItemInfoAckMessage>(GameOpCode.ItemEquipBoostItemInfoAck);
            Register<ClubFindInfoAckMessage>(GameOpCode.ClubFindInfoAck);
            Register<TaskInfoAckMessage>(GameOpCode.TaskInfoAck);
            Register<TaskUpdateAckMessage>(GameOpCode.TaskUpdateAck);
            Register<TaskRequestAckMessage>(GameOpCode.TaskRequestAck);
            Register<TaskRemoveAckMessage>(GameOpCode.TaskRemoveAck);
            Register<MoenyRefreshCoinInfoAckMessage>(GameOpCode.MoenyRefreshCoinInfoAck);
            Register<ItemUseEsperChipItemAckMessage>(GameOpCode.ItemUseEsperChipItemAck);
            Register<RequitalArcadeRewardAckMessage>(GameOpCode.RequitalArcadeRewardAck);
            Register<PlayeArcadeMapInfoAckMessage>(GameOpCode.PlayeArcadeMapInfoAck);
            Register<PlayerArcadeStageInfoAckMessage>(GameOpCode.PlayerArcadeStageInfoAck);
            Register<MoneyRefreshPenInfoAckMessage>(GameOpCode.MoneyRefreshPenInfoAck);
            Register<ItemUseCapsuleAckMessage>(GameOpCode.ItemUseCapsuleAck);
            Register<AdminHGWKickAckMessage>(GameOpCode.AdminHGWKickAck);
            Register<ClubJoinAckMessage>(GameOpCode.ClubJoinAck);
            Register<ClubUnJoinAckMessage>(GameOpCode.ClubUnJoinAck);
            Register<NewShopUpdateCheckAckMessage>(GameOpCode.NewShopUpdateCheckAck);
            Register<NewShopUpdataInfoAckMessage>(GameOpCode.NewShopUpdataInfoAck);
            Register<ItemUseChangeNickAckMessage>(GameOpCode.ItemUseChangeNickAck);
            Register<ItemUseRecordResetAckMessage>(GameOpCode.ItemUseRecordResetAck);
            Register<ItemUseCoinFillingAckMessage>(GameOpCode.ItemUseCoinFillingAck);
            Register<PlayerFindInfoAckMessage>(GameOpCode.PlayerFindInfoAck);
            Register<ItemDiscardItemAckMessage>(GameOpCode.ItemDiscardItemAck);
            Register<ItemInventroyDeleteAckMessage>(GameOpCode.ItemInventroyDeleteAck);
            Register<ClubAddressAckMessage>(GameOpCode.ClubAddressAck);
            Register<ItemUseChangeNickCancelAckMessage>(GameOpCode.ItemUseChangeNickCancelAck);
            Register<RequitalEventItemRewardAckMessage>(GameOpCode.RequitalEventItemRewardAck);
            Register<RoomListInfoAckMessage>(GameOpCode.RoomListInfoAck);
            Register<NickDefaultAckMessage>(GameOpCode.NickDefaultAck);
            Register<RequitalGiveItemResultAckMessage>(GameOpCode.RequitalGiveItemResultAck);
            Register<ShoppingBasketActionAckMessage>(GameOpCode.ShoppingBasketActionAck);
            Register<ShoppingBasketListInfoAckMessage>(GameOpCode.ShoppingBasketListInfoAck);
            Register<RandomShopUpdateRequestAckMessage>(GameOpCode.RandomShopUpdateRequestAck);
            Register<RandomShopUpdateCheckAckMessage>(GameOpCode.RandomShopUpdateCheckAck);
            Register<RandomShopUpdateInfoAckMessage>(GameOpCode.RandomShopUpdateInfoAck);
            Register<RandomShopRollingStartAckMessage>(GameOpCode.RandomShopRollingStartAck);
            Register<RoomInfoRequestAckMessage>(GameOpCode.RoomInfoRequestAck);
            
            Register<NoteGiftItemAckMessage>(GameOpCode.NoteGiftItemAck);
            Register<NoteImportuneItemAckMessage>(GameOpCode.NoteImportuneItemAck);
            Register<NoteGiftItemGainAckMessage>(GameOpCode.NoteGiftItemGainAck);
            Register<RoomQuickJoinAckMessage>(GameOpCode.RoomQuickJoinAck);
            Register<JorbiWebSessionRedirectAckMessage>(GameOpCode.JorbiWebSessionRedirectAck);
            Register<CardGambleAckMessage>(GameOpCode.CardGambleAck);
            Register<NoticeItemGainAckMessage>(GameOpCode.NoticeItemGainAck);
            Register<PromotionPunkinNoticeAckMessage>(GameOpCode.PromotionPunkinNoticeAck);
            Register<PromotionPunkinRankersAckMessage>(GameOpCode.PromotionPunkinRankersAck);
            Register<RequitalLevelAckMessage>(GameOpCode.RequitalLevelAck);
            Register<PromotionAttendanceInfoAckMessage>(GameOpCode.PromotionAttendanceInfoAck);
            Register<PromotionAttendanceGiftItemAckMessage>(GameOpCode.PromotionAttendanceGiftItemAck);
            Register<PromotionCoinEventAckMessage>(GameOpCode.PromotionCoinEventAck);
            Register<PromotionCoinEventDropCoinAckMessage>(GameOpCode.PromotionCoinEventDropCoinAck);
            Register<EnchantEnchantItemAckMessage>(GameOpCode.EnchantEnchantItemAck);
            Register<EnchantRefreshEnchantGaugeAckMessage>(GameOpCode.EnchantRefreshEnchantGaugeAck);
            Register<NoticeEnchantAckMessage>(GameOpCode.NoticeEnchantAck);
            Register<PromotionCardShuffleAckMessage>(GameOpCode.PromotionCardShuffleAck);
            Register<ItemClearEsperChipAckMessage>(GameOpCode.ItemClearEsperChipAck);
            Register<ChallengeMyInfoAckMessage>(GameOpCode.ChallengeMyInfoAck);
            Register<KRShutDownAckMessage>(GameOpCode.KRShutDownAck);
            Register<RequitalChallengeAckMessage>(GameOpCode.RequitalChallengeAck);
            Register<MapOpenInfosMessage>(GameOpCode.MapOpenInfos);
            Register<PromotionCouponEventAckMessage>(GameOpCode.PromotionCouponEventAck);
            Register<TutorialCompletedAckMessage>(GameOpCode.TutorialCompletedAck);
            Register<ExpRefreshInfoAckMessage>(GameOpCode.ExpRefreshInfoAck);
            Register<PromotionActiveAckMessage>(GameOpCode.PromotionActiveAck);

            // C2S
            Register<CharacterCreateReqMessage>(GameOpCode.CharacterCreateReq);
            Register<CharacterSelectReqMessage>(GameOpCode.CharacterSelectReq);
            Register<CharacterDeleteReqMessage>(GameOpCode.CharacterDeleteReq);
            Register<LoginRequestReqMessage>(GameOpCode.LoginRequestReq);
            Register<RoomQuickStartReqMessage>(GameOpCode.RoomQuickStartReq);
            Register<RoomMakeReqMessage>(GameOpCode.RoomMakeReq);
            Register<NickCheckReqMessage>(GameOpCode.NickCheckReq);
            Register<ItemUseItemReqMessage>(GameOpCode.ItemUseItemReq);
            Register<RoomLeaveReqMessage>(GameOpCode.RoomLeaveReq);
            Register<TimeSyncReqMessage>(GameOpCode.TimeSyncReq);
            Register<AdminShowWindowReqMessage>(GameOpCode.AdminShowWindowReq);
            Register<ClubInfoReqMessage>(GameOpCode.ClubInfoReq);
            Register<ChannelEnterReqMessage>(GameOpCode.ChannelEnterReq);
            Register<ChannelLeaveReqMessage>(GameOpCode.ChannelLeaveReq);
            Register<ChannelInfoReqMessage>(GameOpCode.ChannelInfoReq);
            Register<RoomEnterReqMessage>(GameOpCode.RoomEnterReq);
            Register<PlayerInfoReqMessage>(GameOpCode.PlayerInfoReq);
            Register<ItemBuyItemReqMessage>(GameOpCode.ItemBuyItemReq);
            Register<ItemRepairItemReqMessage>(GameOpCode.ItemRepairItemReq);
            Register<ItemRefundItemReqMessage>(GameOpCode.ItemRefundItemReq);
            Register<AdminActionReqMessage>(GameOpCode.AdminActionReq);
            Register<CharacterActiveEquipPresetReqMessage>(GameOpCode.CharacterActiveEquipPresetReq);
            Register<LicenseGainReqMessage>(GameOpCode.LicenseGainReq);
            Register<ClubNoticeChangeReqMessage>(GameOpCode.ClubNoticeChangeReq);
            Register<ClubInfoByIDReqMessage>(GameOpCode.ClubInfoByIDReq);
            Register<ClubInfoByNameReqMessage>(GameOpCode.ClubInfoByNameReq);
            Register<ItemInventoryInfoReqMessage>(GameOpCode.ItemInventoryInfoReq);
            Register<TaskNotifyReqMessage>(GameOpCode.TaskNotifyReq);
            Register<TaskReguestReqMessage>(GameOpCode.TaskReguestReq);
            Register<LicenseExerciseReqMessage>(GameOpCode.LicenseExerciseReq);
            Register<ItemUseCoinReqMessage>(GameOpCode.ItemUseCoinReq);
            Register<ItemUseEsperChipReqMessage>(GameOpCode.ItemUseEsperChipReq);
            Register<PlayerBadUserReqMessage>(GameOpCode.PlayerBadUserReq);
            Register<ClubJoinReqMessage>(GameOpCode.ClubJoinReq);
            Register<ClubUnJoinReqMessage>(GameOpCode.ClubUnJoinReq);
            Register<NewShopUpdateCheckReqMessage>(GameOpCode.NewShopUpdateCheckReq);
            Register<ItemUseChangeNickReqMessage>(GameOpCode.ItemUseChangeNickReq);
            Register<ItemUseRecordResetReqMessage>(GameOpCode.ItemUseRecordResetReq);
            Register<ItemUseCoinFillingReqMessage>(GameOpCode.ItemUseCoinFillingReq);
            Register<PlayerFindInfoReqMessage>(GameOpCode.PlayerFindInfoReq);
            Register<ItemDiscardItemReqMessage>(GameOpCode.ItemDiscardItemReq);
            Register<ItemUseCapsuleReqMessage>(GameOpCode.ItemUseCapsuleReq);
            Register<ClubAddressReqMessage>(GameOpCode.ClubAddressReq);
            Register<ClubHistoryReqMessage>(GameOpCode.ClubHistoryReq);
            Register<ItemUseChangeNickCancelReqMessage>(GameOpCode.ItemUseChangeNickCancelReq);
            Register<TutorialCompletedReqMessage>(GameOpCode.TutorialCompletedReq);
            Register<CharacterFirstCreateReqMessage>(GameOpCode.CharacterFirstCreateReq);
            Register<ShoppingBasketActionReqMessage>(GameOpCode.ShoppingBasketActionReq);
            Register<ShoppingBasketDeleteReqMessage>(GameOpCode.ShoppingBasketDeleteReq);
            Register<RandomShopUpdateCheckReqMessage>(GameOpCode.RandomShopUpdateCheckReq);
            Register<RandomShopRollingStartReqMessage>(GameOpCode.RandomShopRollingStartReq);
            Register<RoomInfoRequestReqMessage>(GameOpCode.RoomInfoRequestReq);
            Register<NoteGiftItemReqMessage>(GameOpCode.NoteGiftItemReq);
            Register<NoteImportuneItemReqMessage>(GameOpCode.NoteImportuneItemReq);
            Register<NoteGiftItemGainReqMessage>(GameOpCode.NoteGiftItemGainReq);
            Register<RoomQuickJoinReqMessage>(GameOpCode.RoomQuickJoinReq);
            Register<MoneyRefreshCashInfoReqMessage>(GameOpCode.MoneyRefreshCashInfoReq);
            Register<CardGambleReqMessage>(GameOpCode.CardGambleReq);
            Register<PromotionAttendanceGiftItemReqMessage>(GameOpCode.PromotionAttendanceGiftItemReq);
            Register<PromotionCoinEventUseCoinReqMessage>(GameOpCode.PromotionCoinEventUseCoinReq);
            Register<ItemEnchanReqMessage>(GameOpCode.ItemEnchanReq);
            Register<CPromotionCardShuffleReqMessage>(GameOpCode.CPromotionCardShuffleReq);
            Register<BillingCashInfoReqMessage>(GameOpCode.BillingCashInfoReq);
            Register<PromotionCouponEventReqMessage>(GameOpCode.PromotionCouponEventReq);
        }
    }
}
