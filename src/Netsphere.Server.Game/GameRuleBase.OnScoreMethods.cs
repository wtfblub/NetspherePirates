namespace Netsphere.Server.Game
{
    public abstract partial class GameRuleBase
    {
        protected internal virtual void OnScoreMission(int unk)
        {
        }

        protected internal virtual void OnScoreKill(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
        }

        protected internal virtual void OnScoreTeamKill(ScoreContext killer, ScoreContext target,
            AttackAttribute attackAttribute)
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

        protected internal virtual void OnScoreOffense(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
        }

        protected internal virtual void OnScoreDefense(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
        }
    }
}
