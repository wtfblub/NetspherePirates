using System;
using BlubLib.DotNetty.Handlers.MessageHandling;
using ProudNet.Serialization;
using Serilog;

namespace ProudNet
{
    public class Configuration
    {
        public Guid Version { get; set; }
        public IHostIdFactory HostIdFactory { get; set; }
        public ISessionFactory SessionFactory { get; set; }
        public TimeSpan ConnectTimeout { get; set; }
        public MessageFactory[] MessageFactories { get; set; }
        public IMessageHandler[] MessageHandlers { get; set; }
        public ILogger Logger { get; set; }

        public bool EnableServerLog { get; set; }
        public FallbackMethod FallbackMethod { get; set; }
        public uint MessageMaxLength { get; set; }
        public TimeSpan IdleTimeout { get; set; }
        public DirectP2PStartCondition DirectP2PStartCondition { get; set; }
        public uint OverSendSuspectingThresholdInBytes { get; set; }
        public bool EnableNagleAlgorithm { get; set; }
        public int EncryptedMessageKeyLength { get; set; }
        public bool AllowServerAsP2PGroupMember { get; set; }
        public bool EnableP2PEncryptedMessaging { get; set; }
        public bool UpnpDetectNatDevice { get; set; }
        public bool UpnpTcpAddrPortMapping { get; set; }
        public bool EnableLookaheadP2PSend { get; set; }
        public bool EnablePingTest { get; set; }
        public uint EmergencyLogLineCount { get; set; }

        public Configuration()
        {
            Version = Guid.Empty;
            HostIdFactory = new HostIdFactory();
            SessionFactory = new ProudSessionFactory();
            ConnectTimeout = TimeSpan.FromSeconds(10);

            EnableServerLog = false;
            FallbackMethod = FallbackMethod.None;
            MessageMaxLength = 65000;
            IdleTimeout = TimeSpan.FromMilliseconds(900);
            DirectP2PStartCondition = DirectP2PStartCondition.Jit;
            OverSendSuspectingThresholdInBytes = 15360;
            EnableNagleAlgorithm = true;
            EncryptedMessageKeyLength = 128;
            AllowServerAsP2PGroupMember = false;
            EnableP2PEncryptedMessaging = false;
            UpnpDetectNatDevice = true;
            UpnpTcpAddrPortMapping = true;
            EnableLookaheadP2PSend = false;
            EnablePingTest = false;
            EmergencyLogLineCount = 0;
        }
    }
}
