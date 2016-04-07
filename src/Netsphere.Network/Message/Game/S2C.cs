using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Game
{
    public class SLoginAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public GameLoginResult Result { get; set; }

        [Serialize(2)]
        public ulong Unk { get; set; }
    }

    public class SBeginAccountInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; } // IsGM?

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public byte Level { get; set; }

        [Serialize(3)]
        public byte Unk3 { get; set; }

        [Serialize(4)]
        public uint TotalExp { get; set; }

        [Serialize(5)]
        public uint AP { get; set; }

        [Serialize(6)]
        public uint PEN { get; set; }

        [Serialize(7)]
        public uint TutorialState { get; set; }

        [Serialize(8, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(9)]
        public uint Unk4 { get; set; } // something with licenses needed to enter s4league

        [Serialize(10)]
        public DMStatsDto DMStats { get; set; }

        [Serialize(11)]
        public TDStatsDto TDStats { get; set; }

        [Serialize(12)]
        public ChaserStatsDto ChaserStats { get; set; }

        [Serialize(13)]
        public BRStatsDto BRStats { get; set; }

        [Serialize(14)]
        public CPTStatsDto CPTStats { get; set; }

        public SBeginAccountInfoAckMessage()
        {
            DMStats = new DMStatsDto();
            TDStats = new TDStatsDto();
            ChaserStats = new ChaserStatsDto();
            BRStats = new BRStatsDto();
            CPTStats = new CPTStatsDto();
        }
    }

    public class SOpenCharacterInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }

        [Serialize(1)]
        public byte Unk1 { get; set; }

        [Serialize(2)]
        public byte Unk2 { get; set; }

        [Serialize(3)]
        public CharacterStyle Style { get; set; }

        public SOpenCharacterInfoAckMessage()
        {
            Unk1 = 1; // max skill?
            Unk2 = 3; // max weapons?
        }
    }

    public class SCharacterEquipInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Unk1 { get; set; }

        [Serialize(2, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Unk2 { get; set; }

        [Serialize(3, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Weapons { get; set; }

        [Serialize(4, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Skills { get; set; }

        [Serialize(5, typeof(ArrayWithIntPrefixAndIndexSerializer))]
        public ulong[] Clothes { get; set; }

        public SCharacterEquipInfoAckMessage()
        {
            Unk1 = new ulong[3];
            Unk2 = Unk1;
            Weapons = Unk1;
            Skills = new ulong[1];
            Clothes = new ulong[7];
        }
    }

    public class SInventoryInfoAckMessage : GameMessage
    {
        public ItemDto[] Items { get; set; }

        public SInventoryInfoAckMessage()
        {
            Items = Array.Empty<ItemDto>();
        }
    }

    public class SSuccessDeleteCharacterAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }
    }

    public class SSuccessSelectCharacterAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }
    }

    public class SSuccessCreateCharacterAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Slot { get; set; }

        [Serialize(1)]
        public CharacterStyle Style { get; set; }

        [Serialize(2)]
        public byte Unk1 { get; set; }

        [Serialize(3)]
        public byte Unk2 { get; set; }
    }

    public class SServerResultInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ServerResult Result { get; set; }

        public SServerResultInfoAckMessage()
        { }

        public SServerResultInfoAckMessage(ServerResult result)
        {
            Result = result;
        }
    }

    public class SCreateNickAckMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Nickname { get; set; }
    }

    public class SCheckNickAckMessage : GameMessage
    {
        [Serialize(0)]
        public bool IsAvailable { get; set; }

        public SCheckNickAckMessage()
        { }

        public SCheckNickAckMessage(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }

    public class SUseItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte CharacterSlot { get; set; }

        [Serialize(1)]
        public byte EquipSlot { get; set; }

        [Serialize(2)]
        public ulong ItemId { get; set; }

        [Serialize(3, typeof(EnumSerializer))]
        public UseItemAction Action { get; set; }
    }

    public class SInventoryActionAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public InventoryAction Action { get; set; }

        [Serialize(0)]
        public ItemDto Item { get; set; }

        public SInventoryActionAckMessage()
        {
            Item = new ItemDto();
        }
    }

    public class SIdsInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }

        [Serialize(1)]
        public byte Slot { get; set; }

        public SIdsInfoAckMessage()
        { }

        public SIdsInfoAckMessage(uint unk, byte slot)
        {
            Unk = unk;
            Slot = slot;
        }
    }

    public class SEnteredPlayerAckMessage : GameMessage
    {
        [Serialize(0)]
        public RoomPlayerDto Player { get; set; }

        public SEnteredPlayerAckMessage()
        {
            Player = new RoomPlayerDto();
        }

        public SEnteredPlayerAckMessage(RoomPlayerDto plr)
        {
            Player = plr;
        }
    }

    public class SEnteredPlayerClubInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public PlayerClubInfoDto Player { get; set; }

        public SEnteredPlayerClubInfoAckMessage()
        {
            Player = new PlayerClubInfoDto();
        }
    }

    public class SEnteredPlayerListAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public RoomPlayerDto[] Players { get; set; }

        public SEnteredPlayerListAckMessage()
        {
            Players = Array.Empty<RoomPlayerDto>();
        }

        public SEnteredPlayerListAckMessage(RoomPlayerDto[] players)
        {
            Players = players;
        }
    }

    public class SEnteredPlayerClubInfoListAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public PlayerClubInfoDto[] Players { get; set; }

        public SEnteredPlayerClubInfoListAckMessage()
        {
            Players = Array.Empty<PlayerClubInfoDto>();
        }
    }

    public class SSuccessEnterRoomAckMessage : GameMessage
    {
        [Serialize(0)]
        public EnterRoomInfoDto RoomInfo { get; set; }

        public SSuccessEnterRoomAckMessage()
        {
            RoomInfo = new EnterRoomInfoDto();
        }

        public SSuccessEnterRoomAckMessage(EnterRoomInfoDto roomInfo)
        {
            RoomInfo = roomInfo;
        }
    }

    public class SLeavePlayerAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SLeavePlayerAckMessage()
        { }

        public SLeavePlayerAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [Obsolete("This handler is empty inside the client")]
    public class SJoinTunnelPlayerAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class STimeSyncAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint ClientTime { get; set; }

        [Serialize(1)]
        public uint ServerTime { get; set; }
    }

    public class SPlayTogetherSignAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class SPlayTogetherInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }
    }

    public class SPlayTogetherSignInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public byte Unk { get; set; }
    }

    public class SPlayTogetherCancelAckMessage : GameMessage
    { }

    public class SChangeGameRoomAckMessage : GameMessage
    {
        [Serialize(0)]
        public RoomDto Room { get; set; }

        public SChangeGameRoomAckMessage()
        {
            Room = new RoomDto();
        }

        public SChangeGameRoomAckMessage(RoomDto room)
        {
            Room = room;
        }
    }

    public class SNewShopUpdateRequestAckMessage : GameMessage
    { }

    public class SLogoutAckMessage : GameMessage
    { }

    public class SPlayTogetherKickAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class SChannelListInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ChannelInfoDto[] Channels { get; set; }

        public SChannelListInfoAckMessage()
        {
            Channels = Array.Empty<ChannelInfoDto>();
        }

        public SChannelListInfoAckMessage(ChannelInfoDto[] channels)
        {
            Channels = channels;
        }
    }

    public class SChannelDeployPlayerAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public uint Unk1 { get; set; } // room id?

        [Serialize(2, typeof(StringSerializer))]
        public string Unk2 { get; set; } // maybe nickname

        public SChannelDeployPlayerAckMessage()
        {
            Unk2 = "";
        }
    }

    public class SChannelDisposePlayerAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class SGameRoomListAckMessage : GameMessage
    {
        public ChannelInfoRequest ListType { get; set; }
        public RoomDto[] Rooms { get; set; }

        public SGameRoomListAckMessage()
        {
            Rooms = Array.Empty<RoomDto>();
        }

        public SGameRoomListAckMessage(RoomDto[] rooms)
        {
            ListType = ChannelInfoRequest.RoomList;
            Rooms = rooms;
        }
    }

    public class SDeployGameRoomAckMessage : GameMessage
    {
        [Serialize(0)]
        public RoomDto Room { get; set; }

        public SDeployGameRoomAckMessage()
        {
            Room = new RoomDto();
        }

        public SDeployGameRoomAckMessage(RoomDto room)
        {
            Room = room;
        }
    }

    public class SDisposeGameRoomAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint RoomId { get; set; }

        public SDisposeGameRoomAckMessage()
        { }

        public SDisposeGameRoomAckMessage(uint roomId)
        {
            RoomId = roomId;
        }
    }

    public class SGamePingAverageAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; } // ping?
    }

    public class SBuyItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Ids { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public ItemBuyResult Result { get; set; }

        [Serialize(2)]
        public ShopItemDto Item { get; set; }

        public SBuyItemAckMessage()
        {
            Ids = Array.Empty<ulong>();
            Item = new ShopItemDto();
        }

        public SBuyItemAckMessage(ItemBuyResult result)
            : this()
        {
            Result = result;
        }

        public SBuyItemAckMessage(ulong[] ids, ShopItemDto item)
        {
            Ids = ids;
            Result = ItemBuyResult.OK;
            Item = item;
        }
    }

    public class SRepairItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ItemRepairResult Result { get; set; }

        [Serialize(1)]
        public ulong ItemId { get; set; }
    }

    public class SItemDurabilityInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ItemDurabilityInfoDto[] Items { get; set; }

        public SItemDurabilityInfoAckMessage()
        {
            Items = Array.Empty<ItemDurabilityInfoDto>();
        }

        public SItemDurabilityInfoAckMessage(ItemDurabilityInfoDto[] items)
        {
            Items = items;
        }

    }

    public class SRefundItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ItemRefundResult Result { get; set; }

        [Serialize(1)]
        public ulong ItemId { get; set; }
    }

    public class SRefreshCashInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint PEN { get; set; }

        [Serialize(1)]
        public uint AP { get; set; }

        public SRefreshCashInfoAckMessage()
        { }

        public SRefreshCashInfoAckMessage(uint pen, uint ap)
        {
            PEN = pen;
            AP = ap;
        }
    }

    public class SAdminActionAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Result { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Message { get; set; }

        public SAdminActionAckMessage()
        {
            Message = "";
        }
    }

    public class SAdminShowWindowAckMessage : GameMessage
    {
        [Serialize(0)]
        public bool ShowConsole { get; set; }
    }

    public class SNoticeMessageAckMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Message { get; set; }

        public SNoticeMessageAckMessage()
        {
            Message = "";
        }
        public SNoticeMessageAckMessage(string message)
        {
            Message = message;
        }
    }

    public class SCharacterSlotInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte CharacterCount { get; set; }

        [Serialize(1)]
        public byte MaxSlots { get; set; }

        [Serialize(2)]
        public byte ActiveCharacter { get; set; }
    }

    public class SRefreshInvalidEquipItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }

        public SRefreshInvalidEquipItemAckMessage()
        {
            Items = Array.Empty<ulong>();
        }
    }

    public class SClearInvalidateItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public InvalidateItemInfoDto[] Items { get; set; }

        public SClearInvalidateItemAckMessage()
        {
            Items = Array.Empty<InvalidateItemInfoDto>();
        }
    }

    public class SRefreshItemTimeInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2)]
        public ulong Unk3 { get; set; }
    }

    [Obsolete("This handler is empty inside the client")]
    public class SEnableAccountStatusAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class SActiveEquipPresetAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class SMyLicenseInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public uint[] Licenses { get; set; }

        public SMyLicenseInfoAckMessage()
        {
            Licenses = Array.Empty<uint>();
        }

        public SMyLicenseInfoAckMessage(uint[] licenses)
        {
            Licenses = licenses;
        }
    }

    public class SLicensedAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ItemLicense ItemLicense { get; set; }

        [Serialize(1)]
        public ItemNumber ItemNumber { get; set; }
    }

    public class SCoinEventAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class SCombiCompensationAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }

        [Serialize(4)]
        public uint Unk5 { get; set; }
    }

    public class SClubInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public PlayerClubInfoDto ClubInfo { get; set; }

        public SClubInfoAckMessage()
        {
            ClubInfo = new PlayerClubInfoDto();
        }

        public SClubInfoAckMessage(PlayerClubInfoDto clubInfo)
        {
            ClubInfo = clubInfo;
        }
    }

    public class SClubHistoryAckMessage : GameMessage
    {
        [Serialize(0)]
        public ClubHistoryDto History { get; set; }

        public SClubHistoryAckMessage()
        {
            History = new ClubHistoryDto();
        }
    }

    public class SEquipedBoostItemAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Items { get; set; }

        public SEquipedBoostItemAckMessage()
        {
            Items = Array.Empty<ulong>();
        }
    }

    public class SGetClubInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public ClubInfoDto ClubInfo { get; set; }

        public SGetClubInfoAckMessage()
        {
            ClubInfo = new ClubInfoDto();
        }
    }

    public class STaskInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public TaskDto[] Tasks { get; set; }

        public STaskInfoAckMessage()
        {
            Tasks = Array.Empty<TaskDto>();
        }
    }

    public class STaskUpdateAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint TaskId { get; set; }

        [Serialize(1)]
        public ushort Progress { get; set; }
    }

    public class STaskRequestAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint TaskId { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public MissionRewardType RewardType { get; set; }

        [Serialize(2)]
        public uint Reward { get; set; }

        [Serialize(3)]
        public byte Slot { get; set; }
    }

    public class SExchangeItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }
    }

    public class STaskIngameUpdateAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint TaskId { get; set; }

        [Serialize(1)]
        public ushort Progress { get; set; }
    }

    public class STaskRemoveAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint TaskId { get; set; }
    }

    public class SRandomShopChanceInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Progress { get; set; }
    }

    public class SRandomShopItemInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public RandomShopItemDto Item { get; set; }

        public SRandomShopItemInfoAckMessage()
        {
            Item = new RandomShopItemDto();
        }
    }

    public class SRandomShopInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public RandomShopDto Info { get; set; }

        public SRandomShopInfoAckMessage()
        {
            Info = new RandomShopDto();
        }
    }

    public class SSetCoinAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint ArcadeCoins { get; set; }

        [Serialize(1)]
        public uint BuffCoins { get; set; }
    }

    public class SApplyEsperChipItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public EsperChipItemInfoDto Chip { get; set; }
    }

    public class SArcadeRewardInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public ArcadeRewardDto Reward { get; set; }
    }

    public class SArcadeMapScoreAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeMapScoreDto[] Scores { get; set; }

        public SArcadeMapScoreAckMessage()
        {
            Scores = Array.Empty<ArcadeMapScoreDto>();
        }
    }

    public class SArcadeStageScoreAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeStageScoreDto[] Scores { get; set; }

        public SArcadeStageScoreAckMessage()
        {
            Scores = Array.Empty<ArcadeStageScoreDto>();
        }
    }

    public class SMixedTeamBriefingInfoAckMessage : GameMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public MixedTeamBriefingDto[] Briefing { get; set; }

        public SMixedTeamBriefingInfoAckMessage()
        {
            Briefing = Array.Empty<MixedTeamBriefingDto>();
        }
    }

    public class SSetGameMoneyAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class SUseCapsuleAckMessage : GameMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public CapsuleRewardDto[] Rewards { get; set; }

        [Serialize(1)]
        public byte Result { get; set; }

        public SUseCapsuleAckMessage()
        {
            Rewards = Array.Empty<CapsuleRewardDto>();
        }

        public SUseCapsuleAckMessage(byte result)
            : this()
        {
            Result = result;
        }

        public SUseCapsuleAckMessage(CapsuleRewardDto[] rewards, byte result)
        {
            Rewards = rewards;
            Result = result;
        }
    }

    public class SHGWKickAckMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Message { get; set; }

        public SHGWKickAckMessage()
        {
            Message = "";
        }
    }

    public class SClubJoinAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Message { get; set; }

        public SClubJoinAckMessage()
        {
            Message = "";
        }
    }

    public class SClubUnJoinAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class SNewShopUpdateCheckAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Date01 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Date02 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Date03 { get; set; }

        [Serialize(4, typeof(StringSerializer))]
        public string Date04 { get; set; }

        public SNewShopUpdateCheckAckMessage()
        {
            Date01 = "";
            Date02 = "";
            Date03 = "";
            Date04 = "";
        }
    }

    public class SNewShopUpdateInfoAckMessage : GameMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ShopResourceType Type { get; set; }

        [Serialize(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        [Serialize(2)]
        public uint Unk1 { get; set; } // size of Data?

        [Serialize(3)]
        public uint Unk2 { get; set; } // checksum?

        [Serialize(4, typeof(StringSerializer))]
        public string Date { get; set; }

        public SNewShopUpdateInfoAckMessage()
        {
            Data = Array.Empty<byte>();
            Date = "";
        }
    }

    public class SUseChangeNickItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        public SUseChangeNickItemAckMessage()
        {
            Unk3 = "";
        }
    }

    public class SUseResetRecordItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }
    }

    public class SUseCoinFillingItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }
    }

    public class SDiscardItemAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }

        [Serialize(1)]
        public ulong ItemId { get; set; }
    }

    public class SDeleteItemInventoryAckMessage : GameMessage
    {
        [Serialize(0)]
        public ulong ItemId { get; set; }
    }

    public class SClubAddressAckMessage : GameMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Fingerprint { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        public SClubAddressAckMessage()
        {
            Fingerprint = "";
        }

        public SClubAddressAckMessage(string fingerprint, uint unk2)
        {
            Fingerprint = fingerprint;
            Unk2 = unk2;
        }
    }

    public class SSmallLoudSpeakerAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2, typeof(StringSerializer))]
        public string Unk3 { get; set; }

        [Serialize(3, typeof(StringSerializer))]
        public string Unk4 { get; set; }

        public SSmallLoudSpeakerAckMessage()
        {
            Unk3 = "";
            Unk4 = "";
        }
    }

    public class SIngameEquipCheckAckMessage : GameMessage
    { }

    public class SUseCoinRandomShopChanceAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class SChangeNickCancelAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk { get; set; }
    }

    public class SEventRewardAckMessage : GameMessage
    {
        [Serialize(0)]
        public uint Unk1 { get; set; }

        [Serialize(1)]
        public uint Unk2 { get; set; }

        [Serialize(2)]
        public uint Unk3 { get; set; }

        [Serialize(3)]
        public uint Unk4 { get; set; }

        [Serialize(4)]
        public uint Unk5 { get; set; }

        [Serialize(5)]
        public uint Unk6 { get; set; }
    }
}
