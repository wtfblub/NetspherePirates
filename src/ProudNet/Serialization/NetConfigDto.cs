using BlubLib.Serialization;

namespace ProudNet.Serialization
{
    [BlubContract]
    internal class NetConfigDto
    {
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
        public uint FastEncryptedMessageKeyLength { get; set; }

        [BlubMember(9)]
        public bool AllowServerAsP2PGroupMember { get; set; }

        [BlubMember(10)]
        public bool EnableP2PEncryptedMessaging { get; set; }

        [BlubMember(11)]
        public bool UpnpDetectNatDevice { get; set; }

        [BlubMember(12)]
        public bool UpnpTcpAddrPortMapping { get; set; }
        
        [BlubMember(13)]
        public bool EnableLookaheadP2PSend { get; set; }

        [BlubMember(14)]
        public bool EnablePingTest { get; set; }

        [BlubMember(15)]
        public uint EmergencyLogLineCount { get; set; }
    }
}
