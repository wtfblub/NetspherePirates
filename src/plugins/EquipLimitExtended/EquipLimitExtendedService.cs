using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Server.Game;

namespace EquipLimitExtended
{
    public class EquipLimitExtendedService : IHostedService
    {
        private readonly IOptionsMonitor<EquipLimitExtendedOptions> _options;

        public EquipLimitExtendedService(IOptionsMonitor<EquipLimitExtendedOptions> options)
        {
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            EquipValidator.IsValidHook += IsEquipValidHook;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool IsEquipValidHook(EquipValidatorHookEventArgs e)
        {
            var options = _options.CurrentValue;
            foreach (var rule in options.Rules)
            {
                // If none of these are set the rule applies to ALL rooms

                // Check keyword if set
                if (!string.IsNullOrWhiteSpace(rule.Keyword)
                    && !e.Room.Options.Name.Contains(rule.Keyword, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Check gamerule if set
                if (rule.GameRules?.Length > 0 &&
                    !rule.GameRules.Contains(e.Room.GameRule.GameRule))
                {
                    continue;
                }

                // Check equip limit if set
                if (rule.EquipLimits?.Length > 0 &&
                    !rule.EquipLimits.Contains(e.Room.Options.EquipLimit))
                {
                    continue;
                }

                // ReSharper disable once InvokeAsExtensionMethod
                var items = Enumerable.Concat(
                    e.Character.Weapons.GetItems().Where(x => x != null),
                    e.Character.Skills.GetItems().Where(x => x != null)
                );

                switch (rule.Mode)
                {
                    // ONLY items in the config are allowed
                    case EquipLimitRuleMode.Whitelist:
                        if (items.Any(item => !rule.Items.Contains(item.ItemNumber)))
                        {
                            e.Result = false;
                            return false;
                        }

                        break;

                    // All items in the config are NOT allowed
                    case EquipLimitRuleMode.Blacklist:
                        if (items.Any(item => rule.Items.Contains(item.ItemNumber)))
                        {
                            e.Result = false;
                            return false;
                        }

                        break;
                }
            }

            return true;
        }
    }
}
