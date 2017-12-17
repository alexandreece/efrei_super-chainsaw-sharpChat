using System;
using System.Security;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatroomCreationStatusNotification : ServerToClientMessage
    {
        public enum chatroomStatus
        {
            successfullyCreated,
            chatterStillPending,
            nameCannotBeEmpty,
            nameAlreadyExists
        }

        public chatroomStatus Status { get; }
        public string Name { get; }

        public ChatroomCreationStatusNotification(chatroomStatus status, string name)
        {
            Status = status;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
