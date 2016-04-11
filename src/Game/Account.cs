using Netsphere.Database.Auth;

namespace Netsphere
{
    internal class Account
    {
        public ulong Id { get; private set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public SecurityLevel SecurityLevel { get; set; }

        public Account(AccountDto dto)
        {
            Id = (ulong)dto.Id;
            Username = dto.Username;
            Nickname = dto.Nickname;
            SecurityLevel = (SecurityLevel)dto.SecurityLevel;
        }
    }
}
