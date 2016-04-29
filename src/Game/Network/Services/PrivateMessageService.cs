using System.Linq;
using System.Threading.Tasks;
using BlubLib.Network.Pipes;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;

namespace Netsphere.Network.Services
{
    internal class PrivateMessageService : MessageHandler
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CNoteListReqMessage))]
        public void CNoteListReq(ChatSession session, CNoteListReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message("Page:{0} MessageType:{1}", message.Page, message.MessageType)
                .Write();

            var mailbox = session.Player.Mailbox;
            var maxPages = mailbox.Count / Mailbox.ItemsPerPage + 1;

            if (message.Page > maxPages)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Page {0} does not exist", message.Page)
                    .Write();
                return;
            }

            var mails = session.Player.Mailbox.GetMailsByPage(message.Page);
            session.Send(new SNoteListAckMessage(maxPages, message.Page, mails.Select(mail => mail.Map<Mail, NoteDto>()).ToArray()));
        }

        [MessageHandler(typeof(CReadNoteReqMessage))]
        public async Task CReadNoteReq(ChatSession session, CReadNoteReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message("Id:{0}", message.Id)
                .Write();

            var mail = session.Player.Mailbox[message.Id];
            if (mail == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Mail {0} not found", message.Id)
                    .Write();

                await session.SendAsync(new SReadNoteAckMessage(0, new NoteContentDto(), 1))
                    .ConfigureAwait(false);
                return;
            }

            mail.IsNew = false;
            await session.Player.Mailbox.UpdateReminderAsync()
                .ConfigureAwait(false);

            await session.SendAsync(new SReadNoteAckMessage(mail.Id, mail.Map<Mail, NoteContentDto>(), 0))
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CDeleteNoteReqMessage))]
        public async Task CDeleteNoteReq(ChatSession session, CDeleteNoteReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message("Ids:{0}", string.Join(",", message.Notes))
                .Write();

            session.Player.Mailbox.Remove(message.Notes);
            await session.SendAsync(new SDeleteNoteAckMessage())
                .ConfigureAwait(false);
        }

        [MessageHandler(typeof(CSendNoteReqMessage))]
        public async Task CSendNoteReq(ChatSession session, CSendNoteReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message("{0}", JsonConvert.SerializeObject(message, Formatting.Indented))
                .Write();

            // ToDo use config file
            if (message.Title.Length > 100)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Title is too big({0})", message.Title.Length)
                    .Write();
                return;
            }

            if (message.Message.Length > 112)
            {
                Logger.Error()
                    .Account(session)
                    .Message("Message is too big({0})", message.Message.Length)
                    .Write();
                return;
            }

            var result = await session.Player.Mailbox.SendAsync(message.Receiver, message.Title, message.Message)
                        .ConfigureAwait(false);
            await session.SendAsync(new SSendNoteAckMessage(result ? 0 : 1)).ConfigureAwait(false);
        }
    }
}
