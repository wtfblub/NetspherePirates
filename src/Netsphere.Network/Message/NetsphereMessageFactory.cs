using System;
using System.IO;
using BlubLib.Network;
using BlubLib.Serialization;
using Netsphere.Network.Message.Auth;
using Netsphere.Network.Message.Chat;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
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
            Serializer.AddSerializer(new SChannelPlayerListAckMessageSerializer());
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
            Serializer.AddSerializer(new SInventoryInfoAckMessageSerializer());
            Serializer.AddSerializer(new SGameRoomListAckMessageSerializer());
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
}
