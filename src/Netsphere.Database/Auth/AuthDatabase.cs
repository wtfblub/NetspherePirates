using Shaolinq;

namespace Netsphere.Database.Auth
{
    [DataAccessModel]
    public abstract class AuthDatabase : DataAccessModel
    {
        [DataAccessObjects]
        public abstract DataAccessObjects<AccountDto> Accounts { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<BanDto> Bans { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<LoginHistoryDto> LoginHistory { get; }

        [DataAccessObjects]
        public abstract DataAccessObjects<NicknameHistoryDto> NicknameHistory { get; }
    }
}