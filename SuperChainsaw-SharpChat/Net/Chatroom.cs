using System.Collections.Generic;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.Net
{
    public class Chatroom
    {
        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public string name { get; }
        public List<ChatroomMessageAppended> Messages { get; } = new List<ChatroomMessageAppended>();

        public Chatroom(string name) => this.name = name;

        public void addMessage(string username, string message)
        {
            var chatroomMessageAppended = new ChatroomMessageAppended(username, message);

            Messages.Add(chatroomMessageAppended);

            ChatroomMessageAppended(chatroomMessageAppended);
        }

        public override string ToString() => name;// text shown in notifications and list boxes
    }
}
