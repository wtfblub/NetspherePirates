using LinqToDB;
using LinqToDB.Data;
using Netsphere.Database.Auth;

namespace Netsphere.Database
{
    public class AuthContext : DataConnection
    {
        public ITable<AccountEntity> Accounts => GetTable<AccountEntity>();
        public ITable<BanEntity> Bans => GetTable<BanEntity>();
        public ITable<NicknameHistoryEntity> Nicknames => GetTable<NicknameHistoryEntity>();

        public AuthContext(string provider, string connection)
            : base(provider, connection)
        {
        }
    }
}
