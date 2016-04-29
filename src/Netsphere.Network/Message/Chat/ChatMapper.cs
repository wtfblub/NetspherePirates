using System;
using System.Collections.Generic;
using System.IO;
using BlubLib.Serialization;

namespace Netsphere.Network.Message.Chat
{
    internal static class ChatMapper
    {
        private static readonly Dictionary<ChatOpCode, Type> s_typeLookup = new Dictionary<ChatOpCode, Type>();
        private static readonly Dictionary<Type, ChatOpCode> s_opCodeLookup = new Dictionary<Type, ChatOpCode>();

        static ChatMapper()
        {
            // S2C
            Create<SLoginAckMessage>(ChatOpCode.SLoginAck);
            Create<SFriendAckMessage>(ChatOpCode.SFriendAck);
            Create<SFriendListAckMessage>(ChatOpCode.SFriendListAck);
            Create<SCombiAckMessage>(ChatOpCode.SCombiAck);
            Create<SCombiListAckMessage>(ChatOpCode.SCombiListAck);
            Create<SCheckCombiNameAckMessage>(ChatOpCode.SCheckCombiNameAck);
            Create<SDenyChatAckMessage>(ChatOpCode.SDenyChatAck);
            Create<SDenyChatListAckMessage>(ChatOpCode.SDenyChatListAck);
            Create<SUserDataAckMessage>(ChatOpCode.SUserDataAck);
            Create<SUserDataListAckMessage>(ChatOpCode.SUserDataListAck);
            Create<SChannelPlayerListAckMessage>(ChatOpCode.SChannelPlayerListAck);
            Create<SChannelEnterPlayerAckMessage>(ChatOpCode.SChannelEnterPlayerAck);
            Create<SChannelLeavePlayerAckMessage>(ChatOpCode.SChannelLeavePlayerAck);
            Create<SChatMessageAckMessage>(ChatOpCode.SChatMessageAck);
            Create<SWhisperChatMessageAckMessage>(ChatOpCode.SWhisperChatMessageAck);
            Create<SInvitationPlayerAckMessage>(ChatOpCode.SInvitationPlayerAck);
            Create<SClanMemberListAckMessage>(ChatOpCode.SClanMemberListAck);
            Create<SNoteListAckMessage>(ChatOpCode.SNoteListAck);
            Create<SSendNoteAckMessage>(ChatOpCode.SSendNoteAck);
            Create<SReadNoteAckMessage>(ChatOpCode.SReadNoteAck);
            Create<SDeleteNoteAckMessage>(ChatOpCode.SDeleteNoteAck);
            Create<SNoteErrorAckMessage>(ChatOpCode.SNoteErrorAck);
            Create<SNoteReminderInfoAckMessage>(ChatOpCode.SNoteReminderInfoAck);

            // C2S
            Create<CLoginReqMessage>(ChatOpCode.CLoginReq);
            Create<CDenyChatReqMessage>(ChatOpCode.CDenyChatReq);
            Create<CFriendReqMessage>(ChatOpCode.CFriendReq);
            Create<CCheckCombiNameReqMessage>(ChatOpCode.CCheckCombiNameReq);
            Create<CCombiReqMessage>(ChatOpCode.CCombiReq);
            Create<CGetUserDataReqMessage>(ChatOpCode.CGetUserDataReq);
            Create<CSetUserDataReqMessage>(ChatOpCode.CSetUserDataReq);
            Create<CChatMessageReqMessage>(ChatOpCode.CChatMessageReq);
            Create<CWhisperChatMessageReqMessage>(ChatOpCode.CWhisperChatMessageReq);
            Create<CInvitationPlayerReqMessage>(ChatOpCode.CInvitationPlayerReq);
            Create<CNoteListReqMessage>(ChatOpCode.CNoteListReq);
            Create<CSendNoteReqMessage>(ChatOpCode.CSendNoteReq);
            Create<CReadNoteReqMessage>(ChatOpCode.CReadNoteReq);
            Create<CDeleteNoteReqMessage>(ChatOpCode.CDeleteNoteReq);
            Create<CNoteReminderInfoReqMessage>(ChatOpCode.CNoteReminderInfoReq);
        }

        public static void Create<T>(ChatOpCode opCode)
            where T : ChatMessage, new()
        {
            var type = typeof(T);
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static ChatMessage GetMessage(ChatOpCode opCode, BinaryReader r)
        {
            var type = s_typeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new NetsphereBadOpCodeException(opCode);

            return (ChatMessage)Serializer.Deserialize(r, type);
        }

        public static ChatOpCode GetOpCode<T>()
            where T : ChatMessage
        {
            return GetOpCode(typeof(T));
        }

        public static ChatOpCode GetOpCode(Type type)
        {
            return s_opCodeLookup[type];
        }
    }
}
