using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    public class CEnterPlayerReqMessage : GameRuleMessage
    { }

    public class CLeavePlayerRequestReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public RoomLeaveReason Reason { get; set; }
    }

    public class CChangeTeamReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public Team Team { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public PlayerGameMode Mode { get; set; }
    }

    public class CAutoAssingTeamReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CAutoMixingTeamReqMessage : GameRuleMessage
    { }

    public class CMixChangeTeamReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2)]
        public byte Unk3 { get; set; }

        [Serialize(3)]
        public byte Unk4 { get; set; }
    }

    public class CEventMessageReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public GameEventMessage Event { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(2)]
        public uint Unk1 { get; set; } // server/game time or something like that

        [Serialize(3)]
        public ushort Value { get; set; }

        [Serialize(4)]
        public uint Unk2 { get; set; }
    }

    public class CReadyRoundReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public bool IsReady { get; set; }
    }

    public class CBeginRoundReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public bool IsReady { get; set; }
    }

    public class CAvatarDurabilityDecreaseReqMessage : GameRuleMessage
    { }

    public class CAvatarChangeReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public CAvatarChangeReqMessage()
        {
            Unk1 = new ChangeAvatarUnk1Dto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }
    }

    public class CChangeRuleNotifyReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ChangeRuleDto Settings { get; set; }

        public CChangeRuleNotifyReqMessage()
        {
            Settings = new ChangeRuleDto();
        }
    }

    public class CMissionScoreReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class CScoreKillReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ScoreDto Score { get; set; }
    }

    public class CScoreKillAssistReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    public class CScoreOffenseReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public Score2Dto Score { get; set; }
    }

    public class CScoreOffenseAssistReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    public class CScoreDefenseReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public Score2Dto Score { get; set; }
    }

    public class CScoreDefenseAssistReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    public class CScoreHealAssistReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }
    }

    public class CScoreGoalReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId PeerId { get; set; }
    }

    public class CScoreReboundReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId NewId { get; set; }

        [Serialize(1)]
        public LongPeerId OldId { get; set; }
    }

    public class CScoreSuicideReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }

        [Serialize(1)]
        public uint Icon { get; set; }
    }

    public class CScoreTeamKillReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public Score2Dto Score { get; set; }
    }

    public class CItemsChangeReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public CItemsChangeReqMessage()
        {
            Unk1 = new ChangeItemsUnkDto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }
    }

    public class CPlayerGameModeChangeReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public PlayerGameMode Mode { get; set; }
    }

    public class CArcadeAttackPointReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class CArcadeScoreSyncReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeScoreSyncReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    public class CArcadeBeginRoundReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class CArcadeStageClearReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageClearReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    public class CArcadeStageFailedReqMessage : GameRuleMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageFailedReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    public class CArcadeStageInfoReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class CArcadeEnablePlayTimeReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class CArcadeRespawnReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class CArcadeStageReadyReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class CArcadeStageSelectReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class CSlaughterAttackPointReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public float Unk1 { get; set; }

        [Serialize(2)]
        public float Unk2 { get; set; }
    }

    public class CSlaughterHealPointReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public float Unk { get; set; }
    }

    public class CArcadeLoadingSucceesReqMessage : GameRuleMessage
    { }

    public class CUseCoinReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class CBeginResponeReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong Unk { get; set; }
    }

    public class CWeaponFireReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public float Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }

        [Serialize(3)]
        public ulong Unk4 { get; set; }
    }

    public class CCompulsionLeaveRequestReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }
    }

    public class CCompulsionLeaveVoteReqMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }
}
