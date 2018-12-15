namespace Netsphere.Server.Game
{
    public class DefaultGameRuleResolver : IGameRuleResolver
    {
        private readonly GameRuleManager _gameRuleManager;

        public DefaultGameRuleResolver(GameRuleManager gameRuleManager)
        {
            _gameRuleManager = gameRuleManager;
        }

        public GameRuleBase Resolve(Room room)
        {
            return _gameRuleManager.GetGameRule(room.Options.MatchKey.GameRule);
        }
    }
}