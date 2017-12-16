﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace super_chainsaw_sharpChatClient
{
    [Serializable]
    public abstract class SerializedMessage
    {
        public void writeTo(Stream s) => new BinaryFormatter().Serialize(s, this);

        public static SerializedMessage readFrom(Stream s) => (SerializedMessage) new BinaryFormatter().Deserialize(s);
    }

    [Serializable]
    public abstract class ClientToServerMessage : SerializedMessage
    {
    }

    [Serializable]
    public abstract class ServerToClientMessage : SerializedMessage
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
            successfullyConnected,
            usernameCannotBeEmpty,
            usernameAlreadyTaken,
            pendingConnection,
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
