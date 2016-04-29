using System.IO;
using BlubLib.Network.Message;
using BlubLib.Serialization;
using Netsphere.Network.Message.Auth;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Network.Message.P2P;
using Netsphere.Network.Message.Relay;
using ProudNet.Message;

namespace Netsphere.Network.Message
{
    public abstract class AuthMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(AuthMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public abstract class ChatMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(ChatMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public abstract class GameMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(GameMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public abstract class GameRuleMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(GameRuleMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public abstract class RelayMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(RelayMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public class RelayUnknownMessage : RelayMessage
    {
        public RelayOpCode OpCode { get; }
        public byte[] Data { get; }

        public RelayUnknownMessage(RelayOpCode opCode, byte[] data)
        {
            OpCode = opCode;
            Data = data;
        }

        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(OpCode);
                w.Write(Data);
            }
        }
    }

    public abstract class EventMessage : ProudMessage
    {
        public override void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(EventMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }

    public abstract class P2PMessage : IMessage
    {
        public virtual void Serialize(Stream stream)
        {
            using (var w = stream.ToBinaryWriter(true))
            {
                w.WriteEnum(P2PMapper.GetOpCode(GetType()));
                Serializer.Serialize(w, (object)this);
            }
        }
    }
}
