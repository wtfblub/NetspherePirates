using BlubLib.Serialization;
using Netsphere.Network.Serializers;
using ProudNet.Serialization;

namespace Netsphere.Network.Message.Chat
{
    public interface IChatMessage
    { }

    public class ChatMessageFactory : MessageFactory<ChatOpCode, IChatMessage>
    {
        static ChatMessageFactory()
        {
            Serializer.AddCompiler(new ItemNumberSerializer());
        }

        public ChatMessageFactory()
        {
            // S2C
            Register<SLoginAckMessage>(ChatOpCode.SLoginAck);
            Register<SFriendAckMessage>(ChatOpCode.SFriendAck);
            Register<SFriendListAckMessage>(ChatOpCode.SFriendListAck);
            Register<SCombiAckMessage>(ChatOpCode.SCombiAck);
            Register<SCombiListAckMessage>(ChatOpCode.SCombiListAck);
            Register<SCheckCombiNameAckMessage>(ChatOpCode.SCheckCombiNameAck);
            Register<SDenyChatAckMessage>(ChatOpCode.SDenyChatAck);
            Register<SDenyChatListAckMessage>(ChatOpCode.SDenyChatListAck);
            Register<SUserDataAckMessage>(ChatOpCode.SUserDataAck);
            Register<SUserDataListAckMessage>(ChatOpCode.SUserDataListAck);
            Register<SChannelPlayerListAckMessage>(ChatOpCode.SChannelPlayerListAck);
            Register<SChannelEnterPlayerAckMessage>(ChatOpCode.SChannelEnterPlayerAck);
            Register<SChannelLeavePlayerAckMessage>(ChatOpCode.SChannelLeavePlayerAck);
            Register<SChatMessageAckMessage>(ChatOpCode.SChatMessageAck);
            Register<SWhisperChatMessageAckMessage>(ChatOpCode.SWhisperChatMessageAck);
            Register<SInvitationPlayerAckMessage>(ChatOpCode.SInvitationPlayerAck);
            Register<SClanMemberListAckMessage>(ChatOpCode.SClanMemberListAck);
            Register<SNoteListAckMessage>(ChatOpCode.SNoteListAck);
            Register<SSendNoteAckMessage>(ChatOpCode.SSendNoteAck);
            Register<SReadNoteAckMessage>(ChatOpCode.SReadNoteAck);
            Register<SDeleteNoteAckMessage>(ChatOpCode.SDeleteNoteAck);
            Register<SNoteErrorAckMessage>(ChatOpCode.SNoteErrorAck);
            Register<SNoteReminderInfoAckMessage>(ChatOpCode.SNoteReminderInfoAck);

            // C2S
            Register<CLoginReqMessage>(ChatOpCode.CLoginReq);
            Register<CDenyChatReqMessage>(ChatOpCode.CDenyChatReq);
            Register<CFriendReqMessage>(ChatOpCode.CFriendReq);
            Register<CCheckCombiNameReqMessage>(ChatOpCode.CCheckCombiNameReq);
            Register<CCombiReqMessage>(ChatOpCode.CCombiReq);
            Register<CGetUserDataReqMessage>(ChatOpCode.CGetUserDataReq);
            Register<CSetUserDataReqMessage>(ChatOpCode.CSetUserDataReq);
            Register<CChatMessageReqMessage>(ChatOpCode.CChatMessageReq);
            Register<CWhisperChatMessageReqMessage>(ChatOpCode.CWhisperChatMessageReq);
            Register<CInvitationPlayerReqMessage>(ChatOpCode.CInvitationPlayerReq);
            Register<CNoteListReqMessage>(ChatOpCode.CNoteListReq);
            Register<CSendNoteReqMessage>(ChatOpCode.CSendNoteReq);
            Register<CReadNoteReqMessage>(ChatOpCode.CReadNoteReq);
            Register<CDeleteNoteReqMessage>(ChatOpCode.CDeleteNoteReq);
            Register<CNoteReminderInfoReqMessage>(ChatOpCode.CNoteReminderInfoReq);
        }
    }
}
