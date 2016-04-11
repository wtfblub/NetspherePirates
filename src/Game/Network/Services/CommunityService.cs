using System.Linq;
using BlubLib.Network.Pipes;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Network.Services
{
    internal class CommunityService : Service
    {
        [MessageHandler(typeof(CSetUserDataReqMessage))]
        public void SetUserDataHandler(ChatSession session, CSetUserDataReqMessage message)
        {
            var plr = session.Player;

            if (message.UserData.ChannelId > 0 && !plr.SentPlayerList && plr.Channel != null)
            {
                plr.SentPlayerList = true;
                var data = plr.Channel.Players.Values.Select(p => p.Map<Player, UserDataWithNickDto>()).ToArray();
                session.Send(new SChannelPlayerListAckMessage(data));
            }

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
        }

        [MessageHandler(typeof(CGetUserDataReqMessage))]
        public void GetUserDataHandler(ChatSession session, CGetUserDataReqMessage message)
        {
            var plr = session.Player;

            if (plr.Account.Id == message.AccountId)
            {
                session.Send(new SUserDataAckMessage(plr.Map<Player, UserDataDto>()));
                return;
            }

            Player target;
            if (plr.Channel.Players.TryGetValue(message.AccountId, out target))
            {
                switch (target.Settings.Get<CommunitySetting>(nameof(UserDataDto.AllowInfoRequest)))
                {
                    case CommunitySetting.Deny:
                        // Not sure if there is an answer to this
                        return;

                    case CommunitySetting.FriendOnly:
                        // ToDo
                        return;
                }

                session.Send(new SUserDataAckMessage(target.Map<Player, UserDataDto>()));
            }
        }

        [MessageHandler(typeof(CDenyChatReqMessage))]
        public void DenyHandler(ChatServer service, ChatSession session, CDenyChatReqMessage message)
        {
            var server = service.GameServer;
            var plr = session.Player;

            if (message.Deny.AccountId == plr.Account.Id)
                return;

            Deny deny;
            switch (message.Action)
            {
                case DenyAction.Add:
                    if (plr.DenyManager.Contains(message.Deny.AccountId))
                        return;
                    var target = server.PlayerManager[message.Deny.AccountId];
                    if (target == null)
                        return;

                    deny = plr.DenyManager.Add(target);
                    session.Send(new SDenyChatAckMessage(0, DenyAction.Add, deny.Map<Deny, DenyDto>()));
                    break;

                case DenyAction.Remove:
                    deny = plr.DenyManager[message.Deny.AccountId];
                    if (deny == null)
                        return;

                    plr.DenyManager.Remove(deny);
                    session.Send(new SDenyChatAckMessage(0, DenyAction.Remove, deny.Map<Deny, DenyDto>()));
                    break;
            }
        }
    }
}
