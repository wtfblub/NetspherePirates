using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    [BlubContract]
    public class CEnterPlayerReqMessage : GameRuleMessage
    { }

    [BlubContract]
    public class CLeavePlayerRequestReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public RoomLeaveReason Reason { get; set; }
    }

    [BlubContract]
    public class CChangeTeamReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public Team Team { get; set; }

        [BlubMember(1)]
        public PlayerGameMode Mode { get; set; }
    }

    [BlubContract]
    public class CAutoAssingTeamReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CAutoMixingTeamReqMessage : GameRuleMessage
    { }

    [BlubContract]
    public class CMixChangeTeamReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        [BlubMember(3)]
        public byte Unk4 { get; set; }
    }

    [BlubContract]
    public class CEventMessageReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public GameEventMessage Event { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public uint Unk1 { get; set; } // server/game time or something like that

        [BlubMember(3)]
        public ushort Value { get; set; }

        [BlubMember(4)]
        public uint Unk2 { get; set; }
    }

    [BlubContract]
    public class CReadyRoundReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public bool IsReady { get; set; }
    }

    [BlubContract]
    public class CBeginRoundReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public bool IsReady { get; set; }
    }

    [BlubContract]
    public class CAvatarDurabilityDecreaseReqMessage : GameRuleMessage
    { }

    [BlubContract]
    public class CAvatarChangeReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public CAvatarChangeReqMessage()
        {
            Unk1 = new ChangeAvatarUnk1Dto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }
    }

    [BlubContract]
    public class CChangeRuleNotifyReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public CChangeRuleNotifyReqMessage()
        {
            Settings = new ChangeRuleDto();
        }
    }

    [BlubContract]
    public class CMissionScoreReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CScoreKillReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }
    }

    [BlubContract]
    public class CScoreKillAssistReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreOffenseReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreOffenseAssistReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreDefenseReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreDefenseAssistReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreHealAssistReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }
    }

    [BlubContract]
    public class CScoreGoalReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId PeerId { get; set; }
    }

    [BlubContract]
    public class CScoreReboundReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId NewId { get; set; }

        [BlubMember(1)]
        public LongPeerId OldId { get; set; }
    }

    [BlubContract]
    public class CScoreSuicideReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public uint Icon { get; set; }
    }

    [BlubContract]
    public class CScoreTeamKillReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CItemsChangeReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public CItemsChangeReqMessage()
        {
            Unk1 = new ChangeItemsUnkDto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }
    }

    [BlubContract]
    public class CPlayerGameModeChangeReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public PlayerGameMode Mode { get; set; }
    }

    [BlubContract]
    public class CArcadeAttackPointReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeScoreSyncReqMessage : GameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeScoreSyncReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeBeginRoundReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageClearReqMessage : GameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageClearReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeStageFailedReqMessage : GameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageFailedReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeStageInfoReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeEnablePlayTimeReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeRespawnReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageReadyReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageSelectReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class CSlaughterAttackPointReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk1 { get; set; }

        [BlubMember(2)]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class CSlaughterHealPointReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeLoadingSucceesReqMessage : GameRuleMessage
    { }

    [BlubContract]
    public class CUseCoinReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CBeginResponeReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class CWeaponFireReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public float Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public ulong Unk4 { get; set; }
    }

    [BlubContract]
    public class CCompulsionLeaveRequestReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class CCompulsionLeaveVoteReqMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }
}
