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
                Room.Broadcast(new SScoreKillAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreKillAckMessage(
                    new ScoreDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
        }

        public void SendScoreTeamKill(ScoreContext killer, ScoreContext target, AttackAttribute attackAttribute)
        {
            Room.Broadcast(new SScoreTeamKillAckMessage(
                new Score2Dto(
                    killer.SentryId ?? killer.Player.PeerId,
                    target.SentryId ?? target.Player.PeerId, attackAttribute)));
        }

        public void SendScoreHeal(Player plr)
        {
            Room.Broadcast(new SScoreHealAssistAckMessage(plr.PeerId));
        }

        public void SendScoreSuicide(Player plr)
        {
            Room.Broadcast(new SScoreSuicideAckMessage(plr.PeerId, AttackAttribute.KillOneSelf));
        }

        public void SendScoreTouchdown(Player plr, Player assist)
        {
            if (assist != null)
                Room.Broadcast(new SScoreGoalAssistAckMessage(plr.PeerId, assist.PeerId));
            else
                Room.Broadcast(new SScoreGoalAckMessage(plr.PeerId));
        }

        public void SendScoreFumbi(Player newPlayer, Player oldPlayer)
        {
            Room.Broadcast(new SScoreReboundAckMessage(newPlayer?.PeerId ?? 0, oldPlayer?.PeerId ?? 0));
        }

        public void SendScoreOffense(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new SScoreOffenseAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreOffenseAckMessage(
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
                Room.Broadcast(new SScoreDefenseAssistAckMessage(
                    new ScoreAssistDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        assist.SentryId ?? assist.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreDefenseAckMessage(
                    new ScoreDto(
                        killer.SentryId ?? killer.Player.PeerId,
                        target.SentryId ?? target.Player.PeerId, attackAttribute)));
            }
        }
    }
}
