using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

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

        [BlubMember(1, typeof(StringSerializer))]
        public string Username { get; set; }

        [BlubMember(2, typeof(VersionSerializer))]
        public Version Version { get; set; }

        [BlubMember(3)]
        public int Unk2 { get; set; }

        [BlubMember(4)]
        public ulong AccountId { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string SessionId { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(7)]
        public bool KickConnection { get; set; }
        
        [BlubMember(8, typeof(StringSerializer))]
        public string Unk4 { get; set; }
        
        [BlubMember(9)]
        public uint Unk6 { get; set; }
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
        [BlubMember(0, typeof(StringSerializer))]
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
        public RoomLeaveReason Reason { get; set; }
    }

    [BlubContract]
    public class TimeSyncReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Time { get; set; }
    }

    [BlubContract]
    public class AdminShowWindowReqMessage : IGameMessage
    { }

    [BlubContract]
    public class ClubInfoReqMessage : IGameMessage
    { }

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

        [BlubMember(1, typeof(StringSerializer))]
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
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ShopItemDto[] Items { get; set; }
    }

    [BlubContract]
    public class ItemRepairItemReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
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
        [BlubMember(0, typeof(StringSerializer))]
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
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubInfoByIDReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class ClubInfoByNameReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
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

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class ClubUnJoinReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class NewShopUpdateCheckReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Date01 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Date02 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Date03 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Date04 { get; set; }

        [BlubMember(4)]
        public uint Checksum01 { get; set; }

        [BlubMember(5)]
        public uint Checksum02 { get; set; }

        [BlubMember(6)]
        public uint Checksum03 { get; set; }

        [BlubMember(7)]
        public uint Checksum04 { get; set; }
    }

    [BlubContract]
    public class ItemUseChangeNickReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
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
        [BlubMember(0, typeof(StringSerializer))]
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
    { }

    [BlubContract]
    public class ItemUseChangeNickCancelReqMessage : IGameMessage
    { }

    [BlubContract]
    public class TutorialCompletedReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CharacterFirstCreateReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(1)]
        public int Unk1 { get; set; }

        [BlubMember(2, typeof(FixedArraySerializer), 8)]
        public int[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketActionReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }

        [BlubMember(1)]
        public ShopItemDto ShopItem { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketDeleteReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public long[] Unk { get; set; }
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
    public class RoomInfoRequestReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }
    }

    [BlubContract]
    public class NoteGiftItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public ShopItemDto Unk6 { get; set; }

        [BlubMember(6)]
        public long Unk7 { get; set; }
    }

    [BlubContract]
    public class NoteImportuneItemReqMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public ShopItemDto Unk6 { get; set; }
    }

    [BlubContract]
    public class NoteGiftItemGainReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }
    }

    [BlubContract]
    public class RoomQuickJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class MoneyRefreshCashInfoReqMessage : IGameMessage
    { }

    [BlubContract]
    public class CardGambleReqMessage : IGameMessage
    { }

    [BlubContract]
    public class PromotionAttendanceGiftItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class PromotionCoinEventUseCoinReqMessage : IGameMessage
    { }

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
    { }

    [BlubContract]
    public class BillingCashInfoReqMessage : IGameMessage
    { }

    [BlubContract]
    public class PromotionCouponEventReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }
    }
}
