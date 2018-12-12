namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        protected internal virtual void OnScoreMission(int unk)
        {
        }

        protected internal virtual void OnScoreKill(Player killer, Player assist, Player target, AttackAttribute attackAttribute)
        {
        }

        protected internal virtual void OnScoreTeamKill(Player killer, Player target, AttackAttribute attackAttribute)
        {
        }

        protected internal virtual void OnScoreHeal(Player plr)
        {
        }

        protected internal virtual void OnScoreSuicide(Player plr)
        {
        }

        protected internal virtual void OnScoreTouchdown(Player plr)
        {
        }

        protected internal virtual void OnScoreFumbi(Player newPlr, Player oldPlr)
        {
        }

        protected internal virtual void OnScoreOffense(Player killer, Player assist, Player target,
            AttackAttribute attackAttribute)
        {
        }

        protected internal virtual void OnScoreDefense(Player killer, Player assist, Player target,
            AttackAttribute attackAttribute)
        {
        }
    }
}
