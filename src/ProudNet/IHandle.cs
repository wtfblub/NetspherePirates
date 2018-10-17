using System.Threading.Tasks;

namespace ProudNet
{
    public interface IHandle<in TMessage>
    {
        /// <summary>
        /// Handles a incoming message
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>Returns true when the next handler should be called
        /// and false when it should stop executing handlers</returns>
        Task<bool> OnHandle(MessageContext context, TMessage message);
    }
}
