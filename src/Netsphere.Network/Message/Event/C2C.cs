using System;
using BlubLib.Serialization;
using ProudNet.Serializers;

namespace Netsphere.Network.Message.Event
{
    public class ChatMessage : EventMessage
    {
        [Serialize(0, typeof(StringSerializer))]
        public string Message { get; set; }
    }

    public class EventMessageMessage : EventMessage
    {
        [Serialize(0, typeof(EnumSerializer), typeof(uint))]
        public GameEventMessage Event { get; set; }

        [Serialize(1)]
        public ulong AccountId { get; set; }

        [Serialize(2)]
        public uint Unk { get; set; } // server/game time or something like that

        [Serialize(3)]
        public ushort Value { get; set; }

        [Serialize(4, typeof(StringSerializer))]
        public string String { get; set; }

        public EventMessageMessage()
        {
            String = "";
        }

        public EventMessageMessage(GameEventMessage @event, ulong accountId, uint unk, ushort value, string @string)
        {
            Event = @event;
            AccountId = accountId;
            Unk = unk;
            Value = value;
            String = @string;
        }
    }

    public class ChangeTargetMessage : EventMessage
    {
        [Serialize(0)]
        public short Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }
    }

    public class ArcadeSyncMessage : EventMessage
    {
        [Serialize(0)]
        public byte Unk1 { get; set; }

        [Serialize(1)]
        public int Unk2 { get; set; }

        [Serialize(2, typeof(ArrayWithScalarSerializer))]
        public byte[] Unk3 { get; set; }

        public ArcadeSyncMessage()
        {
            Unk3 = Array.Empty<byte>();
        }
    }

    public class ArcadeSyncReqMessage : EventMessage
    {
        [Serialize(0)]
        public int Unk1 { get; set; }

        [Serialize(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Unk2 { get; set; }

        public ArcadeSyncReqMessage()
        {
            Unk2 = Array.Empty<byte>();
        }
    }

    public class PacketMessage : EventMessage
    {
        [Serialize(0)]
        public bool IsCompressed { get; set; }

        [Serialize(1, typeof(ArrayWithScalarSerializer))]
        public byte[] Data { get; set; }

        public PacketMessage()
        {
            Data = Array.Empty<byte>();
        }

        public PacketMessage(bool isCompressed, byte[] data)
        {
            IsCompressed = isCompressed;
            Data = data;
        }
    }
}