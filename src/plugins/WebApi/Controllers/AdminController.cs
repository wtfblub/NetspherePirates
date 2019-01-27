using System;
using System.Net;
using System.Threading.Tasks;
using Netsphere;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Server.Game;
using Netsphere.Server.Game.Services;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace WebApi.Controllers
{
    public class AdminController : WebApiController
    {
        private const string BasePath = "/admin";

        private readonly DatabaseService _databaseService;
        private readonly PlayerManager _playerManager;
        private readonly ChannelService _channelService;

        public AdminController(IHttpContext context, DatabaseService databaseService,
            PlayerManager playerManager, ChannelService channelService)
            : base(context)
        {
            _databaseService = databaseService;
            _playerManager = playerManager;
            _channelService = channelService;
        }

        [WebApiHandler(HttpVerbs.Post, BasePath + "/kick")]
        public Task<bool> Kick()
        {
            var body = this.RequestFormDataDictionary();
            if (!body.TryGetValue("playerId", out var playerIdStr))
                return this.JsonResponseAsync(new { error = "Invalid payload" }, HttpStatusCode.BadRequest);

            if (!ulong.TryParse(playerIdStr.ToString(), out var playerId))
                return this.JsonResponseAsync(new { error = "Invalid payload" }, HttpStatusCode.BadRequest);

            var plr = _playerManager[playerId];
            if (plr == null)
                return this.JsonResponseAsync(new { error = "Player not found" }, HttpStatusCode.NotFound);

            plr.Disconnect();
            return this.JsonResponseAsync(null);
        }

        [WebApiHandler(HttpVerbs.Post, BasePath + "/ban")]
        public async Task<bool> Ban()
        {
            var request = this.ParseJsonNet<BanRequestDto>();
            var plr = _playerManager[request.PlayerId];

            if (plr == null)
                return await this.JsonResponseAsync(new { error = "Player not found" }, HttpStatusCode.NotFound);

            using (var db = _databaseService.Open<AuthContext>())
            {
                db.Bans.Add(new BanEntity
                {
                    AccountId = (int)plr.Account.Id,
                    Date = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Duration = request.Duration,
                    Reason = request.Reason
                });

                await db.SaveChangesAsync();
            }

            plr.Disconnect();
            return await this.JsonResponseAsync(null);
        }

        [WebApiHandler(HttpVerbs.Post, BasePath + "/roomkick")]
        public Task<bool> RoomKick()
        {
            var request = this.ParseJsonNet<RoomKickRequestDto>();
            var plr = _playerManager[request.PlayerId];

            if (plr == null)
                return this.JsonResponseAsync(new { error = "Player not found" }, HttpStatusCode.NotFound);

            if (plr.Room == null)
                return this.JsonResponseAsync(new { error = "Player is not in a room" }, HttpStatusCode.NotFound);

            plr.Room.Leave(plr, request.Reason ?? RoomLeaveReason.ModeratorKick);
            return this.JsonResponseAsync(null);
        }

        [WebApiHandler(HttpVerbs.Post, BasePath + "/closeroom")]
        public Task<bool> CloseRoom()
        {
            var request = this.ParseJsonNet<CloseRoomRequestDto>();
            var channel = _channelService[request.ChannelId];

            if (channel == null)
                return this.JsonResponseAsync(new { error = "Channel not found" }, HttpStatusCode.NotFound);

            var room = channel.RoomManager[request.RoomId];

            if (room == null)
                return this.JsonResponseAsync(new { error = "Room not found" }, HttpStatusCode.NotFound);

            foreach (var plr in room.Players.Values)
                plr.Room.Leave(plr, request.Reason ?? RoomLeaveReason.ModeratorKick);

            return this.JsonResponseAsync(null);
        }

        public class BanRequestDto
        {
            public ulong PlayerId { get; set; }
            public long? Duration { get; set; }
            public string Reason { get; set; }
        }

        public class RoomKickRequestDto
        {
            public ulong PlayerId { get; set; }
            public RoomLeaveReason? Reason { get; set; }
        }

        public class CloseRoomRequestDto
        {
            public uint ChannelId { get; set; }
            public uint RoomId { get; set; }
            public RoomLeaveReason? Reason { get; set; }
        }
    }
}
