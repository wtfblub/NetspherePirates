using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Server.Game.GameRules;

namespace Netsphere.Server.Game
{
    public class GameRuleResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentStack<Func<RoomCreationOptions, Type>> _gameRules;

        public GameRuleResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gameRules = new ConcurrentStack<Func<RoomCreationOptions, Type>>();

            Register(x => x.MatchKey.GameRule == GameRule.Deathmatch ? typeof(Deathmatch) : null);
            Register(x => x.MatchKey.GameRule == GameRule.Touchdown ? typeof(Touchdown) : null);
            Register(x => x.MatchKey.GameRule == GameRule.BattleRoyal ? typeof(BattleRoyal) : null);
        }

        public void Register(Func<RoomCreationOptions, Type> gameRuleType)
        {
            _gameRules.Push(gameRuleType);
        }

        public bool HasGameRule(RoomCreationOptions roomCreationOptions)
        {
            return GetGameRuleType(roomCreationOptions) != null;
        }

        public GameRuleBase CreateGameRule(RoomCreationOptions roomCreationOptions)
        {
            var type = GetGameRuleType(roomCreationOptions);
            Console.WriteLine(type);
            return type != null
                ? (GameRuleBase)_serviceProvider.GetRequiredService(type)
                : null;
        }

        private Type GetGameRuleType(RoomCreationOptions roomCreationOptions)
        {
            foreach (var getGameRuleType in _gameRules)
            {
                var type = getGameRuleType(roomCreationOptions);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
