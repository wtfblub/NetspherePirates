using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        public void SendScoreKill(ScoreContext killer, ScoreContext assist, ScoreContext target, AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new ScoreKillAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new ScoreKillAckMessage(
                    new ScoreDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
        }

        public void SendScoreTeamKill(ScoreContext killer, ScoreContext target, AttackAttribute attackAttribute)
        {
            Room.Broadcast(new ScoreTeamKillAckMessage(
                new Score2Dto(
                    killer.SentryId ?? killer.Player.PeerId,
                    target.SentryId ?? target.Player.PeerId, attackAttribute)));
        }

        public void SendScoreHeal(Player plr)
        {
            Room.Broadcast(new ScoreHealAssistAckMessage(plr.PeerId));
        }

        public void SendScoreSuicide(Player plr)
        {
            Room.Broadcast(new ScoreSuicideAckMessage(plr.PeerId, AttackAttribute.KillOneSelf));
        }

        public void SendScoreTouchdown(Player plr, Player assist)
        {
            if (assist != null)
                Room.Broadcast(new ScoreGoalAssistAckMessage(plr.PeerId, assist.PeerId));
            else
                Room.Broadcast(new ScoreGoalAckMessage(plr.PeerId));
        }

        public void SendScoreFumbi(Player newPlayer, Player oldPlayer)
        {
            Room.Broadcast(new ScoreReboundAckMessage(newPlayer?.PeerId ?? 0, oldPlayer?.PeerId ?? 0));
        }

        public void SendScoreOffense(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new ScoreOffenseAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new ScoreOffenseAckMessage(
                    new ScoreDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
        }

        public void SendScoreDefense(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new ScoreDefenseAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new ScoreDefenseAckMessage(
                    new ScoreDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
        }

        public void SendScoreMission(ScoreContext context, int scoreGained)
        {
            Room.Broadcast(new ScoreMissionScoreAckMessage(context.Player.Account.Id, scoreGained));
        }
    }
}
