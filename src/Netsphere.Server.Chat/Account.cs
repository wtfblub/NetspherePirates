using Netsphere.Database.Auth;

namespace Netsphere.Server.Chat
{
    public class Account
    {
        public ulong Id { get; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public SecurityLevel SecurityLevel { get; set; }

        public Account(AccountEntity entity)
        {
            Id = (ulong)entity.Id;
            Username = entity.Username;
            Nickname = entity.Nickname;
            SecurityLevel = (SecurityLevel)entity.SecurityLevel;
        }
    }
}
