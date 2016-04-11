using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Chat;
using Shaolinq;

namespace Netsphere
{
    internal class Mailbox : IEnumerable<Mail>
    {
        public const int ItemsPerPage = 10;
        private readonly ConcurrentDictionary<ulong, Mail> _mails = new ConcurrentDictionary<ulong, Mail>();
        private readonly ConcurrentStack<Mail> _mailsToDelete = new ConcurrentStack<Mail>();

        public Player Player { get; }
        public int Count => _mails.Count;
        public Mail this[ulong id] => _mails.GetValueOrDefault(id);

        public Mailbox(Player player, PlayerDto dto)
        {
            Player = player;

            foreach (var mailDto in dto.Inbox.Where(mailDto => !mailDto.IsMailDeleted))
                _mails.TryAdd((ulong)mailDto.Id, new Mail(mailDto));
        }

        public IEnumerable<Mail> GetMailsByPage(byte page)
        {
            return _mails.Values.OrderBy(mail => mail.SendDate.ToUnixTimeSeconds()).Skip(ItemsPerPage * (page - 1)).Take(ItemsPerPage);
        }

        internal void Add(Mail mail)
        {
            if (_mails.TryAdd(mail.Id, mail))
                UpdateReminder();
        }

        public async Task<bool> SendAsync(string receiver, string title, string message)
        {
            // ToDo consume pen
            // ToDo check for ignore

            var accDto = await AuthDatabase.Instance.Accounts
                .FirstOrDefaultAsync(acc => acc.Nickname == receiver)
                .ConfigureAwait(false);
            if (accDto == null)
                return false;

            using (var scope = new DataAccessScope())
            {
                var mailDto = GameDatabase.Instance.Players
                    .GetReference(accDto.Id)
                    .Inbox.Create();
                mailDto.SenderPlayer = GameDatabase.Instance.Players.GetReference((int)Player.Account.Id);
                mailDto.SentDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                mailDto.Title = title;
                mailDto.Message = message;
                mailDto.IsMailNew = true;
                mailDto.IsMailDeleted = false;

                await scope.CompleteAsync()
                    .ConfigureAwait(false);

                var plr = GameServer.Instance.PlayerManager.Get(receiver);
                plr?.Mailbox.Add(new Mail(mailDto));
                return true;
            }
        }

        public bool Remove(IEnumerable<Mail> mails)
        {
            return Remove(mails.Select(mail => mail.Id));
        }

        public bool Remove(IEnumerable<ulong> mailIds)
        {
            var changed = false;

            foreach (var mail in mailIds.Select(id => this[id]).Where(mail => mail != null))
            {
                changed = true;

                _mails.Remove(mail.Id);
                _mailsToDelete.Push(mail);
            }
            UpdateReminder();

            return changed;
        }

        public void UpdateReminder()
        {
            Player.ChatSession.Send(new SNoteReminderInfoAckMessage((byte)this.Count(m => m.IsNew), 0, 0));
        }

        public Task UpdateReminderAsync()
        {
            return Player.ChatSession.SendAsync(new SNoteReminderInfoAckMessage((byte)this.Count(m => m.IsNew), 0, 0));
        }

        internal void Save()
        {
            Mail mailToDelete;
            while (_mailsToDelete.TryPop(out mailToDelete))
                GameDatabase.Instance.PlayerMails.GetReference(mailToDelete.Id).IsMailDeleted = true;

            foreach (var mail in _mails.Values.Where(mail => mail.NeedsToSave))
            {
                GameDatabase.Instance.PlayerMails
                    .GetReference(mail.Id)
                    .IsMailNew = mail.IsNew;
            }
        }

        public IEnumerator<Mail> GetEnumerator()
        {
            return _mails.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class Mail
    {
        private bool _isNew;

        internal bool NeedsToSave { get; set; }

        public ulong Id { get; }
        public string Sender { get; }
        public ulong SenderId { get; }

        public DateTimeOffset SendDate { get; }
        public DateTimeOffset Expires => SendDate.AddDays(30); // ToDo use config

        public string Title { get; }
        public string Message { get; }

        public bool IsNew
        {
            get { return _isNew; }
            internal set
            {
                if (_isNew == value)
                    return;
                _isNew = value;
                NeedsToSave = true;
            }
        }

        internal Mail(PlayerMailDto mailDto)
        {
            Id = (ulong)mailDto.Id;
            SenderId = (ulong)mailDto.SenderPlayer.Id;
            Sender = GetNickname(SenderId);
            SendDate = DateTimeOffset.FromUnixTimeSeconds(mailDto.SentDate);
            Title = mailDto.Title;
            Message = mailDto.Message;
            IsNew = mailDto.IsMailNew;
        }

        private static string GetNickname(ulong id)
        {
            // fast path
            var plr = GameServer.Instance.PlayerManager[id];
            if (plr != null)
                return plr.Account.Nickname;

            // if player is not online use a database lookup
            return AuthDatabase.Instance.Accounts.FirstOrDefault(acc => acc.Id == (int)id)?.Nickname ?? "";
        }
    }
}
