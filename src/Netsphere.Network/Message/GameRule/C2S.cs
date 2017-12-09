using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    [BlubContract]
    public class CEnterPlayerReqMessage : IGameRuleMessage
    { }

    [BlubContract]
    public class CLeavePlayerRequestReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public RoomLeaveReason Reason { get; set; }
    }

    [BlubContract]
    public class CChangeTeamReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public Team Team { get; set; }

        [BlubMember(1)]
        public PlayerGameMode Mode { get; set; }
    }

    [BlubContract]
    public class CAutoAssingTeamReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CAutoMixingTeamReqMessage : IGameRuleMessage
    { }

    [BlubContract]
    public class CMixChangeTeamReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong PlayerToMove { get; set; }

        [BlubMember(1)]
        public ulong PlayerToReplace { get; set; }

        [BlubMember(2)]
        public Team FromTeam { get; set; }

        [BlubMember(3)]
        public Team ToTeam { get; set; }
    }

    [BlubContract]
    public class CEventMessageReqMessage : IGameRuleMessage
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
    public class CReadyRoundReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public bool IsReady { get; set; }
    }

    [BlubContract]
    public class CBeginRoundReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public bool IsReady { get; set; }
    }

    [BlubContract]
    public class CAvatarDurabilityDecreaseReqMessage : IGameRuleMessage
    { }

    [BlubContract]
    public class CAvatarChangeReqMessage : IGameRuleMessage
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
    public class CChangeRuleNotifyReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public CChangeRuleNotifyReqMessage()
        {
            Settings = new ChangeRuleDto();
        }
    }

    [BlubContract]
    public class CMissionScoreReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CScoreKillReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }
    }

    [BlubContract]
    public class CScoreKillAssistReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreOffenseReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreOffenseAssistReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreDefenseReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreDefenseAssistReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssist2Dto Score { get; set; }
    }

    [BlubContract]
    public class CScoreHealAssistReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }
    }

    [BlubContract]
    public class CScoreGoalReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId PeerId { get; set; }
    }

    [BlubContract]
    public class CScoreReboundReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId NewId { get; set; }

        [BlubMember(1)]
        public LongPeerId OldId { get; set; }
    }

    [BlubContract]
    public class CScoreSuicideReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public uint Icon { get; set; }
    }

    [BlubContract]
    public class CScoreTeamKillReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }
    }

    [BlubContract]
    public class CItemsChangeReqMessage : IGameRuleMessage
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
    public class CPlayerGameModeChangeReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public PlayerGameMode Mode { get; set; }
    }

    [BlubContract]
    public class CArcadeAttackPointReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeScoreSyncReqMessage : IGameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeScoreSyncReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeBeginRoundReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageClearReqMessage : IGameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageClearReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeStageFailedReqMessage : IGameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncReqDto[] Scores { get; set; }

        public CArcadeStageFailedReqMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncReqDto>();
        }
    }

    [BlubContract]
    public class CArcadeStageInfoReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeEnablePlayTimeReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeRespawnReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageReadyReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class CArcadeStageSelectReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class CSlaughterAttackPointReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk1 { get; set; }

        [BlubMember(2)]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class CSlaughterHealPointReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class CArcadeLoadingSucceesReqMessage : IGameRuleMessage
    { }

    [BlubContract]
    public class CUseCoinReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class CBeginResponeReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk { get; set; }
    }

    [BlubContract]
    public class CWeaponFireReqMessage : IGameRuleMessage
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
    public class CCompulsionLeaveRequestReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class CCompulsionLeaveVoteReqMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }
}
