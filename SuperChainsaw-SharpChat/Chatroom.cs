using System.Collections.Generic;

namespace SuperChainsaw_SharpChat
{
    public class Chatroom
    {
        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public string name { get; }
        private List<ChatroomMessageAppended> messages = new List<ChatroomMessageAppended>();

        public Chatroom(string name) => this.name = name;

        public void addMessage(string username, string message)
        {
            var chatroomMessageAppended = new ChatroomMessageAppended(username, message);

            messages.Add(chatroomMessageAppended);

            ChatroomMessageAppended(chatroomMessageAppended);
        }

        public override string ToString() => name;// text shown in notifications and list boxes
    }
}
