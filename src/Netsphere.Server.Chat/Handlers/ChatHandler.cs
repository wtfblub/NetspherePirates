using System.Threading.Tasks;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class ChatHandler : IHandle<MessageChatReqMessage>, IHandle<MessageWhisperChatReqMessage>
    {
        private readonly PlayerManager _playerManager;

        public ChatHandler(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, MessageChatReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            switch (message.ChatType)
            {
                case ChatType.Channel:
                    plr.Channel.SendChatMessage(session.Player, message.Message);
                    break;

                case ChatType.Club:
                    // ToDo Change this when clans are implemented
                    session.Send(new MessageChatAckMessage(ChatType.Club,
                        plr.Account.Id, plr.Account.Nickname, message.Message));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, MessageWhisperChatReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var toPlr = _playerManager.GetByNickname(message.ToNickname);

            // TODO Is there an answer for this case?
            if (toPlr == null)
            {
                session.Send(new MessageChatAckMessage(ChatType.Channel, session.Player.Account.Id,
                    "SYSTEM", $"{message.ToNickname} is not online"));
                return true;
            }

            // TODO Is there an answer for this case?
            if (toPlr.Ignore.Contains(session.Player.Account.Id))
            {
                session.Send(new MessageChatAckMessage(ChatType.Channel, session.Player.Account.Id,
                    "SYSTEM", $"{message.ToNickname} is ignoring you"));
                return true;
            }

            toPlr.Session.Send(
                new MessageWhisperChatAckMessage(
                    0,
                    toPlr.Account.Nickname,
                    plr.Account.Id,
                    plr.Account.Nickname,
                    message.Message
                )
            );
            return true;
        }
    }
}
