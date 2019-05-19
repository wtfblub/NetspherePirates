using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Server.Game.GameRules;

namespace Netsphere.Server.Game
{
    public class GameRuleResolver
    {
        private readonly IServiceProvider _serviceProvider;
        // private readonly ConcurrentStack<Func<RoomCreationOptions, Type>> _gameRules;
        private readonly Dictionary<GameRule, List<Entry>> _gameRules;

        public GameRuleResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gameRules = new Dictionary<GameRule, List<Entry>>();

            Register(GameRule.Deathmatch, x => typeof(Deathmatch));
            Register(GameRule.Touchdown, x => typeof(Touchdown));
            Register(GameRule.BattleRoyal, x => typeof(BattleRoyal));
            Register(GameRule.Practice, X => typeof(Practice));
        }

        /// <param name="priority">Higher value means higher priority. Default is 10</param>
        public void Register(GameRule gameRule, Func<RoomCreationOptions, Type> gameRuleType, int priority = 10)
        {
            if (!_gameRules.TryGetValue(gameRule, out var entries))
                entries = new List<Entry>();

            entries.Add(new Entry(priority, gameRuleType));
            _gameRules[gameRule] = entries;
        }

        public bool HasGameRule(RoomCreationOptions roomCreationOptions)
        {
            return GetGameRuleType(roomCreationOptions) != null;
        }

        public GameRuleBase CreateGameRule(RoomCreationOptions roomCreationOptions)
        {
            var type = GetGameRuleType(roomCreationOptions);
            return type != null
                ? (GameRuleBase)_serviceProvider.GetRequiredService(type)
                : null;
        }

        private Type GetGameRuleType(RoomCreationOptions roomCreationOptions)
        {
            if (!_gameRules.TryGetValue(roomCreationOptions.GameRule, out var entries))
                return null;

            foreach (var entry in entries.OrderByDescending(x => x.Priority))
            {
                var type = entry.Func(roomCreationOptions);
                if (type != null)
                    return type;
            }

            return null;
        }

        private class Entry
        {
            public int Priority { get; }
            public Func<RoomCreationOptions, Type> Func { get; }

            public Entry(int priority, Func<RoomCreationOptions, Type> func)
            {
                Priority = priority;
                Func = func;
            }
        }
    }
}
