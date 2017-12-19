using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatroomMessageAppended : ServerToClientMessage
    {
        public string Username { get; }
        public string Message { get; }
        public DateTime DateAdded { get; }

        public ChatroomMessageAppended(string username, string message)
        {
            Username = username;
            Message = message;
            DateAdded = DateTime.Now;
        }
    }
}
