using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Game
{
    [BlubContract]
    public class CCreateCharacterReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(1)]
        public CharacterStyle Style { get; set; }
    }

    [BlubContract]
    public class CSelectCharacterReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class CDeleteCharacterReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class CLoginReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Username { get; set; }

        [BlubMember(2, typeof(VersionSerializer))]
        public Version Version { get; set; }

        [BlubMember(3)]
        public uint Unk3 { get; set; }

        [BlubMember(4)]
        public ulong AccountId { get; set; }

        [BlubMember(5, typeof(StringSerializer))]
        public string SessionId { get; set; }

        [BlubMember(6, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        [BlubMember(7)]
        public bool KickConnection { get; set; }
    }

    [BlubContract]
    public class CQuickStartReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CMakeRoomReqMessage : GameMessage
    {
        [BlubMember(0)]
        public MakeRoomDto Room { get; set; }
    }

    [BlubContract]
    public class CCreateNickReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CCheckNickReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CUseItemReqMessage : GameMessage
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
    public class CJoinTunnelInfoReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CTimeSyncReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Time { get; set; }
    }

    [BlubContract]
    public class CGameArgPingReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CAdminShowWindowReqMessage : GameMessage
    { }

    [BlubContract]
    public class CClubInfoReqMessage : GameMessage
    { }

    [BlubContract]
    public class CIngameEquipCheckReqMessage : GameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] ItemIds { get; set; }
    }

    [BlubContract]
    public class CUseCoinRandomShopChanceReqMessage : GameMessage
    { }

    [BlubContract]
    public class CChannelEnterReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class CChannelLeaveReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class CGetChannelInfoReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ChannelInfoRequest Request { get; set; }
    }

    [BlubContract]
    public class CGameRoomEnterReqMessage : GameMessage
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
    public class CGetPlayerInfoReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CBuyItemReqMessage : GameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ShopItemDto[] Items { get; set; }
    }

    [BlubContract]
    public class CRepairItemReqMessage : GameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }
    }

    [BlubContract]
    public class CRefundItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CAdminActionReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Command { get; set; }
    }

    [BlubContract]
    public class CActiveEquipPresetReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CLicensedReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class CClubNoticeChangeReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetClubInfoReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetClubInfoByNameReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetInventoryItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CTaskNotifyReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }

        [BlubMember(1)]
        public ushort Progress { get; set; }
    }

    [BlubContract]
    public class CTaskRequestReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public uint TaskId { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; } // slot?
    }

    [BlubContract]
    public class CRandomShopRollingStartReqMessage : GameMessage
    {
        [BlubMember(0)]
        public bool IsWeapon { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        [BlubMember(3)]
        public byte Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }

        [BlubMember(5)]
        public uint Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }
    }

    [BlubContract]
    public class CRandomShopItemGetReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CRandomShopItemSaleReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CExerciseLicenceReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class CUseCoinReqGSMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CApplyEsperChipItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class CBadUserReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CClubJoinReqMessage : GameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class CClubUnJoinReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CNewShopUpdateCheckReqMessage : GameMessage
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
    public class CUseChangeNickNameItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CUseResetRecordItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CUseCoinFillingItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CGetUserInfoListReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint Unk4 { get; set; }
    }

    [BlubContract]
    public class CFindUserReqMessage : GameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CDiscardItemReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CUseCapsuleReqMessage : GameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CSaveConfigPermissionNotifyReqMessage : GameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Settings { get; set; }
    }

    [BlubContract]
    public class CClubAddressReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint RequestId { get; set; }

        [BlubMember(1)]
        public uint LanguageId { get; set; }

        [BlubMember(2)]
        public uint Command { get; set; }
    }

    [BlubContract]
    public class CSmallLoudSpeakerReqMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class CClubHistoryReqMessage : GameMessage
    { }

    [BlubContract]
    public class CChangeNickCancelReqMessage : GameMessage
    { }

    [BlubContract]
    public class CEnableAccountStatusAckMessage : GameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }
}
