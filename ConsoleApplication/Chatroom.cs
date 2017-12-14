using System;
using System.Collections.Generic;

namespace super_chainsaw_sharpChatClient
{
    public class Chatroom
    {
        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public string name { get; }
        private List<ChatroomMessageAppended> messages;

        public Chatroom(string name) => this.name = name;

        public void addMessage(string username, string message)
        {
            var chatroomMessageAppended = new ChatroomMessageAppended(username, message);

            messages.Add(chatroomMessageAppended);

            ChatroomMessageAppended(chatroomMessageAppended);
        }
    }
}
