using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Game
{
    [BlubContract]
    public class CharacterCreateReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(1)]
        public CharacterStyle Style { get; set; }
    }

    [BlubContract]
    public class CharacterSelectReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class CharacterDeleteReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class LoginRequestReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public string Username { get; set; }

        [BlubMember(2)]
        public Version Version { get; set; }

        [BlubMember(3)]
        public uint Unk3 { get; set; }

        [BlubMember(4)]
        public ulong AccountId { get; set; }

        [BlubMember(5)]
        public string SessionId { get; set; }

        [BlubMember(6)]
        public string Unk4 { get; set; }

        [BlubMember(7)]
        public bool KickConnection { get; set; }

        [BlubMember(8)]
        public string Unk5 { get; set; }

        [BlubMember(9)]
        public uint Unk6 { get; set; }

        [BlubMember(10)]
        public Data.Auth.AeriaTokenDto AeriaToken { get; set; }
    }

    [BlubContract]
    public class RoomQuickStartReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte GameRule { get; set; }
    }

    [BlubContract]
    public class RoomMakeReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public MakeRoomDto Room { get; set; }
    }

    [BlubContract]
    public class NickCheckReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class ItemUseItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public UseItemAction Action { get; set; }

        [BlubMember(1)]
        public byte CharacterSlot { get; set; }

        [BlubMember(2)]
        public byte EquipSlot { get; set; }

        [BlubMember(3)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class RoomLeaveReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class TimeSyncReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Time { get; set; }
    }

    [BlubContract]
    public class AdminShowWindowReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ClubInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ChannelEnterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class ChannelLeaveReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class ChannelInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ChannelInfoRequest Request { get; set; }
    }

    [BlubContract]
    public class RoomEnterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }

        [BlubMember(1)]
        public string Password { get; set; }

        // player gamemode and ?
        [BlubMember(2)]
        public byte Unk1 { get; set; }

        [BlubMember(3)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class PlayerInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class ItemBuyItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ShopItemDto[] Items { get; set; }
    }

    [BlubContract]
    public class ItemRepairItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }
    }

    [BlubContract]
    public class ItemRefundItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class AdminActionReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Command { get; set; }
    }

    [BlubContract]
    public class CharacterActiveEquipPresetReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class LicenseGainReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class ClubNoticeChangeReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubInfoByIdReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubInfoByNameReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ItemInventoryInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class TaskNotifyReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }

        [BlubMember(1)]
        public ushort Progress { get; set; }
    }

    [BlubContract]
    public class TaskReguestReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public uint TaskId { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; } // slot?
    }

    [BlubContract]
    public class LicenseExerciseReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class ItemUseCoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class ItemUseEsperChipReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class PlayerBadUserReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class ClubJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubUnJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class NewShopUpdateCheckReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string PriceVersion { get; set; }

        [BlubMember(1)]
        public string EffectVersion { get; set; }

        [BlubMember(2)]
        public string ItemVersion { get; set; }

        [BlubMember(3)]
        public string UniqueItemVersion { get; set; }

        [BlubMember(4)]
        public uint PriceLength { get; set; }

        [BlubMember(5)]
        public uint EffectLength { get; set; }

        [BlubMember(6)]
        public uint ItemLength { get; set; }

        [BlubMember(7)]
        public uint UniqueItemLength { get; set; }
    }

    [BlubContract]
    public class ItemUseChangeNickReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class ItemUseRecordResetReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class ItemUseCoinFillingReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class PlayerFindInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class ItemDiscardItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class ItemUseCapsuleReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class ClubAddressReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RequestId { get; set; }

        [BlubMember(1)]
        public uint LanguageId { get; set; }

        [BlubMember(2)]
        public uint Command { get; set; }
    }

    [BlubContract]
    public class ClubHistoryReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ItemUseChangeNickCancelReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class TutorialCompletedReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CharacterFirstCreateReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }

        [BlubMember(1)]
        public int Unk { get; set; } // Probably gender

        [BlubMember(1)]
        [BlubSerializer(typeof(FixedArraySerializer), 8)]
        public ItemNumber[] Items { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketActionReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public short Unk5 { get; set; }

        [BlubMember(5)]
        public byte Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketDeleteReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk { get; set; }
    }

    [BlubContract]
    public class RandomShopUpdateCheckReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class RandomShopRollingStartReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class RoomInfoReqeustReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }
    }

    [BlubContract]
    public class NoteGiftItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public ShopItemDto ShopItem { get; set; }

        [BlubMember(6)]
        public ulong Unk7 { get; set; }

        public NoteGiftItemReqMessage()
        {
            ShopItem = new ShopItemDto();
        }
    }

    [BlubContract]
    public class NoteImportuneItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public string Unk3 { get; set; }

        [BlubMember(3)]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public ShopItemDto ShopItem { get; set; }

        public NoteImportuneItemReqMessage()
        {
            ShopItem = new ShopItemDto();
        }
    }

    [BlubContract]
    public class NoteGiftItemGainReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class RoomQuickJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class MoneyRefreshCashInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CardGambleReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class PromotionAttendanceGiftItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class PromotionCoinEventUseCoinReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ItemEnchanReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }
    }

    [BlubContract]
    public class CPromotionCardShuffleReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class BillingCashInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class XTrapDetourMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class PromotionCouponEventReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class CollectBookUpdateCheckReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CollectBookInventoryInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CollectBookItemRegistReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public BookItemDto[] Unk { get; set; }
    }

    [BlubContract]
    public class CollectBookUseRewardReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class UseInstantItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class UseInstantItemRemoveEffectReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class PromotionRouletteMachineStartReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class GameGuardCSAuthReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte[] Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class GameGuardHackReportReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte[] Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionXMasCardUseReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class PromotionNewYearCardUseReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class HackShieldMakeResponseMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte[] Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomMakeReq2Message : IGameMessage
    {
        [BlubMember(0)]
        public MakeRoom2Dto Room { get; set; }

        public RoomMakeReq2Message()
        {
            Room = new MakeRoom2Dto();
        }
    }

    [BlubContract]
    public class AlchemyCombinationReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public AlchemyCombinationDto[] Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class AlchemyDecompositionReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionItemPaymentReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ItemUseFillExpReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }
    }

    [BlubContract]
    public class DailyMissionRewardReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class DailyMissionInitReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class DailyMissionNextStepReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class AchieveMissionReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class AchieveMissionRewardReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class BtcClearReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class NewItemEnchanReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }
    }

    [BlubContract]
    public class ItemUseJewelItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }
    }

    [BlubContract]
    public class XignCodeAliveReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(FixedArraySerializer), 512)]
        public byte[] Unk2 { get; set; }
    }

    [BlubContract]
    public class EsperEnchantReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }
    }

    [BlubContract]
    public class MatchStartReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchStopReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchListReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchInviteReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class BattleInvitesReceivedResultMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ReMatchReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }
    }

    [BlubContract]
    public class MatchVoteBeginMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchClubMarkReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchPointReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class MatchRoomQuitReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ClubNoticePointRefreshReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ClubNoticeRecordRefreshReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ClubSearchRoomReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class ClubStadiumEditMapDataReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubStadiumEditBlastinfoEditReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public byte Unk4 { get; set; }
    }

    [BlubContract]
    public class ClubStadiumInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class ClubStadiumSelectReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ClubOtherClubinfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }
}
