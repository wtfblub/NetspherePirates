using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        public void SendScoreKill(Player killer, Player assist, Player target, AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new SScoreKillAssistAckMessage(
                    new ScoreAssistDto(killer.PeerId, assist.PeerId, target.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreKillAckMessage(
                    new ScoreDto(killer.PeerId, target.PeerId, attackAttribute)));
            }
        }

        public void SendScoreTeamKill(Player killer, Player target, AttackAttribute attackAttribute)
        {
            Room.Broadcast(new SScoreTeamKillAckMessage(
                new Score2Dto(killer.PeerId, target.PeerId, attackAttribute)));
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

        public void SendScoreOffense(Player killer, Player assist, Player target,
            AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new SScoreOffenseAssistAckMessage(
                    new ScoreAssistDto(killer.PeerId, assist.PeerId, target.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreOffenseAckMessage(
                    new ScoreDto(killer.PeerId, target.PeerId, attackAttribute)));
            }
        }

        public void SendScoreDefense(Player killer, Player assist, Player target,
            AttackAttribute attackAttribute)
        {
            if (assist != null)
            {
                Room.Broadcast(new SScoreDefenseAssistAckMessage(
                    new ScoreAssistDto(killer.PeerId, assist.PeerId, target.PeerId, attackAttribute)));
            }
            else
            {
                Room.Broadcast(new SScoreDefenseAckMessage(
                    new ScoreDto(killer.PeerId, target.PeerId, attackAttribute)));
            }
        }
    }
}
