using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Server.Game;
using Netsphere.Server.Game.Services;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class PlayerController : WebApiController
    {
        private const string BasePath = "/players";

        private readonly PlayerManager _playerManager;

        public PlayerController(IHttpContext context, PlayerManager playerManager)
            : base(context)
        {
            _playerManager = playerManager;
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/{playerId}")]
        public Task<bool> GetPlayer(ulong playerId)
        {
            var plr = _playerManager[playerId];
            if (plr == null)
                return this.JsonResponseAsync(null, HttpStatusCode.NotFound);

            return this.JsonResponseAsync(plr.Map<Player, PlayerDto>());
        }

        [WebApiHandler(HttpVerbs.Get, BasePath)]
        public Task<bool> GetPlayers()
        {
            return this.JsonResponseAsync(_playerManager.Select(x => x.Map<Player, PlayerDto>()).ToArray());
        }
    }
}
