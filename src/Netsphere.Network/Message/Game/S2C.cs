using System;
using System.IO;
using System.Runtime.CompilerServices;
using BlubLib;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;
using ProudNet.Serialization.Serializers;

namespace Netsphere.Network.Message.Game
{
    [BlubContract]
    public class LoginReguestAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public GameLoginResult Result { get; set; }

        [BlubMember(2, typeof(UnixTimeSerializer))]
        public DateTimeOffset ServerTime { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        public LoginReguestAckMessage()
        {
            Unk1 = "";
            Unk2 = "";
            ServerTime = DateTimeOffset.Now;
        }

        public LoginReguestAckMessage(GameLoginResult result, ulong accountId)
            : this()
        {
            AccountId = accountId;
            Result = result;
        }

        public LoginReguestAckMessage(GameLoginResult result)
            : this()
        {
            Result = result;
        }
    }

    [BlubContract]
    public class PlayerAccountInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public PlayerAccountInfoDto Info { get; set; }

        public PlayerAccountInfoAckMessage()
        {
            Info = new PlayerAccountInfoDto();
        }

        public PlayerAccountInfoAckMessage(PlayerAccountInfoDto info)
        {
            Info = info;
        }
    }

    [BlubContract]
    public class CharacterCurrentInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; }

        [BlubMember(3)]
        public CharacterStyle Style { get; set; }

        public CharacterCurrentInfoAckMessage()
        {
            Unk1 = 1; // max skill?
            Unk2 = 3; // max weapons?
        }
    }

    [BlubContract]
    public class CharacterCurrentItemInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(3, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Weapons { get; set; }

        [BlubMember(4, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Skills { get; set; }

        [BlubMember(5, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Clothes { get; set; }

        public CharacterCurrentItemInfoAckMessage()
        {
            Weapons = new ulong[9];
            Skills = new ulong[1];
            Clothes = new ulong[7];
        }
    }

    [BlubContract]
    public class ItemInventoryInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Items { get; set; }

        public ItemInventoryInfoAckMessage()
        {
            Items = Array.Empty<ItemDto>();
        }
    }

    [BlubContract]
    public class CharacterDeleteAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        public CharacterDeleteAckMessage()
        { }

        public CharacterDeleteAckMessage(byte slot)
        {
            Slot = slot;
        }
    }

    [BlubContract]
    public class CharacterSelectAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        public CharacterSelectAckMessage()
        { }

        public CharacterSelectAckMessage(byte slot)
        {
            Slot = slot;
        }
    }

    [BlubContract]
    public class CSuccessCreateCharacterAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Slot { get; set; }

        [BlubMember(1)]
        public CharacterStyle Style { get; set; }

        [BlubMember(2)]
        public byte MaxSkills { get; set; }

        [BlubMember(3)]
        public byte MaxWeapons { get; set; }

        public CSuccessCreateCharacterAckMessage()
        {
            MaxSkills = 1;
            MaxWeapons = 3;
        }

        public CSuccessCreateCharacterAckMessage(byte slot, CharacterStyle style)
            : this()
        {
            Slot = slot;
            Style = style;
        }
    }

    [BlubContract]
    public class ServerResultAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ServerResult Result { get; set; }

        public ServerResultAckMessage()
        { }

        public ServerResultAckMessage(ServerResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class NickCheckAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(IntBooleanSerializer))]
        public bool IsTaken { get; set; }

        public NickCheckAckMessage()
        { }

        public NickCheckAckMessage(bool isTaken)
        {
            IsTaken = isTaken;
        }
    }

    [BlubContract]
    public class ItemUseItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte CharacterSlot { get; set; }

        [BlubMember(1)]
        public byte EquipSlot { get; set; }

        [BlubMember(2)]
        public ulong ItemId { get; set; }

        [BlubMember(3)]
        public UseItemAction Action { get; set; }

        public ItemUseItemAckMessage()
        { }

        public ItemUseItemAckMessage(UseItemAction action, byte characterSlot, byte equipSlot, ulong itemId)
        {
            CharacterSlot = characterSlot;
            EquipSlot = equipSlot;
            ItemId = itemId;
            Action = action;
        }
    }

    [BlubContract]
    public class ItemUpdateInventoryAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public InventoryAction Action { get; set; }

        [BlubMember(1)]
        public ItemDto Item { get; set; }

        public ItemUpdateInventoryAckMessage()
        {
            Item = new ItemDto();
        }

        public ItemUpdateInventoryAckMessage(InventoryAction action, ItemDto item)
        {
            Action = action;
            Item = item;
        }
    }

    [BlubContract]
    public class RoomCurrentCharacterSlotAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1)]
        public byte Slot { get; set; }

        public RoomCurrentCharacterSlotAckMessage()
        { }

        public RoomCurrentCharacterSlotAckMessage(uint unk, byte slot)
        {
            Unk = unk;
            Slot = slot;
        }
    }

    [BlubContract]
    public class RoomEnterPlayerInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public RoomPlayerDto Player { get; set; }

        public RoomEnterPlayerInfoAckMessage()
        {
            Player = new RoomPlayerDto();
        }

        public RoomEnterPlayerInfoAckMessage(RoomPlayerDto plr)
        {
            Player = plr;
        }
    }

    [BlubContract]
    public class RoomEnterClubInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public PlayerClubInfoDto Player { get; set; }

        public RoomEnterClubInfoAckMessage()
        {
            Player = new PlayerClubInfoDto();
        }
    }

    [BlubContract]
    public class RoomPlayerInfoListForEnterPlayerAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public RoomPlayerDto[] Players { get; set; }

        public RoomPlayerInfoListForEnterPlayerAckMessage()
        {
            Players = Array.Empty<RoomPlayerDto>();
        }

        public RoomPlayerInfoListForEnterPlayerAckMessage(RoomPlayerDto[] players)
        {
            Players = players;
        }
    }

    [BlubContract]
    public class RoomClubInfoListForEnterPlayerAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public PlayerClubInfoDto[] Players { get; set; }

        public RoomClubInfoListForEnterPlayerAckMessage()
        {
            Players = Array.Empty<PlayerClubInfoDto>();
        }
    }

    [BlubContract]
    public class RoomEnterRoomInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public EnterRoomInfoDto RoomInfo { get; set; }

        public RoomEnterRoomInfoAckMessage()
        {
            RoomInfo = new EnterRoomInfoDto();
        }

        public RoomEnterRoomInfoAckMessage(EnterRoomInfoDto roomInfo)
        {
            RoomInfo = roomInfo;
        }
    }

    [BlubContract]
    public class RoomLeavePlayerInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public RoomLeavePlayerInfoAckMessage()
        { }

        public RoomLeavePlayerInfoAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class TimeSyncAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint ClientTime { get; set; }

        [BlubMember(1)]
        public uint ServerTime { get; set; }
    }

    [BlubContract]
    public class RoomChangeRoomInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public RoomDto Room { get; set; }

        public RoomChangeRoomInfoAckMessage()
        {
            Room = new RoomDto();
        }

        public RoomChangeRoomInfoAckMessage(RoomDto room)
        {
            Room = room;
        }
    }

    [BlubContract]
    public class NewShopUpdateEndAckMessage : IGameMessage
    { }

    [BlubContract]
    public class ChannelListInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ChannelInfoDto[] Channels { get; set; }

        public ChannelListInfoAckMessage()
        {
            Channels = Array.Empty<ChannelInfoDto>();
        }

        public ChannelListInfoAckMessage(ChannelInfoDto[] channels)
        {
            Channels = channels;
        }
    }

    [BlubContract]
    public class RoomDeployAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public RoomDto Room { get; set; }

        public RoomDeployAckMessage()
        {
            Room = new RoomDto();
        }

        public RoomDeployAckMessage(RoomDto room)
        {
            Room = room;
        }
    }

    [BlubContract]
    public class RoomDisposeAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint RoomId { get; set; }

        public RoomDisposeAckMessage()
        { }

        public RoomDisposeAckMessage(uint roomId)
        {
            RoomId = roomId;
        }
    }

    [BlubContract]
    public class PlayerInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public byte Unk2 { get; set; }

        [BlubMember(3)]
        public uint Unk3 { get; set; }

        [BlubMember(4)]
        public short Unk4 { get; set; }

        [BlubMember(5)]
        public short Unk5 { get; set; }

        [BlubMember(6)]
        public int Unk6 { get; set; }

        [BlubMember(7)]
        public byte Unk7 { get; set; }

        [BlubMember(7)]
        public int Unk8 { get; set; }

        [BlubMember(8)]
        public byte Unk9 { get; set; }

        [BlubMember(9)]
        public short Unk10 { get; set; }

        [BlubMember(10)]
        public byte Unk11 { get; set; }

        [BlubMember(11)]
        public int Unk12 { get; set; }

        [BlubMember(12)]
        public int Unk13 { get; set; }

        [BlubMember(13)]
        public int Unk14 { get; set; }

        [BlubMember(14)]
        public int Unk15 { get; set; }

        [BlubMember(15)]
        public int Unk16 { get; set; }

        [BlubMember(16)]
        public int Unk17 { get; set; }

        [BlubMember(17)]
        public int Unk18 { get; set; }

        [BlubMember(18)]
        public int Unk19 { get; set; }

        [BlubMember(19)]
        public int Unk20 { get; set; }

        [BlubMember(20)]
        public int Unk21 { get; set; }

        [BlubMember(21)]
        public int Unk22 { get; set; }

        [BlubMember(22)]
        public DMStatsDto DMStats { get; set; }

        [BlubMember(23)]
        public TDStatsDto TDStats { get; set; }

        [BlubMember(24)]
        public ChaserStatsDto ChaserStats { get; set; }

        [BlubMember(25)]
        public BRStatsDto BRStats { get; set; }

        [BlubMember(26)]
        public CPTStatsDto CPTStats { get; set; }

        [BlubMember(27)]
        public SiegeStatsDto SiegeStats { get; set; }
    }

    [BlubContract]
    public class ItemBuyItemAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Ids { get; set; }

        [BlubMember(1)]
        public ItemBuyResult Result { get; set; }

        [BlubMember(2)]
        public ShopItemDto Item { get; set; }

        public ItemBuyItemAckMessage()
        {
            Ids = Array.Empty<ulong>();
            Item = new ShopItemDto();
        }

        public ItemBuyItemAckMessage(ItemBuyResult result)
            : this()
        {
            Result = result;
        }

        public ItemBuyItemAckMessage(ulong[] ids, ShopItemDto item)
        {
            Ids = ids;
            Result = ItemBuyResult.OK;
            Item = item;
        }
    }

    [BlubContract]
    public class ItemRepairItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ItemRepairResult Result { get; set; }

        [BlubMember(1)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class ItemDurabilityItemAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDurabilityInfoDto[] Items { get; set; }

        public ItemDurabilityItemAckMessage()
        {
            Items = Array.Empty<ItemDurabilityInfoDto>();
        }

        public ItemDurabilityItemAckMessage(ItemDurabilityInfoDto[] items)
        {
            Items = items;
        }

    }

    [BlubContract]
    public class ItemRefundItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        [BlubMember(1)]
        public ItemRefundResult Result { get; set; }
    }

    [BlubContract]
    public class MoneyRefreshCashInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint PEN { get; set; }

        [BlubMember(1)]
        public uint AP { get; set; }

        public MoneyRefreshCashInfoAckMessage()
        { }

        public MoneyRefreshCashInfoAckMessage(uint pen, uint ap)
        {
            PEN = pen;
            AP = ap;
        }
    }

    [BlubContract]
    public class AdminActionAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }

        public AdminActionAckMessage()
        {
            Message = "";
        }
    }

    [BlubContract]
    public class AdminShowWindowAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public bool DisableConsole { get; set; }

        public AdminShowWindowAckMessage()
        { }

        public AdminShowWindowAckMessage(bool disableConsole)
        {
            DisableConsole = disableConsole;
        }
    }

    [BlubContract]
    public class NoticeAdminMessageAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Message { get; set; }

        public NoticeAdminMessageAckMessage()
        {
            Message = "";
        }
        public NoticeAdminMessageAckMessage(string message)
        {
            Message = message;
        }
    }

    [BlubContract]
    public class CharacterCurrentSlotInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte CharacterCount { get; set; }

        [BlubMember(1)]
        public byte MaxSlots { get; set; }

        [BlubMember(2)]
        public byte ActiveCharacter { get; set; }
    }

    [BlubContract]
    public class ItemRefreshInvalidEquipItemAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }

        public ItemRefreshInvalidEquipItemAckMessage()
        {
            Items = Array.Empty<ulong>();
        }
    }

    [BlubContract]
    public class ItemClearInvalidEquipItemAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public InvalidateItemInfoDto[] Items { get; set; }

        public ItemClearInvalidEquipItemAckMessage()
        {
            Items = Array.Empty<InvalidateItemInfoDto>();
        }
    }

    [BlubContract]
    public class CharacterAvatarEquipPresetAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class LicenseMyInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Licenses { get; set; }

        public LicenseMyInfoAckMessage()
        {
            Licenses = Array.Empty<uint>();
        }

        public LicenseMyInfoAckMessage(uint[] licenses)
        {
            Licenses = licenses;
        }
    }

    [BlubContract]
    public class ClubInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public PlayerClubInfoDto ClubInfo { get; set; }

        public ClubInfoAckMessage()
        {
            ClubInfo = new PlayerClubInfoDto();
        }

        public ClubInfoAckMessage(PlayerClubInfoDto clubInfo)
        {
            ClubInfo = clubInfo;
        }
    }

    [BlubContract]
    public class ClubHistoryAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ClubHistoryDto History { get; set; }

        public ClubHistoryAckMessage()
        {
            History = new ClubHistoryDto();
        }
    }

    [BlubContract]
    public class ItemEquipBoostItemInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }

        public ItemEquipBoostItemInfoAckMessage()
        {
            Items = Array.Empty<ulong>();
        }
    }

    [BlubContract]
    public class ClubFindInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ClubInfoDto ClubInfo { get; set; }

        public ClubFindInfoAckMessage()
        {
            ClubInfo = new ClubInfoDto();
        }
    }

    [BlubContract]
    public class TaskInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public TaskDto[] Tasks { get; set; }

        public TaskInfoAckMessage()
        {
            Tasks = Array.Empty<TaskDto>();
        }
    }

    [BlubContract]
    public class TaskUpdateAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }

        [BlubMember(1)]
        public ushort Progress { get; set; }
    }

    [BlubContract]
    public class TaskRequestAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }

        [BlubMember(1)]
        public MissionRewardType RewardType { get; set; }

        [BlubMember(2)]
        public uint Reward { get; set; }

        [BlubMember(3)]
        public byte Slot { get; set; }
    }

    [BlubContract]
    public class TaskRemoveAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint TaskId { get; set; }
    }

    [BlubContract]
    public class MoenyRefreshCoinInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint ArcadeCoins { get; set; }

        [BlubMember(1)]
        public uint BuffCoins { get; set; }
    }

    [BlubContract]
    public class ItemUseEsperChipItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class RequitalArcadeRewardAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ArcadeRewardDto Reward { get; set; }
    }

    [BlubContract]
    public class PlayeArcadeMapInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeMapInfoDto[] Infos { get; set; }

        public PlayeArcadeMapInfoAckMessage()
        {
            Infos = Array.Empty<ArcadeMapInfoDto>();
        }
    }

    [BlubContract]
    public class PlayerArcadeStageInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeStageInfoDto[] Infos { get; set; }

        public PlayerArcadeStageInfoAckMessage()
        {
            Infos = Array.Empty<ArcadeStageInfoDto>();
        }
    }

    [BlubContract]
    public class MoneyRefreshPenInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class ItemUseCapsuleAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public CapsuleRewardDto[] Rewards { get; set; }

        [BlubMember(1)]
        public byte Result { get; set; }

        public ItemUseCapsuleAckMessage()
        {
            Rewards = Array.Empty<CapsuleRewardDto>();
        }

        public ItemUseCapsuleAckMessage(byte result)
            : this()
        {
            Result = result;
        }

        public ItemUseCapsuleAckMessage(CapsuleRewardDto[] rewards, byte result)
        {
            Rewards = rewards;
            Result = result;
        }
    }

    [BlubContract]
    public class AdminHGWKickAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Message { get; set; }

        public AdminHGWKickAckMessage()
        {
            Message = "";
        }
    }

    [BlubContract]
    public class ClubJoinAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Message { get; set; }

        public ClubJoinAckMessage()
        {
            Message = "";
        }
    }

    [BlubContract]
    public class ClubUnJoinAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class NewShopUpdateCheckAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Date01 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Date02 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Date03 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Date04 { get; set; }

        public NewShopUpdateCheckAckMessage()
        {
            Date01 = "";
            Date02 = "";
            Date03 = "";
            Date04 = "";
        }
    }

    [BlubContract]
    public class NewShopUpdataInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ShopResourceType Type { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        [BlubMember(2)]
        public uint Unk1 { get; set; } // size of Data?

        [BlubMember(3)]
        public uint Unk2 { get; set; } // checksum?

        [BlubMember(4, typeof(StringSerializer))]
        public string Date { get; set; }

        public NewShopUpdataInfoAckMessage()
        {
            Data = Array.Empty<byte>();
            Date = "";
        }
    }

    [BlubContract]
    public class ItemUseChangeNickAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        public ItemUseChangeNickAckMessage()
        {
            Unk3 = "";
        }
    }

    [BlubContract]
    public class ItemUseRecordResetAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class ItemUseCoinFillingAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }
    }

    [BlubContract]
    public class PlayerFindInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [BlubMember(4)]
        public int Unk4 { get; set; }

        [BlubMember(5)]
        public int Unk5 { get; set; }

        [BlubMember(6)]
        public int Unk6 { get; set; }
    }

    [BlubContract]
    public class ItemDiscardItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }

        [BlubMember(1)]
        public ulong ItemId { get; set; }
    }

    [BlubContract]
    public class ItemInventroyDeleteAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public ulong ItemId { get; set; }

        public ItemInventroyDeleteAckMessage()
        { }

        public ItemInventroyDeleteAckMessage(ulong itemId)
        {
            ItemId = itemId;
        }
    }

    [BlubContract]
    public class ClubAddressAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Fingerprint { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        public ClubAddressAckMessage()
        {
            Fingerprint = "";
        }

        public ClubAddressAckMessage(string fingerprint, uint unk2)
        {
            Fingerprint = fingerprint;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class ItemUseChangeNickCancelAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk { get; set; }
    }

    [BlubContract]
    public class RequitalEventItemRewardAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public uint Unk1 { get; set; }

        [BlubMember(1)]
        public uint Unk2 { get; set; }

        [BlubMember(2)]
        public uint Unk3 { get; set; }

        [BlubMember(3)]
        public uint Unk4 { get; set; }

        [BlubMember(4)]
        public uint Unk5 { get; set; }

        [BlubMember(5)]
        public uint Unk6 { get; set; }

        [BlubMember(6)]
        public uint Unk7 { get; set; }
    }

    [BlubContract]
    public class RoomListInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public RoomDto[] Rooms { get; set; }

        public RoomListInfoAckMessage()
        {
            Rooms = Array.Empty<RoomDto>();
        }

        public RoomListInfoAckMessage(RoomDto[] rooms)
        {
            Rooms = rooms;
        }
    }

    [BlubContract]
    public class NickDefaultAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class RequitalGiveItemResultAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public RequitalGiveItemResultDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketActionAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public ShoppingBasketDto Item { get; set; }
    }

    [BlubContract]
    public class ShoppingBasketListInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ShoppingBasketDto[] Items { get; set; }
    }

    [BlubContract]
    public class RandomShopUpdateRequestAckMessage : IGameMessage
    { }

    [BlubContract]
    public class RandomShopUpdateCheckAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk { get; set; }
    }

    [BlubContract]
    public class RandomShopUpdateInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Unk5 { get; set; }
    }

    [BlubContract]
    public class RandomShopRollingStartAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomInfoRequestAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public RoomInfoRequestDto Info { get; set; }
    }

    [BlubContract]
    public class NoteGiftItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class NoteImportuneItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class NoteGiftItemGainAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomQuickJoinAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class JorbiWebSessionRedirectAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(StringSerializer))]
        public string Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class CardGambleAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public ShopItemDto ShopItem { get; set; }
    }

    [BlubContract]
    public class NoticeItemGainAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Unk2 { get; set; }

        [BlubMember(2)]
        public ulong Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public short Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }
    }

    [BlubContract]
    public class PromotionPunkinNoticeAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionPunkinRankersAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public PromotionPunkinRankerDto[] Unk { get; set; }
    }

    [BlubContract]
    public class RequitalLevelAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public RequitalLevelDto[] Unk { get; set; }
    }

    [BlubContract]
    public class PromotionAttendanceInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public int[] Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionAttendanceGiftItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }
        
        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionCoinEventAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }
        
        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class PromotionCoinEventDropCoinAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class EnchantEnchantItemAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }
        
        [BlubMember(1)]
        public ulong Unk2 { get; set; }
        
        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class EnchantRefreshEnchantGaugeAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public EnchantGaugeDto[] Unk { get; set; }
    }

    [BlubContract]
    public class NoticeEnchantAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public NoticeEnchantDto[] Unk { get; set; }
    }

    [BlubContract]
    public class PromotionCardShuffleAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public RequitalLevelDto Unk2 { get; set; }
    }

    [BlubContract]
    public class ItemClearEsperChipAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ClearEsperChipDto[] Unk { get; set; }
    }

    [BlubContract]
    public class ChallengeMyInfoAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ChallengeMyInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class KRShutDownAckMessage : IGameMessage
    { }

    [BlubContract]
    public class RequitalChallengeAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public RequitalLevelDto[] Unk2 { get; set; }
    }

    [BlubContract]
    public class MapOpenInfosMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public MapOpenInfoDto[] Unk { get; set; }
    }

    [BlubContract]
    public class PromotionCouponEventAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class TutorialCompletedAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ExpRefreshInfoAckMessage : IGameMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class PromotionActiveAckMessage : IGameMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public PromotionActiveDto[] Unk { get; set; }
    }
}
