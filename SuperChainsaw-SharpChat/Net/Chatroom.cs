using System;
using System.Collections.Generic;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.Net
{
    public class Chatroom
    {
        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public string name { get; }
        public DateTime DateCreated { get; }
        public Server.Receiver creator { get; }
        public List<ChatroomMessageAppended> Messages { get; } = new List<ChatroomMessageAppended>();

        public Chatroom(string name, Server.Receiver creator)
        {
            this.name = name;
            this.creator = creator;
            DateCreated = DateTime.Now;
        }

        public void addMessage(string username, string message)
        {
            var chatroomMessageAppended = new ChatroomMessageAppended(username, message);

            Messages.Add(chatroomMessageAppended);

            ChatroomMessageAppended(chatroomMessageAppended);
        }

        public override string ToString() => name;// text shown in notifications and list boxes
    }
}
