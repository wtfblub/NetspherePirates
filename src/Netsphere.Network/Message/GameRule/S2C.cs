using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    [BlubContract]
    public class SEnterPlayerAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; } // 0 = char does not spawn

        [BlubMember(2)]
        public PlayerGameMode PlayerGameMode { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string Nickname { get; set; }

        public SEnterPlayerAckMessage()
        {
            Nickname = "";
        }

        public SEnterPlayerAckMessage(ulong accountId, string nickname, byte unk1, PlayerGameMode mode, int unk3)
        {
            AccountId = accountId;
            Unk1 = unk1;
            PlayerGameMode = mode;
            Unk3 = unk3;
            Nickname = nickname;
        }
    }

    [BlubContract]
    public class SLeavePlayerAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public RoomLeaveReason Reason { get; set; }

        public SLeavePlayerAckMessage()
        {
            Nickname = "";
        }

        public SLeavePlayerAckMessage(ulong accountId, string nickname, RoomLeaveReason reason)
        {
            AccountId = accountId;
            Nickname = nickname;
            Reason = reason;
        }
    }

    [BlubContract]
    public class SLeavePlayerRequestAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; } // result?
    }

    [BlubContract]
    public class SChangeTeamAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public Team Team { get; set; }

        [BlubMember(2)]
        public PlayerGameMode Mode { get; set; }

        public SChangeTeamAckMessage()
        { }

        public SChangeTeamAckMessage(ulong accountId, Team team, PlayerGameMode mode)
        {
            AccountId = accountId;
            Team = team;
            Mode = mode;
        }
    }

    [BlubContract]
    public class SChangeTeamFailAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeTeamResult Result { get; set; }

        public SChangeTeamFailAckMessage()
        { }

        public SChangeTeamFailAckMessage(ChangeTeamResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class SMixChangeTeamAckMessage : GameRuleMessage
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
    public class SMixChangeTeamFailAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class SAutoAssignTeamAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SEventMessageAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public GameEventMessage Event { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public uint Unk { get; set; } // server/game time or something like that

        [BlubMember(3)]
        public ushort Value { get; set; }

        [BlubMember(4, typeof(StringSerializer))]
        public string String { get; set; }

        public SEventMessageAckMessage()
        {
            String = "";
        }

        public SEventMessageAckMessage(GameEventMessage @event, ulong accountId, uint unk, ushort value, string @string)
        {
            Event = @event;
            AccountId = accountId;
            Unk = unk;
            Value = value;
            String = @string;
        }
    }

    [BlubContract]
    public class SBriefingAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public bool IsResult { get; set; }

        [BlubMember(1)]
        public bool IsEvent { get; set; }

        [BlubMember(2, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public SBriefingAckMessage()
        {
            Data = Array.Empty<byte>();
        }

        public SBriefingAckMessage(bool isResult, bool isEvent, byte[] data)
        {
            IsResult = isResult;
            IsEvent = isEvent;
            Data = data;
        }
    }

    [BlubContract]
    public class SChangeStateAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public GameState State { get; set; }

        public SChangeStateAckMessage()
        { }

        public SChangeStateAckMessage(GameState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class SChangeSubStateAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public GameTimeState State { get; set; }

        public SChangeSubStateAckMessage()
        { }

        public SChangeSubStateAckMessage(GameTimeState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class SDestroyGameRuleAckMessage : GameRuleMessage
    { }

    [BlubContract]
    public class SChangeMasterAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeMasterAckMessage()
        { }

        public SChangeMasterAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeRefeReeAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeRefeReeAckMessage()
        { }

        public SChangeRefeReeAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeTheFirstAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeTheFirstAckMessage()
        { }

        public SChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeSlaughtererAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk { get; set; }

        public SChangeSlaughtererAckMessage()
        {
            Unk = Array.Empty<ulong>();
        }

        public SChangeSlaughtererAckMessage(ulong accountId)
        {
            AccountId = accountId;
            Unk = Array.Empty<ulong>();
        }

        public SChangeSlaughtererAckMessage(ulong accountId, ulong[] unk)
        {
            AccountId = accountId;
            Unk = unk;
        }
    }

    [BlubContract]
    public class SReadyRoundAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public bool IsReady { get; set; }

        [BlubMember(2)]
        public byte Result { get; set; }

        public SReadyRoundAckMessage()
        { }

        public SReadyRoundAckMessage(ulong accountId, bool isReady)
        {
            AccountId = accountId;
            IsReady = isReady;
        }
    }

    [BlubContract]
    public class SBeginRoundAckMessage : GameRuleMessage
    { }

    [BlubContract]
    public class SAvatarChangeAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public SAvatarChangeAckMessage()
        {
            Unk1 = new ChangeAvatarUnk1Dto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }

        public SAvatarChangeAckMessage(ChangeAvatarUnk1Dto unk1, ChangeAvatarUnk2Dto[] unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class SChangeRuleNotifyAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public SChangeRuleNotifyAckMessage()
        {
            Settings = new ChangeRuleDto();
        }

        public SChangeRuleNotifyAckMessage(ChangeRuleDto settings)
        {
            Settings = settings;
        }
    }

    [BlubContract]
    public class SChangeRuleAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public SChangeRuleAckMessage()
        {
            Settings = new ChangeRuleDto();
        }

        public SChangeRuleAckMessage(ChangeRuleDto settings)
        {
            Settings = settings;
        }
    }

    [BlubContract]
    public class SChangeRuleResultMsgAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class SMissionNotifyAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SMissionScoreAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class SScoreKillAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public SScoreKillAckMessage()
        {
            Score = new ScoreDto();
        }

        public SScoreKillAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreKillAssistAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public SScoreKillAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public SScoreKillAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreOffenseAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public SScoreOffenseAckMessage()
        {
            Score = new ScoreDto();
        }

        public SScoreOffenseAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreOffenseAssistAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public SScoreOffenseAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public SScoreOffenseAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreDefenseAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public SScoreDefenseAckMessage()
        {
            Score = new ScoreDto();
        }

        public SScoreDefenseAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreDefenseAssistAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public SScoreDefenseAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public SScoreDefenseAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreHealAssistAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        public SScoreHealAssistAckMessage()
        {
            Id = 0;
        }

        public SScoreHealAssistAckMessage(LongPeerId id)
        {
            Id = id;
        }
    }

    [BlubContract]
    public class SScoreGoalAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        public SScoreGoalAckMessage()
        {
            Id = 0;
        }

        public SScoreGoalAckMessage(LongPeerId id)
        {
            Id = id;
        }
    }

    [BlubContract]
    public class SScoreGoalAssistAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public LongPeerId Assist { get; set; }

        public SScoreGoalAssistAckMessage()
        {
            Id = 0;
            Assist = 0;
        }

        public SScoreGoalAssistAckMessage(LongPeerId id, LongPeerId assist)
        {
            Id = id;
            Assist = assist;
        }
    }

    [BlubContract]
    public class SScoreReboundAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId NewId { get; set; }

        [BlubMember(1)]
        public LongPeerId OldId { get; set; }

        public SScoreReboundAckMessage()
        {
            NewId = 0;
            OldId = 0;
        }

        public SScoreReboundAckMessage(LongPeerId newId, LongPeerId oldId)
        {
            NewId = newId;
            OldId = oldId;
        }
    }

    [BlubContract]
    public class SScoreSuicideAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1, typeof(EnumSerializer), typeof(uint))]
        public AttackAttribute Icon { get; set; }

        public SScoreSuicideAckMessage()
        {
            Id = 0;
        }

        public SScoreSuicideAckMessage(LongPeerId id, AttackAttribute icon)
        {
            Id = id;
            Icon = icon;
        }
    }

    [BlubContract]
    public class SScoreTeamKillAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }

        public SScoreTeamKillAckMessage()
        {
            Score = new Score2Dto();
        }

        public SScoreTeamKillAckMessage(Score2Dto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SScoreRoundWinAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        public SScoreRoundWinAckMessage()
        { }

        public SScoreRoundWinAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class SScoreSLRoundWinAckMessage : GameRuleMessage
    { }

    [BlubContract]
    public class SItemsChangeAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public SItemsChangeAckMessage()
        {
            Unk1 = new ChangeItemsUnkDto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }

        public SItemsChangeAckMessage(ChangeItemsUnkDto unk1, ChangeAvatarUnk2Dto[] unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class SPlayerGameModeChangeAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public PlayerGameMode Mode { get; set; }

        public SPlayerGameModeChangeAckMessage()
        { }

        public SPlayerGameModeChangeAckMessage(ulong accountId, PlayerGameMode mode)
        {
            AccountId = accountId;
            Mode = mode;
        }
    }

    [BlubContract]
    public class SRefreshGameRuleInfoAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class SArcadeScoreSyncAckMessage : GameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncDto[] Scores { get; set; }

        public SArcadeScoreSyncAckMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncDto>();
        }
    }

    [BlubContract]
    public class SArcadeBeginRoundAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SArcadeStageBriefingAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; } // ToDo

        public SArcadeStageBriefingAckMessage()
        {
            Data = Array.Empty<byte>();
        }
    }

    [BlubContract]
    public class SArcadeEnablePlayeTimeAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class SArcadeStageInfoAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class SArcadeRespawnAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SArcadeDeathPlayerInfoAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Players { get; set; }

        public SArcadeDeathPlayerInfoAckMessage()
        {
            Players = Array.Empty<ulong>();
        }
    }

    [BlubContract]
    public class SArcadeStageReadyAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class SArcadeRespawnFailAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }
    }

    [BlubContract]
    public class SChangeHPAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }
    }

    [BlubContract]
    public class SChangeMPAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }
    }

    [BlubContract]
    public class SArcadeChangeStageAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Stage { get; set; }
    }

    [BlubContract]
    public class SArcadeStageSelectAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SArcadeSaveDataInfAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class SSlaughterAttackPointAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk1 { get; set; }

        [BlubMember(2)]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class SSlaughterHealPointAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class SChangeBonusTargetAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeBonusTargetAckMessage()
        { }

        public SChangeBonusTargetAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SArcadeLoadingSucceedAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class SArcadeAllLoadingSucceedAckMessage : GameRuleMessage
    { }

    [BlubContract]
    public class SUseCoinAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }
    }

    [BlubContract]
    public class SLuckyShotAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class SGameRuleChangeTheFirstAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SGameRuleChangeTheFirstAckMessage()
        { }

        public SGameRuleChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SDevLogStartAckMessage : GameRuleMessage
    { }

    [BlubContract]
    public class SCompulsionLeaveRequestAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SCompulsionLeaveResultAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }

        [BlubMember(2)]
        public ulong Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public int Unk5 { get; set; }

        [BlubMember(5)]
        public int Unk6 { get; set; }
    }

    [BlubContract]
    public class SCompulsionLeaveActionAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SCaptainLifeRoundSetUpAckMessage : GameRuleMessage
    {
        [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
        public CaptainLifeDto[] Players { get; set; }

        public SCaptainLifeRoundSetUpAckMessage()
        {
            Players = Array.Empty<CaptainLifeDto>();
        }
    }

    [BlubContract]
    public class SCaptainSubRoundEndReasonAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SCurrentRoundInformationAckMessage : GameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
