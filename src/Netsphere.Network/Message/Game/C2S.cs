using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Game
{
    public class CCreateCharacterReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }

        [Serialize(1)]
        public CharacterStyle Style { get; set; }
    }

    public class CSelectCharacterReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }
    }

    public class CDeleteCharacterReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }
    }

    public class CLoginReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Username { get; set; }

        [Serialize(2, typeof(VersionSerializer))]
        public Version Version { get; set; }

        [Serialize(3)]
        public uint Unk3 { get; set; }

        [Serialize(4)]
        public ulong AccountId { get; set; }

        [Serialize(5, typeof(StringSerializer))]
        public string SessionId { get; set; }

        [Serialize(6, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [Serialize(7)]
        public bool KickConnection { get; set; }
    }

    public class CQuickStartReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CMakeRoomReqMessage : GameMessage
    {
        [Serialize(0)]
        public MakeRoomDto Room { get; set; }
    }

    public class CCreateNickReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class CCheckNickReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class CUseItemReqMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public UseItemAction Action { get; set; }

        [Serialize(1)]
        public byte CharacterSlot { get; set; }

        [Serialize(2)]
        public byte EquipSlot { get; set; }

        [Serialize(3)]
        public ulong ItemId { get; set; }
    }

    public class CJoinTunnelInfoReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CTimeSyncReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Time { get; set; }
    }

    public class CGameArgPingReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class CAdminShowWindowReqMessage : GameMessage
    { }

    public class CClubInfoReqMessage : GameMessage
    { }

    public class CIngameEquipCheckReqMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] ItemIds { get; set; }
    }

    public class CUseCoinRandomShopChanceReqMessage : GameMessage
    { }

    public class CChannelEnterReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Channel { get; set; }
    }

    public class CChannelLeaveReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Channel { get; set; }
    }

    public class CGetChannelInfoReqMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ChannelInfoRequest Request { get; set; }
    }

    public class CGameRoomEnterReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint RoomId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Password { get; set; }

        // player gamemode and ?
        [Serialize(2)]
        public byte Unk1 { get; set; }

        [Serialize(3)]
        public byte Unk2 { get; set; }
    }

    public class CGetPlayerInfoReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class CBuyItemReqMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ShopItemDto[] Items { get; set; }
    }

    public class CRepairItemReqMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }
    }

    public class CRefundItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CAdminActionReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Command { get; set; }
    }

    public class CActiveEquipPresetReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CLicensedReqMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ItemLicense License { get; set; }
    }

    public class CClubNoticeChangeReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    public class CGetClubInfoReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    public class CGetClubInfoByNameReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    public class CGetInventoryItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CTaskNotifyReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint TaskId { get; set; }

        [Serialize(1)]
        public ushort Progress { get; set; }
    }

    public class CTaskRequestReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public uint TaskId { get; set; }

        [Serialize(2)]
        public byte Unk2 { get; set; } // slot?
    }

    public class CRandomShopRollingStartReqMessage : GameMessage
    {
        [Serialize(0)]
        public bool IsWeapon { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2)]
        public byte Unk3 { get; set; }

        [Serialize(3)]
        public byte Unk4 { get; set; }

        [Serialize(4)]
        public byte Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }

        [Serialize(6)]
        public int Unk7 { get; set; }

        [Serialize(7)]
        public int Unk8 { get; set; }
    }

    public class CRandomShopItemGetReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CRandomShopItemSaleReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CExerciseLicenceReqMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ItemLicense License { get; set; }
    }

    public class CUseCoinReqGSMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class CApplyEsperChipItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }
    }

    public class CBadUserReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class CClubJoinReqMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
    }

    public class CClubUnJoinReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    public class CNewShopUpdateCheckReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Date01 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Date02 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Date03 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Date04 { get; set; }

        [Serialize(4)]
        public uint Checksum01 { get; set; }

        [Serialize(5)]
        public uint Checksum02 { get; set; }

        [Serialize(6)]
        public uint Checksum03 { get; set; }

        [Serialize(7)]
        public uint Checksum04 { get; set; }
    }

    public class CUseChangeNickNameItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class CUseResetRecordItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CUseCoinFillingItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CGetUserInfoListReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }
    }

    public class CFindUserReqMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class CDiscardItemReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CUseCapsuleReqMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class CSaveConfigPermissionNotifyReqMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Settings { get; set; }
    }

    public class CClubAddressReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint RequestId { get; set; }

        [Serialize(1)]
        public uint LanguageId { get; set; }

        [Serialize(2)]
        public uint Command { get; set; }
    }

    public class CSmallLoudSpeakerReqMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
    }

    public class CClubHistoryReqMessage : GameMessage
    { }

    public class CChangeNickCancelReqMessage : GameMessage
    { }

    public class CEnableAccountStatusAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }
}
