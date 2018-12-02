namespace Netsphere.Common
{
    public class Account
    {
        public ulong Id { get; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public SecurityLevel SecurityLevel { get; set; }

        public Account()
        {
        }

        public Account(ulong id, string username, string nickname, SecurityLevel securityLevel)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
            SecurityLevel = securityLevel;
        }
    }
}
