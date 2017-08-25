using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Dapper.FastCrud;
using Netsphere.Database.Auth;
using Netsphere.Database.Game;
using Netsphere.Network;
using Netsphere.Network.Message.Chat;

namespace Netsphere
{
    internal class Mailbox : IEnumerable<Mail>
    {
        public const int ItemsPerPage = 10;
        private readonly ConcurrentDictionary<ulong, Mail> _mails = new ConcurrentDictionary<ulong, Mail>();
        private readonly ConcurrentStack<Mail> _mailsToDelete = new ConcurrentStack<Mail>();

        public Player Player { get; }
        public int Count => _mails.Count;
        public Mail this[ulong id] => CollectionExtensions.GetValueOrDefault(_mails, id);

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
                UpdateReminderAsync();
        }

        public async Task<bool> SendAsync(string receiver, string title, string message)
        {
            // ToDo consume pen
            // ToDo check for ignore

            AccountDto account;
            using (var db = AuthDatabase.Open())
            {
                account = (await db.FindAsync<AccountDto>(statement => statement
                           .Where($"{nameof(AccountDto.Nickname):C} = @{nameof(receiver)}")
                           .WithParameters(new { receiver })))
                    .FirstOrDefault();
            }
            if (account == null)
                return false;

            using (var db = GameDatabase.Open())
            {
                var mailDto = new PlayerMailDto
                {
                    PlayerId = account.Id,
                    SenderPlayerId = (int)Player.Account.Id,
                    SentDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Title = title,
                    Message = message,
                    IsMailNew = true,
                    IsMailDeleted = false
                };
                await db.InsertAsync(mailDto);

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
            UpdateReminderAsync();

            return changed;
        }

        public Task UpdateReminderAsync()
        {
            return Player.ChatSession.SendAsync(new NoteCountAckMessage((byte)this.Count(m => m.IsNew), 0, 0));
        }

        internal void Save(IDbConnection db)
        {
            var deleteMapping = OrmConfiguration
                .GetDefaultEntityMapping<PlayerMailDto>()
                .Clone()
                .UpdatePropertiesExcluding(prop => prop.IsExcludedFromUpdates = true,
                    nameof(PlayerMailDto.IsMailDeleted));

            if (!_mailsToDelete.IsEmpty)
            {
                var idsToRemove = new StringBuilder();
                var firstRun = true;
                Mail mailToDelete;
                while (_mailsToDelete.TryPop(out mailToDelete))
                {
                    if (firstRun)
                        firstRun = false;
                    else
                        idsToRemove.Append(',');
                    idsToRemove.Append(mailToDelete.Id);
                }
                db.BulkUpdate(new PlayerMailDto { IsMailDeleted = true }, statement => statement
                      .Where($"{nameof(PlayerMailDto.Id):C} IN ({idsToRemove})")
                      .WithEntityMappingOverride(deleteMapping));
            }

            var isNewMapping = OrmConfiguration
                .GetDefaultEntityMapping<PlayerMailDto>()
                .Clone()
                .UpdatePropertiesExcluding(prop => prop.IsExcludedFromUpdates = true,
                    nameof(PlayerMailDto.IsMailNew));

            var needsSave = _mails.Values.Where(mail => mail.NeedsToSave).ToArray();
            var isNew = needsSave.Where(mail => mail.IsNew);
            var isNotNew = needsSave.Where(mail => !mail.IsNew);

            if (isNew.Any())
            {
                db.BulkUpdate(new PlayerMailDto { IsMailNew = true }, statement => statement
                    .Where($"{nameof(PlayerMailDto.Id):C} IN ({string.Join(",", isNew.Select(x => x.Id))})")
                    .WithEntityMappingOverride(isNewMapping));

                foreach (var mail in isNew)
                    mail.NeedsToSave = false;
            }

            if (isNotNew.Any())
            {
                db.BulkUpdate(new PlayerMailDto {IsMailNew = false}, statement => statement
                    .Where($"{nameof(PlayerMailDto.Id):C} IN ({string.Join(",", isNotNew.Select(x => x.Id))})")
                    .WithEntityMappingOverride(isNewMapping));

                foreach (var mail in isNotNew)
                    mail.NeedsToSave = false;
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
            SenderId = (ulong)mailDto.SenderPlayerId;
            Sender = GetNickname(SenderId);
            SendDate = DateTimeOffset.FromUnixTimeSeconds(mailDto.SentDate);
            Title = mailDto.Title;
            Message = mailDto.Message;
            _isNew = mailDto.IsMailNew;
        }

        private static string GetNickname(ulong id)
        {
            // fast path
            var plr = GameServer.Instance.PlayerManager[id];
            if (plr != null)
                return plr.Account.Nickname;

            // if player is not online use a database lookup
            using (var db = AuthDatabase.Open())
                return db.Get(new AccountDto { Id = (int)id })?.Nickname ?? "";
        }
    }
}
