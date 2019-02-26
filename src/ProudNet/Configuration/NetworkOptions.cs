using System;
using System.Net;

namespace ProudNet.Configuration
{
    public class NetworkOptions
    {
        public IPEndPoint TcpListener { get; set; }
        public IPAddress UdpAddress { get; set; }
        public ushort[] UdpListenerPorts { get; set; }
        public bool ServerAsP2PGroupMemberHack { get; set; }

        public Guid Version { get; set; }
        public TimeSpan ConnectTimeout { get; set; }
        public TimeSpan HolepunchTimeout { get; set; }
        public int HolepunchMaxRetryCount { get; set; }
        public TimeSpan PingTimeout { get; }

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

        public NetworkOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(10);
            HolepunchTimeout = TimeSpan.FromSeconds(15);
            HolepunchMaxRetryCount = 10;
            // Client sends a ping every 10 seconds
            PingTimeout = TimeSpan.FromSeconds(20);
            ServerAsP2PGroupMemberHack = false;

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
