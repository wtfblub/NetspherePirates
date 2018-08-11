using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.Game
{
    [BlubContract]
    public class CCreateCharacterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(1)]
        public CharacterStyle Style { get; set; }
    }

    [BlubContract]
    public class CSelectCharacterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class CDeleteCharacterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class CLoginReqMessage : IGameMessage
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
    }

    [BlubContract]
    public class CQuickStartReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte GameRule { get; set; }
    }

    [BlubContract]
    public class CMakeRoomReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public MakeRoomDto Room { get; set; }
    }

    [BlubContract]
    public class CCreateNickReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CCheckNickReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CUseItemReqMessage : IGameMessage
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
    public class CJoinTunnelInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CTimeSyncReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Time { get; set; }
    }

    [BlubContract]
    public class CGameArgPingReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CAdminShowWindowReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CClubInfoReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CIngameEquipCheckReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] ItemIds { get; set; }
    }

    [BlubContract]
    public class CUseCoinRandomShopChanceReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CChannelEnterReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class CChannelLeaveReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Channel { get; set; }
    }

    [BlubContract]
    public class CGetChannelInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ChannelInfoRequest Request { get; set; }
    }

    [BlubContract]
    public class CGameRoomEnterReqMessage : IGameMessage
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
    public class CGetPlayerInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CBuyItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ShopItemDto[] Items { get; set; }
    }

    [BlubContract]
    public class CRepairItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }
    }

    [BlubContract]
    public class CRefundItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CAdminActionReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Command { get; set; }
    }

    [BlubContract]
    public class CActiveEquipPresetReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CLicensedReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class CClubNoticeChangeReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetClubInfoReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetClubInfoByNameReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CGetInventoryItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CTaskNotifyReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }

        [BlubMember(1)]
        public ushort Progress { get; set; }
    }

    [BlubContract]
    public class CTaskRequestReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public uint TaskId { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; } // slot?
    }

    [BlubContract]
    public class CRandomShopRollingStartReqMessage : IGameMessage
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
    public class CRandomShopItemGetReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CRandomShopItemSaleReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CExerciseLicenceReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ItemLicense License { get; set; }
    }

    [BlubContract]
    public class CUseCoinReqGSMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CApplyEsperChipItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class CBadUserReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class CClubJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class CClubUnJoinReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class CNewShopUpdateCheckReqMessage : IGameMessage
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
    public class CUseChangeNickNameItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CUseResetRecordItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CUseCoinFillingItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CGetUserInfoListReqMessage : IGameMessage
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
    public class CFindUserReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public string Nickname { get; set; }
    }

    [BlubContract]
    public class CDiscardItemReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CUseCapsuleReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class CSaveConfigPermissionNotifyReqMessage : IGameMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Settings { get; set; }
    }

    [BlubContract]
    public class CClubAddressReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RequestId { get; set; }

        [BlubMember(1)]
        public uint LanguageId { get; set; }

        [BlubMember(2)]
        public uint Command { get; set; }
    }

    [BlubContract]
    public class CSmallLoudSpeakerReqMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public string Unk2 { get; set; }
    }

    [BlubContract]
    public class CClubHistoryReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CChangeNickCancelReqMessage : IGameMessage
    {
    }

    [BlubContract]
    public class CEnableAccountStatusAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }
}
