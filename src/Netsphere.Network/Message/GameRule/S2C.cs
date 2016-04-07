using System;
using BlubLib.Serialization;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Serializers;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.GameRule
{
    public class SEnterPlayerAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public byte Unk1 { get; set; } // 0 = char does not spawn

        [Serialize(2, typeof(EnumSerializer))]
        public PlayerGameMode PlayerGameMode { get; set; }

        [Serialize(3)]
        public int Unk3 { get; set; }

        [Serialize(4, typeof(StringSerializer))]
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

    public class SLeavePlayerAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(StringSerializer))]
        public string Nickname { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
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

    public class SLeavePlayerRequestAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; } // result?
    }

    public class SChangeTeamAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public Team Team { get; set; }

        [Serialize(2, typeof(EnumSerializer))]
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

    public class SChangeTeamFailAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public ChangeTeamResult Result { get; set; }

        public SChangeTeamFailAckMessage()
        { }

        public SChangeTeamFailAckMessage(ChangeTeamResult result)
        {
            Result = result;
        }
    }

    public class SMixChangeTeamAckMessage : GameRuleMessage
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

    public class SMixChangeTeamFailAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Result { get; set; }
    }

    public class SAutoAssignTeamAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class SEventMessageAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public GameEventMessage Event { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(2)]
        public uint Unk { get; set; } // server/game time or something like that

        [Serialize(3)]
        public ushort Value { get; set; }

        [Serialize(4, typeof(StringSerializer))]
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

    public class SBriefingAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public bool IsResult { get; set; }

        [Serialize(1)]
        public bool IsEvent { get; set; }

        [Serialize(2, typeof(ArrayWithScalarSerializer))]
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

    public class SChangeStateAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public GameState State { get; set; }

        public SChangeStateAckMessage()
        { }

        public SChangeStateAckMessage(GameState state)
        {
            State = state;
        }
    }

    public class SChangeSubStateAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(EnumSerializer))]
        public GameTimeState State { get; set; }

        public SChangeSubStateAckMessage()
        { }

        public SChangeSubStateAckMessage(GameTimeState state)
        {
            State = state;
        }
    }

    public class SDestroyGameRuleAckMessage : GameRuleMessage
    { }

    public class SChangeMasterAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SChangeMasterAckMessage()
        { }

        public SChangeMasterAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SChangeRefeReeAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SChangeRefeReeAckMessage()
        { }

        public SChangeRefeReeAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SChangeTheFirstAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SChangeTheFirstAckMessage()
        { }

        public SChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SChangeSlaughtererAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
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

    public class SReadyRoundAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public bool IsReady { get; set; }

        [Serialize(2)]
        public byte Result { get; set; }

        public SReadyRoundAckMessage()
        { }

        public SReadyRoundAckMessage(ulong accountId, bool isReady)
        {
            AccountId = accountId;
            IsReady = isReady;
        }
    }

    public class SBeginRoundAckMessage : GameRuleMessage
    { }

    public class SAvatarChangeAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ChangeAvatarUnk1Dto Unk1 { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
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

    public class SChangeRuleNotifyAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SChangeRuleAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SChangeRuleResultMsgAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Result { get; set; }
    }

    public class SMissionNotifyAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class SMissionScoreAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class SScoreKillAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreKillAssistAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreOffenseAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreOffenseAssistAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreDefenseAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreDefenseAssistAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreHealAssistAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreGoalAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreGoalAssistAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }

        [Serialize(1)]
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

    public class SScoreReboundAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId NewId { get; set; }

        [Serialize(1)]
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

    public class SScoreSuicideAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public LongPeerId Id { get; set; }

        [Serialize(1, typeof(EnumSerializer), typeof(uint))]
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

    public class SScoreTeamKillAckMessage : GameRuleMessage
    {
        [Serialize(0)]
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

    public class SScoreRoundWinAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        public SScoreRoundWinAckMessage()
        { }

        public SScoreRoundWinAckMessage(byte unk)
        {
            Unk = unk;
        }
    }

    public class SScoreSLRoundWinAckMessage : GameRuleMessage
    { }

    public class SItemsChangeAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ChangeItemsUnkDto Unk1 { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
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

    public class SPlayerGameModeChangeAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1, typeof(EnumSerializer))]
        public PlayerGameMode Mode { get; set; }

        public SPlayerGameModeChangeAckMessage()
        { }

        public SPlayerGameModeChangeAckMessage(ulong accountId, PlayerGameMode mode)
        {
            AccountId = accountId;
            Mode = mode;
        }
    }

    public class SRefreshGameRuleInfoAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }
    }

    public class SArcadeScoreSyncAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public ArcadeScoreSyncDto[] Scores { get; set; }

        public SArcadeScoreSyncAckMessage()
        {
            Scores = Array.Empty<ArcadeScoreSyncDto>();
        }
    }

    public class SArcadeBeginRoundAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class SArcadeStageBriefingAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }

        [Serialize(2, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; } // ToDo

        public SArcadeStageBriefingAckMessage()
        {
            Data = Array.Empty<byte>();
        }
    }

    public class SArcadeEnablePlayeTimeAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class SArcadeStageInfoAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class SArcadeRespawnAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class SArcadeDeathPlayerInfoAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }

        [Serialize(1, typeof(ArrayWithIntPrefixSerializer))]
        public ulong[] Players { get; set; }

        public SArcadeDeathPlayerInfoAckMessage()
        {
            Players = Array.Empty<ulong>();
        }
    }

    public class SArcadeStageReadyAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class SArcadeRespawnFailAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public uint Result { get; set; }
    }

    public class SChangeHPAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public float Value { get; set; }
    }

    public class SChangeMPAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public float Value { get; set; }
    }

    public class SArcadeChangeStageAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Stage { get; set; }
    }

    public class SArcadeStageSelectAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class SArcadeSaveDataInfAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public byte Unk { get; set; }
    }

    public class SSlaughterAttackPointAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public float Unk1 { get; set; }

        [Serialize(2)]
        public float Unk2 { get; set; }
    }

    public class SSlaughterHealPointAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        [Serialize(1)]
        public float Unk { get; set; }
    }

    public class SChangeBonusTargetAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SChangeBonusTargetAckMessage()
        { }

        public SChangeBonusTargetAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SArcadeLoadingSucceedAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }
    }

    public class SArcadeAllLoadingSucceedAckMessage : GameRuleMessage
    { }

    public class SUseCoinAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }

        [Serialize(3)]
        public int Unk4 { get; set; }
    }

    public class SLuckyShotAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }

        [Serialize(2)]
        public int Unk3 { get; set; }
    }

    public class SGameRuleChangeTheFirstAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public ulong AccountId { get; set; }

        public SGameRuleChangeTheFirstAckMessage()
        { }

        public SGameRuleChangeTheFirstAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
    }

    public class SDevLogStartAckMessage : GameRuleMessage
    { }

    public class SCompulsionLeaveRequestAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class SCompulsionLeaveResultAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public ulong Unk2 { get; set; }

        [Serialize(2)]
        public ulong Unk3 { get; set; }

        [Serialize(3)]
        public int Unk4 { get; set; }

        [Serialize(4)]
        public int Unk5 { get; set; }

        [Serialize(5)]
        public int Unk6 { get; set; }
    }

    public class SCompulsionLeaveActionAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk { get; set; }
    }

    public class SCaptainLifeRoundSetUpAckMessage : GameRuleMessage
    {
        [Serialize(0, typeof(ArrayWithIntPrefixSerializer))]
        public CaptainLifeDto[] Players { get; set; }

        public SCaptainLifeRoundSetUpAckMessage()
        {
            Players = Array.Empty<CaptainLifeDto>();
        }
    }

    public class SCaptainSubRoundEndReasonAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public byte Unk2 { get; set; }
    }

    public class SCurrentRoundInformationAckMessage : GameRuleMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }
}
