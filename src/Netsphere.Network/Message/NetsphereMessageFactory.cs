using System;
using System.IO;
using BlubLib.Network;
using BlubLib.Serialization;
using Netsphere.Network.Message.Auth;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Event;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Network.Message.Relay;
using Netsphere.Network.Serializers;
using ProudNet.Message;

namespace Netsphere.Network.Message
{
    public interface INetsphereMessageFactory
    {
        ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r);
    }

    public class AuthMessageFactory : INetsphereMessageFactory
    {
        public ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r)
        {
            return AuthMapper.GetMessage((AuthOpCode)opCode, r);
        }
    }

    public class ChatMessageFactory : INetsphereMessageFactory
    {
        static ChatMessageFactory()
        {
            Serializer.AddCompiler(new ItemNumberSerializer());
        }

        public ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r)
        {
            return ChatMapper.GetMessage((ChatOpCode)opCode, r);
        }
    }

    public class GameMessageFactory : INetsphereMessageFactory
    {
        static GameMessageFactory()
        {
            Serializer.AddCompiler(new MatchKeySerializer());
            Serializer.AddCompiler(new LongPeerIdSerializer());
            Serializer.AddCompiler(new CharacterStyleSerializer());
        }

        public ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r)
        {
            if (Enum.IsDefined(typeof(GameOpCode), opCode))
                return GameMapper.GetMessage((GameOpCode)opCode, r);

            if (Enum.IsDefined(typeof(GameRuleOpCode), opCode))
                return GameRuleMapper.GetMessage((GameRuleOpCode)opCode, r);

            throw new NetsphereBadOpCodeException(opCode);
        }
    }

    public class RelayMessageFactory : INetsphereMessageFactory
    {
        static RelayMessageFactory()
        {
            Serializer.AddCompiler(new PeerIdSerializer());
        }

        public ProudMessage GetMessage(ISession session, ushort opCode, BinaryReader r)
        {
            if (Enum.IsDefined(typeof(RelayOpCode), opCode))
                return RelayMapper.GetMessage((RelayOpCode)opCode, r);

            if (Enum.IsDefined(typeof(EventOpCode), opCode))
                return EventMapper.GetMessage((EventOpCode)opCode, r);

            throw new NetsphereBadOpCodeException(opCode);
        }
    }
}
