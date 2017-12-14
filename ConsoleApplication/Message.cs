using System;
using System.Collections.Generic;

namespace super_chainsaw_sharpChatClient
{
    public interface SerealizedMessage
    {
        string ToString();
    }

    public interface ClientToServerMessage : SerealizedMessage
    {
    }

    public interface ServerToClientMessage : SerealizedMessage
    {
    }

    [Serializable]
    public class CredentialsToConnect : ClientToServerMessage
    {
        public string Username { get; }
        public string Password { get; }

        public CredentialsToConnect(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    [Serializable]
    public class ChatroomToCreate : ClientToServerMessage
    {
        public string Chatroom { get; }

        public ChatroomToCreate(string chatroom) => Chatroom = chatroom;
    }

    [Serializable]
    public class ChatroomToJoin : ClientToServerMessage
    {
        public string Chatroom { get; }

        public ChatroomToJoin(string chatroom) => Chatroom = chatroom;
    }

    [Serializable]
    public class MessageToAppend : ClientToServerMessage
    {
        public string Message { get; }

        public MessageToAppend(string message) => Message = message;

        public override string ToString() => Message;
    }

    [Serializable]
    public class ConnectionStatusNotification : ServerToClientMessage
    {
        public enum connectionStatus
        {
            succesfullyConnected,
            usernameAlreadyTaken,
            usernameNotFound,
            wrongPassword
        }

        public connectionStatus Status { get; }

        public ConnectionStatusNotification(connectionStatus status) => Status = status;
    }

    [Serializable]
    public class AvailableChatroomsList : ServerToClientMessage
    {
        public List<string> Chatrooms { get; }

        public AvailableChatroomsList(List<string> chatrooms) => Chatrooms = chatrooms;
    }

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
