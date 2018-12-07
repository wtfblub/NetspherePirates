namespace Netsphere.Common.Messaging
{
    public class ChatLoginRequest : MessageWithGuid
    {
        public ulong AccountId { get; set; }
        public string SessionId { get; set; }

        public ChatLoginRequest()
        {
        }

        public ChatLoginRequest(ulong accountId, string sessionId)
        {
            AccountId = accountId;
            SessionId = sessionId;
        }
    }

    public class ChatLoginResponse : MessageWithGuid
    {
        public bool OK { get; set; }
        public Account Account { get; set; }
        public uint TotalExperience { get; set; }

        public ChatLoginResponse()
        {
        }

        public ChatLoginResponse(bool ok, Account account, uint totalExperience)
        {
            OK = ok;
            Account = account;
            TotalExperience = totalExperience;
        }
    }
}
