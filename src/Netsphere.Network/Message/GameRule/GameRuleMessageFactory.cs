using ProudNet.Serialization;

namespace Netsphere.Network.Message.GameRule
{
    public interface IGameRuleMessage
    {
    }

    public class GameRuleMessageFactory : MessageFactory<GameRuleOpCode, IGameRuleMessage>
    {
        public GameRuleMessageFactory()
        {
            // S2C

            // C2S
            Register<RoomEnterPlayerReqMessage>(GameRuleOpCode.RoomEnterPlayerReq);
            Register<RoomLeaveReguestReqMessage>(GameRuleOpCode.RoomLeaveReguestReq);
            Register<RoomTeamChangeReqMessage>(GameRuleOpCode.RoomTeamChangeReq);
            Register<RoomAutoAssingTeamReqMessage>(GameRuleOpCode.RoomAutoAssingTeamReq);
            Register<RoomAutoMixingTeamReqMessage>(GameRuleOpCode.RoomAutoMixingTeamReq);
            Register<RoomChoiceTeamChangeReqMessage>(GameRuleOpCode.RoomChoiceTeamChangeReq);
            Register<GameEventMessageReqMessage>(GameRuleOpCode.GameEventMessageReq);
            Register<RoomReadyRoundReqMessage>(GameRuleOpCode.RoomReadyRoundReq);
            Register<RoomBeginRoundReqMessage>(GameRuleOpCode.RoomBeginRoundReq);
            Register<GameAvatarDurabilityDecreaseReqMessage>(GameRuleOpCode.GameAvatarDurabilityDecreaseReq);
            Register<GameAvatarChangeReqMessage>(GameRuleOpCode.GameAvatarChangeReq);
            Register<RoomChangeRuleNotifyReqMessage>(GameRuleOpCode.RoomChangeRuleNotifyReq);
            Register<ScoreMissionScoreReqMessage>(GameRuleOpCode.ScoreMissionScoreReq);
            Register<ScoreKillReqMessage>(GameRuleOpCode.ScoreKillReq);
            Register<ScoreKillAssistReqMessage>(GameRuleOpCode.ScoreKillAssistReq);
            Register<ScoreOffenseReqMessage>(GameRuleOpCode.ScoreOffenseReq);
            Register<ScoreOffenseAssistReqMessage>(GameRuleOpCode.ScoreOffenseAssistReq);
            Register<ScoreDefenseReqMessage>(GameRuleOpCode.ScoreDefenseReq);
            Register<ScoreDefenseAssistReqMessage>(GameRuleOpCode.ScoreDefenseAssistReq);
            Register<ScoreHealAssistReqMessage>(GameRuleOpCode.ScoreHealAssistReq);
            Register<ScoreGoalReqMessage>(GameRuleOpCode.ScoreGoalReq);
            Register<ScoreReboundReqMessage>(GameRuleOpCode.ScoreReboundReq);
            Register<ScoreSuicideReqMessage>(GameRuleOpCode.ScoreSuicideReq);
            Register<ScoreTeamKillReqMessage>(GameRuleOpCode.ScoreTeamKillReq);
            Register<RoomItemChangeReqMessage>(GameRuleOpCode.RoomItemChangeReq);
            Register<RoomPlayModeChangeReqMessage>(GameRuleOpCode.RoomPlayModeChangeReq);
            Register<ArcadeScoreSyncReqMessage>(GameRuleOpCode.ArcadeScoreSyncReq);
            Register<ArcadeBeginRoundReqMessage>(GameRuleOpCode.ArcadeBeginRoundReq);
            Register<ArcadeStageClearReqMessage>(GameRuleOpCode.ArcadeStageClearReq);
            Register<ArcadeStageFailedReqMessage>(GameRuleOpCode.ArcadeStageFailedReq);
            Register<ArcadeStageInfoReqMessage>(GameRuleOpCode.ArcadeStageInfoReq);
            Register<ArcadeEnablePlayTimeReqMessage>(GameRuleOpCode.ArcadeEnablePlayTimeReq);
            Register<ArcardRespawnReqMessage>(GameRuleOpCode.ArcardRespawnReq);
            Register<ArcadeStageReadyReqMessage>(GameRuleOpCode.ArcadeStageReadyReq);
            Register<ArcadeStageSelectReqMessage>(GameRuleOpCode.ArcadeStageSelectReq);
            Register<SlaughterAttackPointReqMessage>(GameRuleOpCode.SlaughterAttackPointReq);
            Register<SlaughterHealPointReqMessage>(GameRuleOpCode.SlaughterHealPointReq);
            Register<ArcadeLoagdingSuccessReqMessage>(GameRuleOpCode.ArcadeLoagdingSuccessReq);
            Register<MoneyUseCoinReqMessage>(GameRuleOpCode.MoneyUseCoinReq);
            Register<LogBeginResponeReqMessage>(GameRuleOpCode.LogBeginResponeReq);
            Register<LogWeaponFireReqMessage>(GameRuleOpCode.LogWeaponFireReq);
            Register<GameKickOutRequestReqMessage>(GameRuleOpCode.GameKickOutRequestReq);
            Register<GameKickOutVoteResultReqMessage>(GameRuleOpCode.GameKickOutVoteResultReq);
            Register<RoomIntrudeRoundReqMessage>(GameRuleOpCode.RoomIntrudeRoundReq);
            Register<GameLoadingSuccessReqMessage>(GameRuleOpCode.GameLoadingSuccessReq);
            Register<SeizePositionCaptureReqMessage>(GameRuleOpCode.SeizePositionCaptureReq);
            Register<SeizeBuffItemGainReqMessage>(GameRuleOpCode.SeizeBuffItemGainReq);
            Register<RoomChoiceMasterChangeReqMessage>(GameRuleOpCode.RoomChoiceMasterChangeReq);
            Register<GameEquipCheckReqMessage>(GameRuleOpCode.GameEquipCheckReq);
            Register<PromotionCointEventGetCoinReqMessage>(GameRuleOpCode.PromotionCointEventGetCoinReq);
            Register<InGameItemDropReqMessage>(GameRuleOpCode.InGameItemDropReq);
            Register<InGameItemGetReqMessage>(GameRuleOpCode.InGameItemGetReq);
            Register<InGamePlayerResponseReqMessage>(GameRuleOpCode.InGamePlayerResponseReq);
            Register<ChallengeRankingListReqMessage>(GameRuleOpCode.ChallengeRankingListReq);
            Register<ChallengeResultReqMessage>(GameRuleOpCode.ChallengeResultReq);
            Register<ChallengeReStartReqMessage>(GameRuleOpCode.ChallengeReStartReq);
            Register<PromotionCouponEventIngameGetReqMessage>(GameRuleOpCode.PromotionCouponEventIngameGetReq);
            Register<RecordBurningDataMessage>(GameRuleOpCode.RecordBurningData);
            Register<RoomReadyRoundReq2Message>(GameRuleOpCode.RoomReadyRoundReq2);
            Register<RoomBeginRoundReq2Message>(GameRuleOpCode.RoomBeginRoundReq2);
            Register<RoomIntrudeRoundReq2Message>(GameRuleOpCode.RoomIntrudeRoundReq2);
            Register<UseBurningBuffReqMessage>(GameRuleOpCode.UseBurningBuffReq);
            Register<RoomChangeRuleNotifyReq2Message>(GameRuleOpCode.RoomChangeRuleNotifyReq2);
            Register<RoomRandomRoomBeginActionReqMessage>(GameRuleOpCode.RoomRandomRoomBeginActionReq);
            Register<ScoreAIKillReqMessage>(GameRuleOpCode.ScoreAIKillReq);
            Register<ArenaSetGameOptionReqMessage>(GameRuleOpCode.ArenaSetGameOptionReq);
            Register<ArenaSpecialPointReqMessage>(GameRuleOpCode.ArenaSpecialPointReq);
            Register<ArenaDrawHealthPointAckMessage>(GameRuleOpCode.ArenaDrawHealthPointAck);
        }
    }
}
