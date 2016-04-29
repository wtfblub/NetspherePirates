using System;
using System.Net;
using BlubLib.Serialization;

namespace ProudNet
{
    [BlubContract]
    public class ProudConfig
    {
        internal const uint InternalNetVersion = 196713;

        public Guid Version { get; }
        public IPEndPoint UdpListener { get; set; }
        public IPAddress UdpAddress { get; set; }

        [BlubMember(0)]
        public bool EnableServerLog { get; set; }

        [BlubMember(1)]
        public FallbackMethod FallbackMethod { get; set; }

        [BlubMember(2)]
        public uint MessageMaxLength { get; set; }

        [BlubMember(3)]
        public double TimeoutTimeMs { get; set; }

        [BlubMember(4)]
        public DirectP2PStartCondition DirectP2PStartCondition { get; set; }

        [BlubMember(5)]
        public uint OverSendSuspectingThresholdInBytes { get; set; }

        [BlubMember(6)]
        public bool EnableNagleAlgorithm { get; set; }

        [BlubMember(7)]
        public int EncryptedMessageKeyLength { get; set; }

        [BlubMember(8)]
        public bool AllowServerAsP2PGroupMember { get; set; }

        [BlubMember(9)]
        public bool EnableP2PEncryptedMessaging { get; set; }

        [BlubMember(10)]
        public bool UpnpDetectNatDevice { get; set; }

        [BlubMember(11)]
        public bool UpnpTcpAddrPortMapping { get; set; }

        [BlubMember(12)]
        public bool EnablePingTest { get; set; }

        [BlubMember(13)]
        public uint EmergencyLogLineCount { get; set; }

        public ProudConfig()
        {
            Version = Guid.Empty;

            EnableServerLog = false;
            FallbackMethod = FallbackMethod.None;
            MessageMaxLength = 65000;
            TimeoutTimeMs = 900;
            DirectP2PStartCondition = DirectP2PStartCondition.Jit;
            OverSendSuspectingThresholdInBytes = 15360;
            EnableNagleAlgorithm = true;
            EncryptedMessageKeyLength = 128;
            AllowServerAsP2PGroupMember = false;
            EnableP2PEncryptedMessaging = false;
            UpnpDetectNatDevice = true;
            UpnpTcpAddrPortMapping = true;
            EnablePingTest = false;
            EmergencyLogLineCount = 0;
        }

        public ProudConfig(Guid version)
            : this()
        {
            Version = version;
        }
    }
}
