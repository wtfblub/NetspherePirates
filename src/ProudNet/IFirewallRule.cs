using System.Threading.Tasks;

namespace ProudNet
{
    /// <summary>
    /// Implemetns a firewall rule that controlls which messages are allowed to be processed
    /// </summary>
    public interface IFirewallRule
    {
        Task<bool> IsMessageAllowed(MessageContext context, object message);
    }
}
