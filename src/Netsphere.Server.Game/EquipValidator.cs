using System.Collections.Immutable;
using System.Linq;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    /// <summary>
    /// Checks if the equipment is valid for the current rules
    /// </summary>
    public class EquipValidator
    {
        private static readonly EventPipeline<EquipValidatorHookEventArgs> s_isValidHook =
            new EventPipeline<EquipValidatorHookEventArgs>();

        private readonly GameDataService _gameDataService;

        public static event EventPipeline<EquipValidatorHookEventArgs>.SubscriberDelegate IsValidHook
        {
            add => s_isValidHook.Subscribe(value);
            remove => s_isValidHook.Unsubscribe(value);
        }

        public EquipValidator(GameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        public bool IsValid(Character character)
        {
            var plr = character?.CharacterManager.Player;
            if (plr.Room == null)
                return false;

            var eventArgs = new EquipValidatorHookEventArgs(character);
            s_isValidHook.Invoke(eventArgs);
            if (eventArgs.Result.HasValue)
                return eventArgs.Result.Value;

            var equipLimitInfo = _gameDataService.EquipLimits.GetValueOrDefault(plr.Room.Options.EquipLimit);
            if (equipLimitInfo == null)
                return false;

            var items = character.Weapons.GetItems();

            // Need at least one weapon
            if (items.All(x => x == null))
                return false;

            // Check room equip limit
            if (items.Where(x => x != null).Any(x => equipLimitInfo.Blacklist.Contains(x.ItemNumber)))
                return false;

            items = character.Skills.GetItems();

            // Need a skill
            if (items.All(x => x == null))
                return false;

            // Check room equip limit
            if (items.Where(x => x != null).Any(x => equipLimitInfo.Blacklist.Contains(x.ItemNumber)))
                return false;

            return true;
        }
    }
}
