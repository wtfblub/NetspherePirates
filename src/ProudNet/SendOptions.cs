namespace ProudNet
{
    public class SendOptions
    {
        public static readonly SendOptions Reliable = new SendOptions();
        public static readonly SendOptions ReliableSecure = new SendOptions { Encrypt = true };
        public static readonly SendOptions ReliableCompress = new SendOptions { Compress = true };
        public static readonly SendOptions ReliableSecureCompress = new SendOptions { Encrypt = true, Compress = true };

        public bool Encrypt { get; set; }
        public bool Compress { get; set; }
    }
}
