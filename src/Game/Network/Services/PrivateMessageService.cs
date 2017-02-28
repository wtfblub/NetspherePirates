using System.Linq;
using System.Threading.Tasks;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using ProudNet.Handlers;

namespace Netsphere.Network.Services
{
    internal class PrivateMessageService : ProudMessageHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [MessageHandler(typeof(CNoteListReqMessage))]
        public void CNoteListReq(ChatSession session, CNoteListReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message($"Page:{message.Page} MessageType:{message.MessageType}")
                .Write();

            var mailbox = session.Player.Mailbox;
            var maxPages = mailbox.Count / Mailbox.ItemsPerPage + 1;

            if (message.Page > maxPages)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Page {message.Page} does not exist")
                    .Write();
                return;
            }

            var mails = session.Player.Mailbox.GetMailsByPage(message.Page);
            session.SendAsync(new SNoteListAckMessage(maxPages, message.Page, mails.Select(mail => mail.Map<Mail, NoteDto>()).ToArray()));
        }

        [MessageHandler(typeof(CReadNoteReqMessage))]
        public void CReadNoteReq(ChatSession session, CReadNoteReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message($"Id:{message.Id}")
                .Write();

            var mail = session.Player.Mailbox[message.Id];
            if (mail == null)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Mail {message.Id} not found")
                    .Write();

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
            Logger.Debug()
                .Account(session)
                .Message($"Ids:{string.Join(",", message.Notes)}")
                .Write();

            session.Player.Mailbox.Remove(message.Notes);
            session.SendAsync(new SDeleteNoteAckMessage());
        }

        [MessageHandler(typeof(CSendNoteReqMessage))]
        public async Task CSendNoteReq(ChatSession session, CSendNoteReqMessage message)
        {
            Logger.Debug()
                .Account(session)
                .Message($"{JsonConvert.SerializeObject(message, Formatting.Indented)}")
                .Write();

            // ToDo use config file
            if (message.Title.Length > 100)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Title is too big({message.Title.Length})")
                    .Write();
                return;
            }

            if (message.Message.Length > 112)
            {
                Logger.Error()
                    .Account(session)
                    .Message($"Message is too big({message.Message.Length})")
                    .Write();
                return;
            }

            var result = await session.Player.Mailbox.SendAsync(message.Receiver, message.Title, message.Message);
            session.SendAsync(new SSendNoteAckMessage(result ? 0 : 1));
        }
    }
}
