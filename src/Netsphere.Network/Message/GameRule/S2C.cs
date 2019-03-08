using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    [BlubContract]
    public class SEnterPlayerAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public byte Unk1 { get; set; } // 0 = char does not spawn

        [BlubMember(2)]
        public PlayerGameMode PlayerGameMode { get; set; }

        [BlubMember(3)]
        public int Unk3 { get; set; }

        [BlubMember(4)]
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
    public class SLeavePlayerAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
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
    public class SLeavePlayerRequestAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; } // result?
    }

    [BlubContract]
    public class SChangeTeamAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public TeamId Team { get; set; }

        [BlubMember(2)]
        public PlayerGameMode Mode { get; set; }

        public SChangeTeamAckMessage()
        {
        }

        public SChangeTeamAckMessage(ulong accountId, TeamId team, PlayerGameMode mode)
        {
            AccountId = accountId;
            Team = team;
            Mode = mode;
        }
    }

    [BlubContract]
    public class SChangeTeamFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeTeamResult Result { get; set; }

        public SChangeTeamFailAckMessage()
        {
        }

        public SChangeTeamFailAckMessage(ChangeTeamResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class SMixChangeTeamAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong PlayerToMove { get; set; }

        [BlubMember(1)]
        public ulong PlayerToReplace { get; set; }

        [BlubMember(2)]
        public TeamId ToTeam { get; set; }

        [BlubMember(3)]
        public TeamId FromTeam { get; set; }

        public SMixChangeTeamAckMessage()
        {
        }

        public SMixChangeTeamAckMessage(ulong playerToMove, ulong playerToReplace, TeamId fromTeam, TeamId toTeam)
        {
            PlayerToMove = playerToMove;
            PlayerToReplace = playerToReplace;
            FromTeam = fromTeam;
            ToTeam = toTeam;
        }
    }

    [BlubContract]
    public class SMixChangeTeamFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class SAutoAssignTeamAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SEventMessageAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public GameEventMessage Event { get; set; }

        [BlubMember(1)]
        public ulong AccountId { get; set; }

        [BlubMember(2)]
        public uint Unk { get; set; } // server/game time or something like that

        [BlubMember(3)]
        public ushort Value { get; set; }

        [BlubMember(4)]
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
    public class SBriefingAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public bool IsResult { get; set; }

        [BlubMember(1)]
        public bool IsEvent { get; set; }

        [BlubMember(2)]
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
    public class SChangeStateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public GameState State { get; set; }

        public SChangeStateAckMessage()
        {
        }

        public SChangeStateAckMessage(GameState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class SChangeSubStateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public GameTimeState State { get; set; }

        public SChangeSubStateAckMessage()
        {
        }

        public SChangeSubStateAckMessage(GameTimeState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class SDestroyGameRuleAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SChangeMasterAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeMasterAckMessage()
        {
        }

        public SChangeMasterAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeRefeReeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeRefeReeAckMessage()
        {
        }

        public SChangeRefeReeAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeTheFirstAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeTheFirstAckMessage()
        {
        }

        public SChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SChangeSlaughtererAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
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
    public class SReadyRoundAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public bool IsReady { get; set; }

        [BlubMember(2)]
        public byte Result { get; set; }

        public SReadyRoundAckMessage()
        {
        }

        public SReadyRoundAckMessage(ulong accountId, bool isReady)
        {
            AccountId = accountId;
            IsReady = isReady;
        }
    }

    [BlubContract]
    public class SBeginRoundAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SAvatarChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
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
    public class SChangeRuleNotifyAckMessage : IGameRuleMessage
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
    public class SChangeRuleAckMessage : IGameRuleMessage
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
    public class SChangeRuleResultMsgAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class SMissionNotifyAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SMissionScoreAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong PlayerId { get; set; }

        [BlubMember(1)]
        public int ScoreGained { get; set; }

        public SMissionScoreAckMessage()
        {
        }

        public SMissionScoreAckMessage(ulong playerId, int scoreGained)
        {
            PlayerId = playerId;
            ScoreGained = scoreGained;
        }
    }

    [BlubContract]
    public class SScoreKillAckMessage : IGameRuleMessage
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
    public class SScoreKillAssistAckMessage : IGameRuleMessage
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
    public class SScoreOffenseAckMessage : IGameRuleMessage
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
    public class SScoreOffenseAssistAckMessage : IGameRuleMessage
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
    public class SScoreDefenseAckMessage : IGameRuleMessage
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
    public class SScoreDefenseAssistAckMessage : IGameRuleMessage
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
    public class SScoreHealAssistAckMessage : IGameRuleMessage
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
    public class SScoreGoalAckMessage : IGameRuleMessage
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
    public class SScoreGoalAssistAckMessage : IGameRuleMessage
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
    public class SScoreReboundAckMessage : IGameRuleMessage
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
    public class SScoreSuicideAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(EnumSerializer), typeof(int))]
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
    public class SScoreTeamKillAckMessage : IGameRuleMessage
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
    public class SScoreRoundWinAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        public SScoreRoundWinAckMessage()
        {
        }

        public SScoreRoundWinAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class SScoreSLRoundWinAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SItemsChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
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
    public class SPlayerGameModeChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public PlayerGameMode Mode { get; set; }

        public SPlayerGameModeChangeAckMessage()
        {
        }

        public SPlayerGameModeChangeAckMessage(ulong accountId, PlayerGameMode mode)
        {
            AccountId = accountId;
            Mode = mode;
        }
    }

    [BlubContract]
    public class SRefreshGameRuleInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class SArcadeScoreSyncAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncDto[] Scores { get; set; }

        public SArcadeScoreSyncAckMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncDto>();
        }
    }

    [BlubContract]
    public class SArcadeBeginRoundAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        public SArcadeBeginRoundAckMessage()
        {
        }

        public SArcadeBeginRoundAckMessage(byte unk1, byte unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class SArcadeStageBriefingAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte[] Data { get; set; } // ToDo

        public SArcadeStageBriefingAckMessage()
        {
            Data = Array.Empty<byte>();
        }
    }

    [BlubContract]
    public class SArcadeEnablePlayeTimeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        public SArcadeEnablePlayeTimeAckMessage()
        {
        }

        public SArcadeEnablePlayeTimeAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class SArcadeStageInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        public SArcadeStageInfoAckMessage()
        {
        }

        public SArcadeStageInfoAckMessage(byte unk1, int unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class SArcadeRespawnAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SArcadeDeathPlayerInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Players { get; set; }

        public SArcadeDeathPlayerInfoAckMessage()
        {
            Players = Array.Empty<ulong>();
        }
    }

    [BlubContract]
    public class SArcadeStageReadyAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class SArcadeRespawnFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }
    }

    [BlubContract]
    public class SChangeHPAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }

        public SChangeHPAckMessage()
        {
        }

        public SChangeHPAckMessage(float value)
        {
            Value = value;
        }
    }

    [BlubContract]
    public class SChangeMPAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }

        public SChangeMPAckMessage()
        {
        }

        public SChangeMPAckMessage(float value)
        {
            Value = value;
        }
    }

    [BlubContract]
    public class SArcadeChangeStageAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Stage { get; set; }
    }

    [BlubContract]
    public class SArcadeStageSelectAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SArcadeSaveDataInfAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class SSlaughterAttackPointAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk1 { get; set; }

        [BlubMember(2)]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class SSlaughterHealPointAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class SChangeBonusTargetAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SChangeBonusTargetAckMessage()
        {
        }

        public SChangeBonusTargetAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SArcadeLoadingSucceedAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class SArcadeAllLoadingSucceedAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SUseCoinAckMessage : IGameRuleMessage
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
    public class SLuckyShotAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class SGameRuleChangeTheFirstAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SGameRuleChangeTheFirstAckMessage()
        {
        }

        public SGameRuleChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SDevLogStartAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SCompulsionLeaveRequestAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SCompulsionLeaveResultAckMessage : IGameRuleMessage
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
    public class SCompulsionLeaveActionAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class SCaptainLifeRoundSetUpAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public CaptainLifeDto[] Players { get; set; }

        public SCaptainLifeRoundSetUpAckMessage()
        {
            Players = Array.Empty<CaptainLifeDto>();
        }
    }

    [BlubContract]
    public class SCaptainSubRoundEndReasonAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class SCurrentRoundInformationAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }
}
