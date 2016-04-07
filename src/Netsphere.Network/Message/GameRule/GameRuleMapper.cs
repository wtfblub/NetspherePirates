using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.GameRule
{
    internal static class GameRuleMapper
    {
        private static readonly Dictionary<GameRuleOpCode, Type> TypeLookup = new Dictionary<GameRuleOpCode, Type>();
        private static readonly Dictionary<Type, GameRuleOpCode> OpCodeLookup = new Dictionary<Type, GameRuleOpCode>();

        static GameRuleMapper()
        {
            // S2C
            Create<SEnterPlayerAckMessage>(GameRuleOpCode.SEnterPlayerAck);
            Create<SLeavePlayerAckMessage>(GameRuleOpCode.SLeavePlayerAck);
            Create<SLeavePlayerRequestAckMessage>(GameRuleOpCode.SLeavePlayerRequestAck);
            Create<SChangeTeamAckMessage>(GameRuleOpCode.SChangeTeamAck);
            Create<SChangeTeamFailAckMessage>(GameRuleOpCode.SChangeTeamFailAck);
            Create<SMixChangeTeamAckMessage>(GameRuleOpCode.SMixChangeTeamAck);
            Create<SMixChangeTeamFailAckMessage>(GameRuleOpCode.SMixChangeTeamFailAck);
            Create<SAutoAssignTeamAckMessage>(GameRuleOpCode.SAutoAssignTeamAck);
            Create<SEventMessageAckMessage>(GameRuleOpCode.SEventMessageAck);
            Create<SBriefingAckMessage>(GameRuleOpCode.SBriefingAck);
            Create<SChangeStateAckMessage>(GameRuleOpCode.SChangeStateAck);
            Create<SChangeSubStateAckMessage>(GameRuleOpCode.SChangeSubStateAck);
            Create<SDestroyGameRuleAckMessage>(GameRuleOpCode.SDestroyGameRuleAck);
            Create<SChangeMasterAckMessage>(GameRuleOpCode.SChangeMasterAck);
            Create<SChangeRefeReeAckMessage>(GameRuleOpCode.SChangeRefeReeAck);
            Create<SChangeTheFirstAckMessage>(GameRuleOpCode.SChangeTheFirstAck);
            Create<SChangeSlaughtererAckMessage>(GameRuleOpCode.SChangeSlaughtererAck);
            Create<SReadyRoundAckMessage>(GameRuleOpCode.SReadyRoundAck);
            Create<SBeginRoundAckMessage>(GameRuleOpCode.SBeginRoundAck);
            Create<SAvatarChangeAckMessage>(GameRuleOpCode.SAvatarChangeAck);
            Create<SChangeRuleNotifyAckMessage>(GameRuleOpCode.SChangeRuleNotifyAck);
            Create<SChangeRuleAckMessage>(GameRuleOpCode.SChangeRuleAck);
            Create<SChangeRuleResultMsgAckMessage>(GameRuleOpCode.SChangeRuleResultMsgAck);
            Create<SMissionNotifyAckMessage>(GameRuleOpCode.SMissionNotifyAck);
            Create<SMissionScoreAckMessage>(GameRuleOpCode.SMissionScoreAck);
            Create<SScoreKillAckMessage>(GameRuleOpCode.SScoreKillAck);
            Create<SScoreKillAssistAckMessage>(GameRuleOpCode.SScoreKillAssistAck);
            Create<SScoreOffenseAckMessage>(GameRuleOpCode.SScoreOffenseAck);
            Create<SScoreOffenseAssistAckMessage>(GameRuleOpCode.SScoreOffenseAssistAck);
            Create<SScoreDefenseAckMessage>(GameRuleOpCode.SScoreDefenseAck);
            Create<SScoreDefenseAssistAckMessage>(GameRuleOpCode.SScoreDefenseAssistAck);
            Create<SScoreHealAssistAckMessage>(GameRuleOpCode.SScoreHealAssistAck);
            Create<SScoreGoalAckMessage>(GameRuleOpCode.SScoreGoalAck);
            Create<SScoreGoalAssistAckMessage>(GameRuleOpCode.SScoreGoalAssistAck);
            Create<SScoreReboundAckMessage>(GameRuleOpCode.SScoreReboundAck);
            Create<SScoreSuicideAckMessage>(GameRuleOpCode.SScoreSuicideAck);
            Create<SScoreTeamKillAckMessage>(GameRuleOpCode.SScoreTeamKillAck);
            Create<SScoreRoundWinAckMessage>(GameRuleOpCode.SScoreRoundWinAck);
            Create<SScoreSLRoundWinAckMessage>(GameRuleOpCode.SScoreSLRoundWinAck);
            Create<SItemsChangeAckMessage>(GameRuleOpCode.SItemsChangeAck);
            Create<SPlayerGameModeChangeAckMessage>(GameRuleOpCode.SPlayerGameModeChangeAck);
            Create<SRefreshGameRuleInfoAckMessage>(GameRuleOpCode.SRefreshGameRuleInfoAck);
            Create<SArcadeScoreSyncAckMessage>(GameRuleOpCode.SArcadeScoreSyncAck);
            Create<SArcadeBeginRoundAckMessage>(GameRuleOpCode.SArcadeBeginRoundAck);
            Create<SArcadeStageBriefingAckMessage>(GameRuleOpCode.SArcadeStageBriefingAck);
            Create<SArcadeEnablePlayeTimeAckMessage>(GameRuleOpCode.SArcadeEnablePlayeTimeAck);
            Create<SArcadeStageInfoAckMessage>(GameRuleOpCode.SArcadeStageInfoAck);
            Create<SArcadeRespawnAckMessage>(GameRuleOpCode.SArcadeRespawnAck);
            Create<SArcadeDeathPlayerInfoAckMessage>(GameRuleOpCode.SArcadeDeathPlayerInfoAck);
            Create<SArcadeStageReadyAckMessage>(GameRuleOpCode.SArcadeStageReadyAck);
            Create<SArcadeRespawnFailAckMessage>(GameRuleOpCode.SArcadeRespawnFailAck);
            Create<SChangeHPAckMessage>(GameRuleOpCode.SChangeHPAck);
            Create<SChangeMPAckMessage>(GameRuleOpCode.SChangeMPAck);
            Create<SArcadeChangeStageAckMessage>(GameRuleOpCode.SArcadeChangeStageAck);
            Create<SArcadeStageSelectAckMessage>(GameRuleOpCode.SArcadeStageSelectAck);
            Create<SArcadeSaveDataInfAckMessage>(GameRuleOpCode.SArcadeSaveDataInfAck);
            Create<SSlaughterAttackPointAckMessage>(GameRuleOpCode.SSlaughterAttackPointAck);
            Create<SSlaughterHealPointAckMessage>(GameRuleOpCode.SSlaughterHealPointAck);
            Create<SChangeBonusTargetAckMessage>(GameRuleOpCode.SChangeBonusTargetAck);
            Create<SArcadeLoadingSucceedAckMessage>(GameRuleOpCode.SArcadeLoadingSucceedAck);
            Create<SArcadeAllLoadingSucceedAckMessage>(GameRuleOpCode.SArcadeAllLoadingSucceedAck);
            Create<SUseCoinAckMessage>(GameRuleOpCode.SUseCoinAck);
            Create<SLuckyShotAckMessage>(GameRuleOpCode.SLuckyShotAck);
            Create<SGameRuleChangeTheFirstAckMessage>(GameRuleOpCode.SGameRuleChangeTheFirstAck);
            Create<SDevLogStartAckMessage>(GameRuleOpCode.SDevLogStartAck);
            Create<SCompulsionLeaveRequestAckMessage>(GameRuleOpCode.SCompulsionLeaveRequestAck);
            Create<SCompulsionLeaveResultAckMessage>(GameRuleOpCode.SCompulsionLeaveResultAck);
            Create<SCompulsionLeaveActionAckMessage>(GameRuleOpCode.SCompulsionLeaveActionAck);
            Create<SCaptainLifeRoundSetUpAckMessage>(GameRuleOpCode.SCaptainLifeRoundSetUpAck);
            Create<SCaptainSubRoundEndReasonAckMessage>(GameRuleOpCode.SCaptainSubRoundEndReasonAck);
            Create<SCurrentRoundInformationAckMessage>(GameRuleOpCode.SCurrentRoundInformationAck);

            // C2S
            Create<CEnterPlayerReqMessage>(GameRuleOpCode.CEnterPlayerReq);
            Create<CLeavePlayerRequestReqMessage>(GameRuleOpCode.CLeavePlayerRequestReq);
            Create<CChangeTeamReqMessage>(GameRuleOpCode.CChangeTeamReq);
            Create<CAutoAssingTeamReqMessage>(GameRuleOpCode.CAutoAssingTeamReq);
            Create<CAutoMixingTeamReqMessage>(GameRuleOpCode.CAutoMixingTeamReq);
            Create<CMixChangeTeamReqMessage>(GameRuleOpCode.CMixChangeTeamReq);
            Create<CEventMessageReqMessage>(GameRuleOpCode.CEventMessageReq);
            Create<CReadyRoundReqMessage>(GameRuleOpCode.CReadyRoundReq);
            Create<CBeginRoundReqMessage>(GameRuleOpCode.CBeginRoundReq);
            Create<CAvatarDurabilityDecreaseReqMessage>(GameRuleOpCode.CAvatarDurabilityDecreaseReq);
            Create<CAvatarChangeReqMessage>(GameRuleOpCode.CAvatarChangeReq);
            Create<CChangeRuleNotifyReqMessage>(GameRuleOpCode.CChangeRuleNotifyReq);
            Create<CMissionScoreReqMessage>(GameRuleOpCode.CMissionScoreReq);
            Create<CScoreKillReqMessage>(GameRuleOpCode.CScoreKillReq);
            Create<CScoreKillAssistReqMessage>(GameRuleOpCode.CScoreKillAssistReq);
            Create<CScoreOffenseReqMessage>(GameRuleOpCode.CScoreOffenseReq);
            Create<CScoreOffenseAssistReqMessage>(GameRuleOpCode.CScoreOffenseAssistReq);
            Create<CScoreDefenseReqMessage>(GameRuleOpCode.CScoreDefenseReq);
            Create<CScoreDefenseAssistReqMessage>(GameRuleOpCode.CScoreDefenseAssistReq);
            Create<CScoreHealAssistReqMessage>(GameRuleOpCode.CScoreHealAssistReq);
            Create<CScoreGoalReqMessage>(GameRuleOpCode.CScoreGoalReq);
            Create<CScoreReboundReqMessage>(GameRuleOpCode.CScoreReboundReq);
            Create<CScoreSuicideReqMessage>(GameRuleOpCode.CScoreSuicideReq);
            Create<CScoreTeamKillReqMessage>(GameRuleOpCode.CScoreTeamKillReq);
            Create<CItemsChangeReqMessage>(GameRuleOpCode.CItemsChangeReq);
            Create<CPlayerGameModeChangeReqMessage>(GameRuleOpCode.CPlayerGameModeChangeReq);
            Create<CArcadeAttackPointReqMessage>(GameRuleOpCode.CArcadeAttackPointReq);
            Create<CArcadeScoreSyncReqMessage>(GameRuleOpCode.CArcadeScoreSyncReq);
            Create<CArcadeBeginRoundReqMessage>(GameRuleOpCode.CArcadeBeginRoundReq);
            Create<CArcadeStageClearReqMessage>(GameRuleOpCode.CArcadeStageClearReq);
            Create<CArcadeStageFailedReqMessage>(GameRuleOpCode.CArcadeStageFailedReq);
            Create<CArcadeStageInfoReqMessage>(GameRuleOpCode.CArcadeStageInfoReq);
            Create<CArcadeEnablePlayTimeReqMessage>(GameRuleOpCode.CArcadeEnablePlayTimeReq);
            Create<CArcadeRespawnReqMessage>(GameRuleOpCode.CArcadeRespawnReq);
            Create<CArcadeStageReadyReqMessage>(GameRuleOpCode.CArcadeStageReadyReq);
            Create<CArcadeStageSelectReqMessage>(GameRuleOpCode.CArcadeStageSelectReq);
            Create<CSlaughterAttackPointReqMessage>(GameRuleOpCode.CSlaughterAttackPointReq);
            Create<CSlaughterHealPointReqMessage>(GameRuleOpCode.CSlaughterHealPointReq);
            Create<CArcadeLoadingSucceesReqMessage>(GameRuleOpCode.CArcadeLoadingSucceesReq);
            Create<CUseCoinReqMessage>(GameRuleOpCode.CUseCoinReq);
            Create<CBeginResponeReqMessage>(GameRuleOpCode.CBeginResponeReq);
            Create<CWeaponFireReqMessage>(GameRuleOpCode.CWeaponFireReq);
            Create<CCompulsionLeaveRequestReqMessage>(GameRuleOpCode.CCompulsionLeaveRequestReq);
            Create<CCompulsionLeaveVoteReqMessage>(GameRuleOpCode.CCompulsionLeaveVoteReq);
        }

        public static void Create<T>(GameRuleOpCode opCode)
            where T : GameRuleMessage, new()
        {
            var type = typeof(T);
            OpCodeLookup.Add(type, opCode);
            TypeLookup.Add(opCode, type);
        }

        public static GameRuleMessage GetMessage(GameRuleOpCode opCode, BinaryReader r)
        {
            var type = TypeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new NetsphereBadOpCodeException(opCode);

            return (GameRuleMessage)Serializer.Deserialize(r, type);
        }

        public static GameRuleOpCode GetOpCode<T>()
            where T : GameRuleMessage
        {
            return GetOpCode(typeof(T));
        }

        public static GameRuleOpCode GetOpCode(Type type)
        {
            return OpCodeLookup[type];
        }
    }
}
