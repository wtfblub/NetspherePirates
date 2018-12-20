using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using LinqToDB;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Server.Chat
{
    public class Mailbox : ISaveable, IEnumerable<Mail>
    {
        public const int ItemsPerPage = 10;

        private ILogger _logger;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly PlayerManager _playerManager;
        private readonly ConcurrentDictionary<long, Mail> _mails;
        private readonly ConcurrentStack<Mail> _mailsToDelete;

        public Player Player { get; private set; }
        public int Count => _mails.Count;
        public Mail this[long id] => CollectionExtensions.GetValueOrDefault(_mails, id);

        public Mailbox(ILogger<Mailbox> logger, IDatabaseProvider databaseProvider, IdGeneratorService idGeneratorService,
            PlayerManager playerManager)
        {
            _logger = logger;
            _databaseProvider = databaseProvider;
            _idGeneratorService = idGeneratorService;
            _playerManager = playerManager;
            _mails = new ConcurrentDictionary<long, Mail>();
            _mailsToDelete = new ConcurrentStack<Mail>();
        }

        public async Task Initialize(Player player, PlayerEntity entity)
        {
            Player = player;
            _logger = Player.AddContextToLogger(_logger);

            foreach (var mailEntity in entity.Inbox.Where(mailDto => !mailDto.IsMailDeleted))
            {
                var senderNickname = "";

                var sender = _playerManager[(ulong)mailEntity.SenderPlayerId];
                if (sender != null)
                {
                    senderNickname = sender.Account.Nickname;
                }
                else
                {
                    using (var db = _databaseProvider.Open<AuthContext>())
                    {
                        var account = await db.Accounts.FirstOrDefaultAsync(x => x.Id == mailEntity.SenderPlayerId);
                        if (account == null)
                        {
                            _logger.Warning("Player={AccountId} has mail={MailId} with non-existant sender={SenderAccountId}",
                                mailEntity.PlayerId, mailEntity.Id, mailEntity.SenderPlayerId);
                            continue;
                        }

                        senderNickname = account.Nickname;
                    }
                }

                _mails[mailEntity.Id] = new Mail(mailEntity, senderNickname);
            }
        }

        public IEnumerable<Mail> GetMailsByPage(byte page)
        {
            return _mails.Values.OrderBy(mail => mail.SendDate.ToUnixTimeSeconds()).Skip(ItemsPerPage * (page - 1))
                .Take(ItemsPerPage);
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

            AccountEntity account;
            using (var db = _databaseProvider.Open<AuthContext>())
                account = await db.Accounts.FirstOrDefaultAsync(x => x.Nickname == receiver);

            if (account == null)
                return false;

            using (var db = _databaseProvider.Open<GameContext>())
            {
                var entity = new PlayerMailEntity
                {
                    Id = _idGeneratorService.GetNextId(IdKind.Mail),
                    PlayerId = account.Id,
                    SenderPlayerId = (int)Player.Account.Id,
                    SentDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Title = title,
                    Message = message,
                    IsMailNew = true,
                    IsMailDeleted = false
                };
                await db.InsertAsync(entity);

                var plr = _playerManager.GetByNickname(receiver);
                plr?.Mailbox.Add(new Mail(entity, receiver));
                return true;
            }
        }

        public bool Remove(IEnumerable<Mail> mails)
        {
            return Remove(mails.Select(x => x.Id));
        }

        public bool Remove(IEnumerable<long> mailIds)
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
            return Player.Session.SendAsync(new SNoteReminderInfoAckMessage((byte)this.Count(x => x.IsNew), 0, 0));
        }

        public async Task Save(GameContext db)
        {
            if (!_mailsToDelete.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_mailsToDelete.TryPop(out var mailToDelete))
                    idsToRemove.Add(mailToDelete.Id);

                await db.PlayerMails.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var mail in _mails.Values.Where(x => x.IsDirty))
            {
                await db.PlayerMails
                    .Where(x => x.Id == mail.Id)
                    .Set(x => x.IsMailNew, mail.IsNew)
                    .UpdateAsync();

                mail.SetDirtyState(false);
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

    public class Mail : DatabaseObject
    {
        private bool _isNew;

        public long Id { get; }
        public string Sender { get; }
        public ulong SenderId { get; }

        public DateTimeOffset SendDate { get; }
        public DateTimeOffset Expires => SendDate.AddDays(30); // ToDo use config

        public string Title { get; }
        public string Message { get; }
        public bool IsNew
        {
            get => _isNew;
            internal set => SetIfChanged(ref _isNew, value);
        }

        public Mail(PlayerMailEntity entity, string senderNickname)
        {
            SetExistsState(true);
            Id = entity.Id;
            SenderId = (ulong)entity.SenderPlayerId;
            Sender = senderNickname;
            SendDate = DateTimeOffset.FromUnixTimeSeconds(entity.SentDate);
            Title = entity.Title;
            Message = entity.Message;
            _isNew = entity.IsMailNew;
        }
    }
}
