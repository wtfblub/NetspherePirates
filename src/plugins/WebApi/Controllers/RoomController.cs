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
    public class RoomController : WebApiController
    {
        private const string BasePath = "/rooms/{channelId}";

        private readonly ChannelService _channelService;

        public RoomController(IHttpContext context, ChannelService channelService)
            : base(context)
        {
            _channelService = channelService;
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/{roomId}")]
        public Task<bool> GetRoom(uint channelId, uint roomId)
        {
            var channel = _channelService[channelId];
            if (channel == null)
                return this.JsonResponseAsync(null, HttpStatusCode.NotFound);

            var room = channel.RoomManager[roomId];
            return room == null
                ? this.JsonResponseAsync(null, HttpStatusCode.NotFound)
                : this.JsonResponseAsync(room.Map<Room, RoomDto>());
        }

        [WebApiHandler(HttpVerbs.Get, BasePath)]
        public Task<bool> GetRooms(uint channelId)
        {
            var channel = _channelService[channelId];
            return channel == null
                ? this.JsonResponseAsync(null, HttpStatusCode.NotFound)
                : this.JsonResponseAsync(channel.RoomManager.Select(x => x.Map<Room, RoomDto>()).ToArray());
        }
    }
}
