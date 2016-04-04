using System;
using System.Net;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace ProudNet
{
    public class ProudConfig
    {
        internal const uint InternalNetVersion = 196713;

        public Guid Version { get; }
        public IPEndPoint UdpListener { get; set; }
        public IPAddress UdpAddress { get; set; }

        [Serialize(0)]
        public bool EnableServerLog { get; set; }

        [Serialize(1, Compiler = typeof(EnumSerializer))]
        public FallbackMethod FallbackMethod { get; set; }

        [Serialize(2)]
        public uint MessageMaxLength { get; set; }

        [Serialize(3)]
        public double TimeoutTimeMs { get; set; }

        [Serialize(4, Compiler = typeof(EnumSerializer))]
        public DirectP2PStartCondition DirectP2PStartCondition { get; set; }

        [Serialize(5)]
        public uint OverSendSuspectingThresholdInBytes { get; set; }

        [Serialize(6)]
        public bool EnableNagleAlgorithm { get; set; }

        [Serialize(7)]
        public int EncryptedMessageKeyLength { get; set; }

        [Serialize(8)]
        public bool AllowServerAsP2PGroupMember { get; set; }

        [Serialize(9)]
        public bool EnableP2PEncryptedMessaging { get; set; }

        [Serialize(10)]
        public bool UpnpDetectNatDevice { get; set; }

        [Serialize(11)]
        public bool UpnpTcpAddrPortMapping { get; set; }

        [Serialize(12)]
        public bool EnablePingTest { get; set; }

        [Serialize(13)]
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
