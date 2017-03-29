using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BlubLib.Serialization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Serialization.Messages.Core
{
    [BlubContract]
    internal class ConnectServerTimedoutMessage : ICoreMessage
    { }

    [BlubContract(typeof(Serializer))]
    internal class NotifyServerConnectionHintMessage : ICoreMessage
    {
        public NetConfigDto Config { get; set; }
        public RSAParameters PublicKey { get; set; }

        public NotifyServerConnectionHintMessage()
        {
            Config = new NetConfigDto();
            PublicKey = new RSAParameters();
        }

        public NotifyServerConnectionHintMessage(NetConfigDto config, RSAParameters publicKey)
        {
            Config = config;
            PublicKey = publicKey;
        }

        internal class Serializer : ISerializer<NotifyServerConnectionHintMessage>
        {
            public bool CanHandle(Type type) => type == typeof(NotifyServerConnectionHintMessage);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(BinaryWriter writer, NotifyServerConnectionHintMessage value)
            {
                var pubKey = DotNetUtilities.GetRsaPublicKey(value.PublicKey);
                var pubKeyStruct = new RsaPublicKeyStructure(pubKey.Modulus, pubKey.Exponent);
                var encodedKey = pubKeyStruct.GetDerEncoded();
                BlubLib.Serialization.Serializer.Serialize(writer, value.Config);
                writer.WriteStruct(encodedKey);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NotifyServerConnectionHintMessage Deserialize(BinaryReader reader)
            {
                var config = BlubLib.Serialization.Serializer.Deserialize<NetConfigDto>(reader);
                var encodedKey = reader.ReadStruct();
                var sequence = (DerSequence)Asn1Object.FromByteArray(encodedKey);
                var modulus = ((DerInteger)sequence[0]).Value.ToByteArrayUnsigned();
                var exponent = ((DerInteger)sequence[1]).Value.ToByteArrayUnsigned();
                var publicKey = new RSAParameters { Exponent = exponent, Modulus = modulus };
                return new NotifyServerConnectionHintMessage(config, publicKey);
            }
        }
    }

    [BlubContract]
    internal class NotifyCSSessionKeySuccessMessage : ICoreMessage
    { }

    [BlubContract]
    internal class NotifyProtocolVersionMismatchMessage : ICoreMessage
    { }

    [BlubContract]
    internal class NotifyServerDeniedConnectionMessage : ICoreMessage
    {
        [BlubMember(0)]
        public ushort Unk { get; set; }
    }

    [BlubContract]
    internal class NotifyServerConnectSuccessMessage : ICoreMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1)]
        public Guid Version { get; set; }

        [BlubMember(2, typeof(ArrayWithScalarSerializer))]
        public byte[] UserData { get; set; }

        [BlubMember(3, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public NotifyServerConnectSuccessMessage()
        {
            Version = Guid.Empty;
            UserData = Array.Empty<byte>();
            EndPoint = new IPEndPoint(0, 0);
        }

        public NotifyServerConnectSuccessMessage(uint hostId, Guid version, IPEndPoint endPoint)
            : this()
        {
            HostId = hostId;
            Version = version;
            EndPoint = endPoint;
        }
    }

    [BlubContract]
    internal class RequestStartServerHolepunchMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        public RequestStartServerHolepunchMessage()
        {
            MagicNumber = Guid.Empty;
        }

        public RequestStartServerHolepunchMessage(Guid magicNumber)
        {
            MagicNumber = magicNumber;
        }
    }

    [BlubContract]
    internal class ServerHolepunchAckMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        public ServerHolepunchAckMessage()
        {
            MagicNumber = Guid.Empty;
            EndPoint = new IPEndPoint(0, 0);
        }

        public ServerHolepunchAckMessage(Guid magicNumber, IPEndPoint endPoint)
        {
            MagicNumber = magicNumber;
            EndPoint = endPoint;
        }
    }

    [BlubContract]
    internal class NotifyClientServerUdpMatchedMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        public NotifyClientServerUdpMatchedMessage()
        {
            MagicNumber = Guid.Empty;
        }

        public NotifyClientServerUdpMatchedMessage(Guid magicNumber)
        {
            MagicNumber = magicNumber;
        }
    }

    [BlubContract]
    internal class PeerUdp_ServerHolepunchAckMessage : ICoreMessage
    {
        [BlubMember(0)]
        public Guid MagicNumber { get; set; }

        [BlubMember(1, typeof(IPEndPointSerializer))]
        public IPEndPoint EndPoint { get; set; }

        [BlubMember(2)]
        public uint HostId { get; set; }

        public PeerUdp_ServerHolepunchAckMessage()
        {
            MagicNumber = Guid.Empty;
            EndPoint = new IPEndPoint(0, 0);
        }

        public PeerUdp_ServerHolepunchAckMessage(Guid magicNumber, IPEndPoint endPoint, uint hostId)
        {
            MagicNumber = magicNumber;
            EndPoint = endPoint;
            HostId = hostId;
        }
    }

    [BlubContract]
    internal class UnreliablePongMessage : ICoreMessage
    {
        [BlubMember(0)]
        public double ClientTime { get; set; }

        [BlubMember(1)]
        public double ServerTime { get; set; }

        public UnreliablePongMessage()
        { }

        public UnreliablePongMessage(double clientTime, double serverTime)
        {
            ClientTime = clientTime;
            ServerTime = serverTime;
        }
    }

    [BlubContract]
    internal class ReliableRelay2Message : ICoreMessage
    {
        [BlubMember(0)]
        public RelayDestinationDto Destination { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public ReliableRelay2Message()
        {
            Destination = new RelayDestinationDto();
        }

        public ReliableRelay2Message(RelayDestinationDto destination, byte[] data)
        {
            Destination = destination;
            Data = data;
        }
    }

    [BlubContract]
    internal class UnreliableRelay2Message : ICoreMessage
    {
        [BlubMember(0)]
        public uint HostId { get; set; }

        [BlubMember(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public UnreliableRelay2Message()
        { }

        public UnreliableRelay2Message(uint hostId, byte[] data)
        {
            HostId = hostId;
            Data = data;
        }
    }
}
