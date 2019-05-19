using System;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    [BlubContract]
    public class RoomEnterPlayerAckMessage : IGameRuleMessage
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

        [BlubMember(5)]
        public byte Unk4 { get; set; }

        public RoomEnterPlayerAckMessage()
        {
            Nickname = "";
        }

        public RoomEnterPlayerAckMessage(ulong accountId, string nickname, byte unk1, PlayerGameMode mode, int unk3, byte unk4)
        {
            AccountId = accountId;
            Unk1 = unk1;
            PlayerGameMode = mode;
            Unk3 = unk3;
            Nickname = nickname;
            Unk4 = unk4;
        }
    }

    [BlubContract]
    public class RoomLeavePlayerAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public RoomLeaveReason Reason { get; set; }

        public RoomLeavePlayerAckMessage()
        {
            Nickname = "";
        }

        public RoomLeavePlayerAckMessage(ulong accountId, string nickname, RoomLeaveReason reason)
        {
            AccountId = accountId;
            Nickname = nickname;
            Reason = reason;
        }
    }

    [BlubContract]
    public class RoomLeaveReqeustAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; } // result?
    }

    [BlubContract]
    public class RoomChangeTeamAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public TeamId Team { get; set; }

        [BlubMember(2)]
        public PlayerGameMode Mode { get; set; }

        public RoomChangeTeamAckMessage()
        {
        }

        public RoomChangeTeamAckMessage(ulong accountId, TeamId team, PlayerGameMode mode)
        {
            AccountId = accountId;
            Team = team;
            Mode = mode;
        }
    }

    [BlubContract]
    public class RoomChangeTeamFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeTeamResult Result { get; set; }

        public RoomChangeTeamFailAckMessage()
        {
        }

        public RoomChangeTeamFailAckMessage(ChangeTeamResult result)
        {
            Result = result;
        }
    }

    [BlubContract]
    public class RoomChoiceTeamChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong PlayerToMove { get; set; }

        [BlubMember(1)]
        public ulong PlayerToReplace { get; set; }

        [BlubMember(2)]
        public TeamId ToTeam { get; set; }

        [BlubMember(3)]
        public TeamId FromTeam { get; set; }

        public RoomChoiceTeamChangeAckMessage()
        {
        }

        public RoomChoiceTeamChangeAckMessage(ulong playerToMove, ulong playerToReplace, TeamId fromTeam, TeamId toTeam)
        {
            PlayerToMove = playerToMove;
            PlayerToReplace = playerToReplace;
            FromTeam = fromTeam;
            ToTeam = toTeam;
        }
    }

    [BlubContract]
    public class RoomChoiceTeamChangeFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class GameEventMessageAckMessage : IGameRuleMessage
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

        public GameEventMessageAckMessage()
        {
            String = "";
        }

        public GameEventMessageAckMessage(GameEventMessage @event, ulong accountId, uint unk, ushort value, string @string)
        {
            Event = @event;
            AccountId = accountId;
            Unk = unk;
            Value = value;
            String = @string;
        }
    }

    [BlubContract]
    public class GameBriefingInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public bool IsResult { get; set; }

        [BlubMember(1)]
        public bool IsEvent { get; set; }

        [BlubMember(2)]
        public byte[] Data { get; set; }

        public GameBriefingInfoAckMessage()
        {
            Data = Array.Empty<byte>();
        }

        public GameBriefingInfoAckMessage(bool isResult, bool isEvent, byte[] data)
        {
            IsResult = isResult;
            IsEvent = isEvent;
            Data = data;
        }
    }

    [BlubContract]
    public class GameChangeStateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public GameState State { get; set; }

        public GameChangeStateAckMessage()
        {
        }

        public GameChangeStateAckMessage(GameState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class GameChangeSubStateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public GameTimeState State { get; set; }

        public GameChangeSubStateAckMessage()
        {
        }

        public GameChangeSubStateAckMessage(GameTimeState state)
        {
            State = state;
        }
    }

    [BlubContract]
    public class GameDestroyGameRuleAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class RoomChangeMasterAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public RoomChangeMasterAckMessage()
        {
        }

        public RoomChangeMasterAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class RoomChangeRefereeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public RoomChangeRefereeAckMessage()
        {
        }

        public RoomChangeRefereeAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class SlaughterChangeSlaughterAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk { get; set; }

        public SlaughterChangeSlaughterAckMessage()
        {
            Unk = Array.Empty<ulong>();
        }

        public SlaughterChangeSlaughterAckMessage(ulong accountId)
        {
            AccountId = accountId;
            Unk = Array.Empty<ulong>();
        }

        public SlaughterChangeSlaughterAckMessage(ulong accountId, ulong[] unk)
        {
            AccountId = accountId;
            Unk = unk;
        }
    }

    [BlubContract]
    public class RoomReadyRoundAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public bool IsReady { get; set; }

        [BlubMember(2)]
        public byte Result { get; set; }

        public RoomReadyRoundAckMessage()
        {
        }

        public RoomReadyRoundAckMessage(ulong accountId, bool isReady)
        {
            AccountId = accountId;
            IsReady = isReady;
        }
    }

    [BlubContract]
    public class RoomBeginRoundAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class GameAvatarChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public GameAvatarChangeAckMessage()
        {
            Unk1 = new ChangeAvatarUnk1Dto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }

        public GameAvatarChangeAckMessage(ChangeAvatarUnk1Dto unk1, ChangeAvatarUnk2Dto[] unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class RoomChangeRuleNotifyAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public RoomChangeRuleNotifyAckMessage()
        {
            Settings = new ChangeRuleDto();
        }

        public RoomChangeRuleNotifyAckMessage(ChangeRuleDto settings)
        {
            Settings = settings;
        }
    }

    [BlubContract]
    public class RoomChangeRuleAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRuleDto Settings { get; set; }

        public RoomChangeRuleAckMessage()
        {
            Settings = new ChangeRuleDto();
        }

        public RoomChangeRuleAckMessage(ChangeRuleDto settings)
        {
            Settings = settings;
        }
    }

    [BlubContract]
    public class RoomChangeRuleFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Result { get; set; }
    }

    [BlubContract]
    public class ScoreMissionScoreAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong PlayerId { get; set; }

        [BlubMember(1)]
        public int ScoreGained { get; set; }

        public ScoreMissionScoreAckMessage()
        {
        }

        public ScoreMissionScoreAckMessage(ulong playerId, int scoreGained)
        {
            PlayerId = playerId;
            ScoreGained = scoreGained;
        }
    }

    [BlubContract]
    public class ScoreKillAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public ScoreKillAckMessage()
        {
            Score = new ScoreDto();
        }

        public ScoreKillAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreKillAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public ScoreKillAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public ScoreKillAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreOffenseAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public ScoreOffenseAckMessage()
        {
            Score = new ScoreDto();
        }

        public ScoreOffenseAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreOffenseAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public ScoreOffenseAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public ScoreOffenseAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreDefenseAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public ScoreDefenseAckMessage()
        {
            Score = new ScoreDto();
        }

        public ScoreDefenseAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreDefenseAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreAssistDto Score { get; set; }

        public ScoreDefenseAssistAckMessage()
        {
            Score = new ScoreAssistDto();
        }

        public ScoreDefenseAssistAckMessage(ScoreAssistDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreLeaderKillAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ScoreDto Score { get; set; }

        public ScoreLeaderKillAckMessage()
        {
            Score = new ScoreDto();
        }

        public ScoreLeaderKillAckMessage(ScoreDto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class ScoreHealAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        public ScoreHealAssistAckMessage()
        {
            Id = 0;
        }

        public ScoreHealAssistAckMessage(LongPeerId id)
        {
            Id = id;
        }
    }

    [BlubContract]
    public class ScoreGoalAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        public ScoreGoalAckMessage()
        {
            Id = 0;
        }

        public ScoreGoalAckMessage(LongPeerId id)
        {
            Id = id;
        }
    }

    [BlubContract]
    public class ScoreGoalAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public LongPeerId Assist { get; set; }

        public ScoreGoalAssistAckMessage()
        {
            Id = 0;
            Assist = 0;
        }

        public ScoreGoalAssistAckMessage(LongPeerId id, LongPeerId assist)
        {
            Id = id;
            Assist = assist;
        }
    }

    [BlubContract]
    public class ScoreReboundAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId NewId { get; set; }

        [BlubMember(1)]
        public LongPeerId OldId { get; set; }

        public ScoreReboundAckMessage()
        {
            NewId = 0;
            OldId = 0;
        }

        public ScoreReboundAckMessage(LongPeerId newId, LongPeerId oldId)
        {
            NewId = newId;
            OldId = oldId;
        }
    }

    [BlubContract]
    public class ScoreSuicideAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(EnumSerializer), typeof(int))]
        public AttackAttribute Icon { get; set; }

        public ScoreSuicideAckMessage()
        {
            Id = 0;
        }

        public ScoreSuicideAckMessage(LongPeerId id, AttackAttribute icon)
        {
            Id = id;
            Icon = icon;
        }
    }

    [BlubContract]
    public class ScoreTeamKillAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public Score2Dto Score { get; set; }

        public ScoreTeamKillAckMessage()
        {
            Score = new Score2Dto();
        }

        public ScoreTeamKillAckMessage(Score2Dto score)
        {
            Score = score;
        }
    }

    [BlubContract]
    public class SlaughterRoundWinAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        public SlaughterRoundWinAckMessage()
        {
        }

        public SlaughterRoundWinAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class SlaughterSLRoundWinAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class RoomChangeItemAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ChangeAvatarUnk2Dto[] Unk2 { get; set; }

        public RoomChangeItemAckMessage()
        {
            Unk1 = new ChangeItemsUnkDto();
            Unk2 = Array.Empty<ChangeAvatarUnk2Dto>();
        }

        public RoomChangeItemAckMessage(ChangeItemsUnkDto unk1, ChangeAvatarUnk2Dto[] unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class RoomPlayModeChangeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public PlayerGameMode Mode { get; set; }

        public RoomPlayModeChangeAckMessage()
        {
        }

        public RoomPlayModeChangeAckMessage(ulong accountId, PlayerGameMode mode)
        {
            AccountId = accountId;
            Mode = mode;
        }
    }

    [BlubContract]
    public class GameRefreshGameRuleInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class ArcadeScoreSyncAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncDto[] Scores { get; set; }

        public ArcadeScoreSyncAckMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncDto>();
        }
    }

    [BlubContract]
    public class ArcadeBeginRoundAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        public ArcadeBeginRoundAckMessage()
        {
        }

        public ArcadeBeginRoundAckMessage(byte unk1, byte unk2, byte unk3)
        {
            Unk1 = unk1;
            Unk2 = unk2;
            Unk3 = unk3;
        }
    }

    [BlubContract]
    public class ArcadeStageBriefingAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte[] Data { get; set; } // ToDo

        public ArcadeStageBriefingAckMessage()
        {
            Data = Array.Empty<byte>();
        }
    }

    [BlubContract]
    public class ArcadeEnablePlayTimeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        public ArcadeEnablePlayTimeAckMessage()
        {
        }

        public ArcadeEnablePlayTimeAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    [BlubContract]
    public class ArcadeStageInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        public ArcadeStageInfoAckMessage()
        {
        }

        public ArcadeStageInfoAckMessage(byte unk1, int unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }

    [BlubContract]
    public class ArcadeRespawnAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ArcadeDeathPlayerInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Players { get; set; }

        public ArcadeDeathPlayerInfoAckMessage()
        {
            Players = Array.Empty<ulong>();
        }
    }

    [BlubContract]
    public class ArcadeStageReadyAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class ArcadeRespawnFailAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public uint Result { get; set; }
    }

    [BlubContract]
    public class AdminChangeHPAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }

        public AdminChangeHPAckMessage()
        {
        }

        public AdminChangeHPAckMessage(float value)
        {
            Value = value;
        }
    }

    [BlubContract]
    public class AdminChangeMPAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public float Value { get; set; }

        public AdminChangeMPAckMessage()
        {
        }

        public AdminChangeMPAckMessage(float value)
        {
            Value = value;
        }
    }

    [BlubContract]
    public class ArcadeChangeStageAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Stage { get; set; }
    }

    [BlubContract]
    public class ArcadeStageSelectAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class ArcadeSaveDateInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk { get; set; }
    }

    [BlubContract]
    public class SlaughterAttackPointAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk1 { get; set; }

        [BlubMember(2)]
        public float Unk2 { get; set; }
    }

    [BlubContract]
    public class SlaughterHealPointAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public float Unk { get; set; }
    }

    [BlubContract]
    public class SlaughterChangeBonusTargetAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public SlaughterChangeBonusTargetAckMessage()
        {
        }

        public SlaughterChangeBonusTargetAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class ArcadeSucceedLoadingAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }
    }

    [BlubContract]
    public class MoneyUseCoinAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }

        [BlubMember(3)]
        public int Unk4 { get; set; }

        [BlubMember(4)]
        public byte Unk5 { get; set; }
    }

    [BlubContract]
    public class GameLuckyShotAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class FreeAllForChangeTheFirstAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        public FreeAllForChangeTheFirstAckMessage()
        {
        }

        public FreeAllForChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    [BlubContract]
    public class LogDevLogStartAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class GameKickOutRequestAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class GameKickOutVoteResultAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class GameKickOutStateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public uint NoCount { get; set; }

        [BlubMember(1)]
        public uint YesCount { get; set; }

        [BlubMember(2)]
        public uint NeededCount { get; set; }

        [BlubMember(3)]
        public VoteKickReason Reason { get; set; }

        [BlubMember(4)]
        public ulong Sender { get; set; }

        [BlubMember(5)]
        public ulong Target { get; set; }
    }

    [BlubContract]
    public class CaptainRoundCaptainLifeInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public CaptainLifeDto[] Players { get; set; }

        public CaptainRoundCaptainLifeInfoAckMessage()
        {
            Players = Array.Empty<CaptainLifeDto>();
        }
    }

    [BlubContract]
    public class CaptainSubRoundWinAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class CaptainCurrentRoundInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class SeizeUpdateInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public SeizeUpdateInfoDto[] Infos { get; set; }
    }

    [BlubContract]
    public class SeizeUpdateInfoByIntrudeAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public SeizeIntrudeInfoDto[] Infos { get; set; }
    }

    [BlubContract]
    public class SeizeFeverTimeAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class SeizeBuffItemGainAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ulong Unk1 { get; set; }

        [BlubMember(1)]
        public ulong Unk2 { get; set; }
    }

    [BlubContract]
    public class SeizeDropBuffItemAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk { get; set; }
    }

    [BlubContract]
    public class SeizeUpKeepScoreUpdateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class SeizeUpKeepScoreGetAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomChangeMasterRequestAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class RoomMixedTeamBriefingInfoAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public MixedTeamBriefingDto[] Unk2 { get; set; }
    }

    [BlubContract]
    public class GameEquipCheckAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class RoomGameStartAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class RoomGameLoadingAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class GameTackUpdateAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public short Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomGameEndLoadingAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public long Unk { get; set; }
    }

    [BlubContract]
    public class RoomGamePlayCountDownAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class InGameItemDropAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public ItemDropAckDto Item { get; set; }
    }

    [BlubContract]
    public class InGameItemGetAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class InGamePlayerResponseOfDeathAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class ChallengeRankersAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ChallengeRankerDto[] Rankers { get; set; }
    }

    [BlubContract]
    public class ChallengeRankingListAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ChallengeRankerDto[] Rankers { get; set; }
    }

    [BlubContract]
    public class PromotionCouponEventIngameGetAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class RoomEnterPlayerForBookNameTagsAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public long Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public byte Unk3 { get; set; }

        [BlubMember(3)]
        public int Un4 { get; set; }

        [BlubMember(4)]
        public string Unk5 { get; set; }

        [BlubMember(5)]
        public byte Unk6 { get; set; }

        [BlubMember(6)]
        public int Unk7 { get; set; }
    }

    [BlubContract]
    public class RoomEnterPlayerInfoListForNameTagAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public NameTagDto[] NameTags { get; set; }
    }

    [BlubContract]
    public class UseBurningBuffAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }
    }

    [BlubContract]
    public class RoomChangeRuleNotifyAck2Message : IGameRuleMessage
    {
        [BlubMember(0)]
        public ChangeRule2Dto Settings { get; set; }
    }

    [BlubContract]
    public class RoomRandomRoomBeginActionAckMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class ScoreAIKillAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Unk { get; set; }
    }

    [BlubContract]
    public class ScoreAIKillAssistAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public LongPeerId Unk { get; set; }
    }

    [BlubContract]
    public class SyncArenaBattleIdxMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ArenaSyncDto[] Unk2 { get; set; }

        [BlubMember(2)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ArenaSyncDto[] Unk3 { get; set; }
    }

    [BlubContract]
    public class SyncFirstArenaBattleIdxMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk1 { get; set; }

        [BlubMember(1)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Unk2 { get; set; }
    }

    [BlubContract]
    public class ScoreArenaDrawPlayMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class ArenaSetGameOptionAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk { get; set; }
    }

    [BlubContract]
    public class ArenaSpecialPointAckMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class ScoreArenaBattlePlayerLeaveMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public byte Unk1 { get; set; }

        [BlubMember(1)]
        public long Unk2 { get; set; }

        [BlubMember(2)]
        public long Unk3 { get; set; }
    }

    [BlubContract]
    public class ArenaLeaderShowdwonMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class ArenaDrawHealthPointReqMessage : IGameRuleMessage
    {
    }

    [BlubContract]
    public class ArenaHalfTimeStatusMessage : IGameRuleMessage
    {
        [BlubMember(0)]
        public int Unk1 { get; set; }

        [BlubMember(1)]
        public byte Unk2 { get; set; }

        [BlubMember(2)]
        public int Unk3 { get; set; }
    }

    [BlubContract]
    public class ArenaNotIntrudePlayerMessage : IGameRuleMessage
    {
    }
}
