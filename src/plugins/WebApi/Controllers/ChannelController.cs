using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Server.Game;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace WebApi.Controllers
{
    public class ChannelController : WebApiController
    {
        private const string BasePath = "/channels";

        private readonly ChannelService _channelService;

        public ChannelController(IHttpContext context, ChannelService channelService)
            : base(context)
        {
            _channelService = channelService;
        }

        [WebApiHandler(HttpVerbs.Get, BasePath + "/{channelId}")]
        public Task<bool> GetChannel(uint channelId)
        {
            var channel = _channelService[channelId];
            return channel == null
                ? this.JsonResponseAsync(null, HttpStatusCode.NotFound)
                : this.JsonResponseAsync(channel.Map<Channel, Models.ChannelDto>());
        }

        [WebApiHandler(HttpVerbs.Get, BasePath)]
        public Task<bool> GetChannel()
        {
            return this.JsonResponseAsync(_channelService.Select(x => x.Map<Channel, Models.ChannelDto>()).ToArray());
        }
    }
}
