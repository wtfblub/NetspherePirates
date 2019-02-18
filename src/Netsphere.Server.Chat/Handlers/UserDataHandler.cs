using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class UserDataHandler : IHandle<CSetUserDataReqMessage>, IHandle<CGetUserDataReqMessage>
    {
        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, CSetUserDataReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (message.UserData.ChannelId > 0 && !plr.SentPlayerList && plr.Channel != null)
            {
                // We can't send the channel player list in Channel.Join because the client only accepts it here :/
                plr.SentPlayerList = true;
                var data = plr.Channel.Players.Values.Select(p => p.Map<Player, UserDataWithNickDto>()).ToArray();
                session.Send(new SChannelPlayerListAckMessage(data));
            }

            // Save settings if any of them changed
            var settings = plr.Settings;
            var name = nameof(UserDataDto.AllowCombiInvite);
            if (!settings.Contains(name) || settings.Get<CommunitySetting>(name) != message.UserData.AllowCombiInvite)
                settings.AddOrUpdate(name, message.UserData.AllowCombiInvite);

            name = nameof(UserDataDto.AllowFriendRequest);
            if (!settings.Contains(name) || settings.Get<CommunitySetting>(name) != message.UserData.AllowFriendRequest)
                settings.AddOrUpdate(name, message.UserData.AllowFriendRequest);

            name = nameof(UserDataDto.AllowRoomInvite);
            if (!settings.Contains(name) || settings.Get<CommunitySetting>(name) != message.UserData.AllowRoomInvite)
                settings.AddOrUpdate(name, message.UserData.AllowRoomInvite);

            name = nameof(UserDataDto.AllowInfoRequest);
            if (!settings.Contains(name) || settings.Get<CommunitySetting>(name) != message.UserData.AllowInfoRequest)
                settings.AddOrUpdate(name, message.UserData.AllowInfoRequest);

            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        public async Task<bool> OnHandle(MessageContext context, CGetUserDataReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (plr.Account.Id == message.AccountId)
            {
                session.Send(new SUserDataAckMessage(plr.Map<Player, UserDataDto>()));
                return true;
            }

            if (!plr.Channel.Players.TryGetValue(message.AccountId, out var target))
                return true;

            switch (target.Settings.Get<CommunitySetting>(nameof(UserDataDto.AllowInfoRequest)))
            {
                case CommunitySetting.Deny:
                    // Not sure if there is an answer to this
                    return true;

                case CommunitySetting.FriendOnly:
                    // TODO
                    return true;
            }

            session.Send(new SUserDataAckMessage(target.Map<Player, UserDataDto>()));
            return true;
        }
    }
}
