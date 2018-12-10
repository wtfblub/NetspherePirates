using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Server.Game.GameRules;

namespace Netsphere.Server.Game
{
    using GameRuleFactory = Func<Room, GameRuleBase>;

    public class GameRuleManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyDictionary<GameRule, Type> _gameRuleLookup;

        public GameRuleManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gameRuleLookup = new Dictionary<GameRule, Type>
            {
                [GameRule.Deathmatch] = typeof(Deathmatch)
            };
        }

        public bool HasGameRule(GameRule gameRule)
        {
            return _gameRuleLookup.ContainsKey(gameRule);
        }

        public GameRuleBase GetGameRule(GameRule gameRule, Room room)
        {
            return _gameRuleLookup.TryGetValue(gameRule, out var type)
                ? (GameRuleBase)_serviceProvider.GetRequiredService(type)
                : null;
        }
    }
}
