using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using ProudNet.Handlers;
using Serilog;
using Serilog.Core;

namespace Netsphere.Network.Services
{
    internal class PrivateMessageService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(PrivateMessageService));

        [MessageHandler(typeof(CNoteListReqMessage))]
        public void CNoteListReq(ChatSession session, CNoteListReqMessage message)
        {
            Logger.ForAccount(session)
                .Debug("Note list Page:{page} MessageType:{messageType}", message.Page, message.MessageType);

            var mailbox = session.Player.Mailbox;
            var maxPages = mailbox.Count / Mailbox.ItemsPerPage + 1;

            if (message.Page > maxPages)
            {
                Logger.ForAccount(session)
                    .Error("Page {page} does not exist", message.Page);
                return;
            }

            var mails = session.Player.Mailbox.GetMailsByPage(message.Page);
            session.SendAsync(new SNoteListAckMessage(maxPages, message.Page, mails.Select(mail => mail.Map<Mail, NoteDto>()).ToArray()));
        }

        [MessageHandler(typeof(CReadNoteReqMessage))]
        public void CReadNoteReq(ChatSession session, CReadNoteReqMessage message)
        {
            Logger.ForAccount(session)
                .Debug("Read note {id}", message.Id);

            var mail = session.Player.Mailbox[message.Id];
            if (mail == null)
            {
                Logger.ForAccount(session)
                    .Error("Mail {id} not found", message.Id);

                session.SendAsync(new SReadNoteAckMessage(0, new NoteContentDto(), 1));
                return;
            }

            mail.IsNew = false;
            session.Player.Mailbox.UpdateReminderAsync();
            session.SendAsync(new SReadNoteAckMessage(mail.Id, mail.Map<Mail, NoteContentDto>(), 0));
        }

        [MessageHandler(typeof(CDeleteNoteReqMessage))]
        public void CDeleteNoteReq(ChatSession session, CDeleteNoteReqMessage message)
        {
            Logger.ForAccount(session)
                .Debug("Delete note Ids:{id}", string.Join(",", message.Notes));

            session.Player.Mailbox.Remove(message.Notes);
            session.SendAsync(new SDeleteNoteAckMessage());
        }

        [MessageHandler(typeof(CSendNoteReqMessage))]
        public async Task CSendNoteReq(ChatSession session, CSendNoteReqMessage message)
        {
            Logger.ForAccount(session)
                .Debug("Send note {message}", message);

            // ToDo use config file
            if (message.Title.Length > 100)
            {
                Logger.ForAccount(session)
                    .Error("Title is too big({length})", message.Title.Length);
                return;
            }

            if (message.Message.Length > 112)
            {
                Logger.ForAccount(session)
                    .Error("Message is too big({length})", message.Message.Length);
                return;
            }

            var result = await session.Player.Mailbox.SendAsync(message.Receiver, message.Title, message.Message);
            session.SendAsync(new SSendNoteAckMessage(result ? 0 : 1));
        }
    }
}
