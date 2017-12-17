using System;
using System.Security;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatroomStatusNotification : ServerToClientMessage
    {
        public enum chatroomStatus
        {
            successfullyCreated,
            chatterStillPending,
            nameCannotBeEmpty,
            nameAlreadyExists,
            deleted
        }

        public chatroomStatus Status { get; }
        public string Name { get; }

        public ChatroomStatusNotification(chatroomStatus status, string name)
        {
            Status = status;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
