using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatroomToJoin : ClientToServerMessage
    {
        public string Chatroom { get; }

        public ChatroomToJoin(string chatroom) => Chatroom = chatroom;
    }
}
