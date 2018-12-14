using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Netsphere.Server.Game;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class StatisticsController : WebApiController
    {
        private readonly PlayerManager _playerManager;

        public StatisticsController(IHttpContext context, PlayerManager playerManager)
            : base(context)
        {
            _playerManager = playerManager;
        }

        [WebApiHandler(HttpVerbs.Get, "/statistics")]
        public Task<bool> GetStatistics()
        {
            var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
            return this.JsonResponseAsync(new StatisticsDto(
                (long)uptime.TotalSeconds,
                _playerManager.Count));
        }
    }
}
