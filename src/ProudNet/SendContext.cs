namespace ProudNet
{
    internal class SendContext
    {
        public object Message { get; set; }
        public SendOptions SendOptions { get; set; }

        public SendContext()
        { }

        public SendContext(object message, SendOptions sendOptions)
        {
            Message = message;
            SendOptions = sendOptions;
        }
    }
}
