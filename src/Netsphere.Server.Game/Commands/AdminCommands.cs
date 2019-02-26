using System;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using BlubLib.Threading.Tasks;
using Logging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    public class AdminCommands : ICommandHandler
    {
        private readonly ILogger<AdminCommands> _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly GameDataService _gameDataService;
        private readonly PlayerManager _playerManager;
        private readonly object _shutdownMutex;
        private bool _isInShutdown;

        public AdminCommands(ILogger<AdminCommands> logger, IApplicationLifetime applicationLifetime,
            GameDataService gameDataService, PlayerManager playerManager)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _gameDataService = gameDataService;
            _playerManager = playerManager;
            _shutdownMutex = new object();
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: shutdown [minutes]"
        )]
        public async Task<bool> Shutdown(Player plr, string[] args)
        {
            lock (_shutdownMutex)
            {
                if (_isInShutdown)
                {
                    if (plr != null)
                        plr.SendConsoleMessage("Server is already in shutdown mode");
                    else
                        Console.WriteLine("Server is already in shutdown mode");

                    return true;
                }

                if (args.Length == 0)
                {
                    _isInShutdown = true;
                    Shutdown();
                }
                else
                {
                    if (!uint.TryParse(args[0], out var minutes) || minutes <= 0)
                        return false;

                    _isInShutdown = true;
                    var timer = TimeSpan.FromMinutes(minutes);

                    var _ = Task.Run(async () =>
                    {
                        while (timer.TotalSeconds > 0)
                        {
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if (timer.TotalMinutes >= 1 && timer.TotalSeconds % 60 == 0)
                                Announce();
                            else if (timer.TotalSeconds == 30)
                                Announce();
                            else if (timer.TotalSeconds <= 15 && timer.TotalSeconds % 5 == 0)
                                Announce();
                            // ReSharper restore CompareOfFloatsByEqualityOperator

                            await Task.Delay(TimeSpan.FromSeconds(1)).AnyContext();
                            timer -= TimeSpan.FromSeconds(1);
                        }

                        Shutdown();

                        void Announce()
                        {
                            _playerManager.ForEach(x => x.SendNotice($"Server shutdown in {timer.ToHumanReadable()}"));
                            _logger.Information("Server shutdown in {Time}", timer);
                        }
                    });
                }
            }

            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: reloadshop"
        )]
        public async Task<bool> ReloadShop(Player plr, string[] args)
        {
            plr?.SendConsoleMessage("Reloading shop...");
            _playerManager.ForEach(x => x.SendNotice("Reloading shop - Game might lag for a minute"));
            await _gameDataService.LoadShop();
            _playerManager.ForEach(x => x.Session.Send(new SNewShopUpdateRequestAckMessage()));
            plr?.SendConsoleMessage("Reloaded!");
            return true;
        }

        private void Shutdown()
        {
            _playerManager.ForEach(x => x.SendNotice("Server is shutting down..."));
            _applicationLifetime.StopApplication();
        }
    }
}
