using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Netsphere.Game.GameRules;

// ReSharper disable once CheckNamespace
namespace Netsphere.Game
{
    internal class GameRuleFactory
    {
        private readonly IDictionary<GameRule, Func<Room, GameRuleBase>> _gameRules = new ConcurrentDictionary<GameRule, Func<Room, GameRuleBase>>();

        public GameRuleFactory()
        {
            Add(GameRule.Touchdown, room => new TouchdownGameRule(room));
            Add(GameRule.Deathmatch, room => new DeathmatchGameRule(room));
            Add(GameRule.Chaser, room => new ChaserGameRule(room));
            Add(GameRule.BattleRoyal, room => new BattleRoyalGameRule(room));
        }

        public void Add(GameRule gameRule, Func<Room, GameRuleBase> gameRuleFactory)
        {
            if (!_gameRules.TryAdd(gameRule, gameRuleFactory))
                throw new Exception($"GameRule {gameRule} already registered");
        }

        public void Remove(GameRuleBase gameRule)
        {
            _gameRules.Remove(gameRule.GameRule);
        }

        public GameRuleBase Get(GameRule gameRule, Room room)
        {
            Func<Room, GameRuleBase> gameRuleFactory;
            if (!_gameRules.TryGetValue(gameRule, out gameRuleFactory))
                throw new Exception($"GameRule {gameRule} not registered");

            return gameRuleFactory(room);
        }

        public bool Contains(GameRule gameRule)
        {
            return _gameRules.ContainsKey(gameRule);
        }
    }
}
