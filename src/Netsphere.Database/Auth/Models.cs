using Shaolinq;

namespace Netsphere.Database.Auth
{
    [DataAccessObject("accounts")]
    public abstract class AccountDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [PersistedMember]
        public abstract string Username { get; set; }

        [PersistedMember]
        public abstract string Nickname { get; set; }

        [PersistedMember]
        public abstract string Password { get; set; }

        [PersistedMember]
        public abstract string Salt { get; set; }

        [PersistedMember]
        public abstract byte SecurityLevel { get; set; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<BanDto> Bans { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<LoginHistoryDto> LoginHistory { get; }

        [RelatedDataAccessObjects]
        public abstract RelatedDataAccessObjects<NicknameHistoryDto> NicknameHistory { get; }
    }

    [DataAccessObject("bans")]
    public abstract class BanDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract AccountDto Account { get; set; }

        [PersistedMember]
        public abstract long Date { get; set; }

        [PersistedMember]
        public abstract long Duration { get; set; }

        [PersistedMember]
        public abstract string Reason { get; set; }
    }

    [DataAccessObject("login_history")]
    public abstract class LoginHistoryDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract AccountDto Account { get; set; }

        [PersistedMember]
        public abstract long Date { get; set; }

        [PersistedMember]
        public abstract string IP { get; set; }
    }

    [DataAccessObject("nickname_history")]
    public abstract class NicknameHistoryDto : DataAccessObject
    {
        [PrimaryKey]
        [AutoIncrement]
        [PersistedMember]
        public virtual int Id { get; set; }

        [BackReference]
        public abstract AccountDto Account { get; set; }

        [PersistedMember]
        public abstract string Nickname { get; set; }

        [PersistedMember]
        public abstract long ExpireDate { get; set; }
    }
}
