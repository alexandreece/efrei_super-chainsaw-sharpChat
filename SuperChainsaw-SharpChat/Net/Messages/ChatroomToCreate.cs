using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatroomToCreate : ClientToServerMessage
    {
        public string Chatroom { get; }

        public ChatroomToCreate(string chatroom) => Chatroom = chatroom;
    }
}
