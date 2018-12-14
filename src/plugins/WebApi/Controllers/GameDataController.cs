using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Common;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class GameDataController : WebApiController
    {
        private const string BasePath = "/gamedata";

        private readonly GameDataService _gameDataService;

        public GameDataController(IHttpContext context, GameDataService gameDataService)
            : base(context)
        {
            _gameDataService = gameDataService;
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/maps")]
        public Task<bool> GetMaps()
        {
            return this.JsonResponseAsync(_gameDataService.Maps.Select(x => x.Map<MapInfo, MapDto>()).ToArray());
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/maps/{mapId}")]
        public Task<bool> GetMap(int mapId)
        {
            var map = _gameDataService.Maps.FirstOrDefault(x => x.Id == mapId);
            return map == null
                ? this.JsonResponseAsync(null, HttpStatusCode.NotFound)
                : this.JsonResponseAsync(map.Map<MapInfo, MapDto>());
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/items/{itemId}")]
        public Task<bool> GetItem(uint itemId)
        {
            var item = _gameDataService.Items[itemId];
            return item == null
                ? this.JsonResponseAsync(null, HttpStatusCode.NotFound)
                : this.JsonResponseAsync(item.Map<ItemInfo, ItemDto>());
        }
    }
}
