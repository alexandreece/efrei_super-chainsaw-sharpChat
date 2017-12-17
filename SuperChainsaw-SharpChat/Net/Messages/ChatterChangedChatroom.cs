using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatterChangedChatroom : ServerToClientMessage
    {
        public string Chatroom { get; }
        public DateTime DateChanged { get; }

        public ChatterChangedChatroom(string chatroomName)
        {
            Chatroom = chatroomName;
            DateChanged = DateTime.Now;
        }
    }
}
