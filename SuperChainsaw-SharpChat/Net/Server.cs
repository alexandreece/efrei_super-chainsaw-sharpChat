﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.Net
{
    public partial class Server
    {
        public delegate void update(int port);
        public event update Started;

        public delegate void chatterUpdate(Receiver receiver);
        public event chatterUpdate ChatterPending;
        public event chatterUpdate ChatterAccepted;
        public event chatterUpdate ChatterChangedChatroom;
        public event chatterUpdate ChatterDisconnected;

        public delegate void manageChatrooms(Chatroom chatroom);
        public event manageChatrooms ChatroomAdded;

        private int port;
        private byte[] localhost = {127, 0, 0, 1};

        private Chatrooms chatrooms = new Chatrooms();
        private List<Receiver> pendingConnections = new List<Receiver>();
        private List<Receiver> chattersNotChattingYet = new List<Receiver>();
        private List<KeyValuePair<Receiver, Chatroom> > chatters = new List<KeyValuePair<Receiver, Chatroom> >();

        public Server(int port)
        {
            this.port = port;

            chatrooms.ChatroomAdded += chatroom => ChatroomAdded(chatroom);
            chatrooms.ChatroomAdded +=
                delegate
                {
                    foreach (var chatterNotChattingYet in chattersNotChattingYet)
                        chatterNotChattingYet.send(new AvailableChatroomsList(chatrooms.names()));
                    foreach (var chatter in chatters)
                        chatter.Key.send(new AvailableChatroomsList(chatrooms.names()));
                };
        }

        public void start()
        {
            var l = new TcpListener(new IPAddress(localhost), port);
            l.Start();
            Started(port);

            while (Thread.CurrentThread.IsAlive)
            {
                var comm = new Receiver(l.AcceptTcpClient());
                pendingConnections.Add(comm);
                comm.ConnectChatter +=
                    delegate(string username)
                    {
                        bool usernameAlreadyTaken = false;
                        {
                            foreach (var pendingConnection in pendingConnections)
                                if (pendingConnection.username == username)
                                    usernameAlreadyTaken = true;
                            foreach (var chatterNotChattingYet in chattersNotChattingYet)
                                if (chatterNotChattingYet.username == username)
                                    usernameAlreadyTaken = true;
                            foreach (var chatter in chatters)
                                if (chatter.Key.username == username)
                                    usernameAlreadyTaken = true;
                        }

                        if (usernameAlreadyTaken)
                            comm.send(new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.usernameAlreadyTaken));
                        else if (username.Length == 0)
                            comm.send(new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.usernameCannotBeEmpty));
                        else
                        {
                            comm.username = username;
                            ChatterPending(comm);

                            comm.send(new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.pendingConnection));
                        }
                    };
                comm.CreateChatroom +=
                    delegate(string chatroomName)
                    {
                        if (pendingConnections.Contains(comm))
                            return;// client not connected yet so not allowed to create a chatroom; client must be accepted first

                        var chatroom = new Chatroom(chatroomName);
                        chatrooms.Add(chatroom);// todo : store information that <comm> created this chatroom (print their username)

                        chatroom.ChatroomMessageAppended +=
                            delegate(ChatroomMessageAppended appended)
                            {
                                foreach (var chatter in chatters)
                                    if (chatter.Value == chatroom)
                                        chatter.Key.send(appended);
                            };
                    };
                comm.JoinChatroom +=
                    delegate(string chatroomName)
                    {
                        Chatroom chatroom = chatrooms.fromName(chatroomName);
                        if (chatroom == null)
                            throw new ArgumentNullException(nameof(chatroom));

                        chattersNotChattingYet.Remove(comm);
                        foreach (var chatter in chatters)
                            if (chatter.Key == comm)
                            {
                                chatters.Remove(chatter);
                                break;
                            }
                        chatters.Add(new KeyValuePair<Receiver, Chatroom>(comm, chatroom));

                        comm.chatroom = chatroom;
                        ChatterChangedChatroom(comm);
                    };
                ChatterChangedChatroom +=
                    delegate(Receiver receiver)
                    {
                        Chatroom chatroom = null;
                        foreach (var chatter in chatters)
                            if (chatter.Key == receiver)
                            {
                                chatroom = chatter.Value;
                                break;
                            }
                        if (chatroom == null)
                            throw new ArgumentNullException(nameof(chatroom));

                        receiver.send(new ChatterChangedChatroom(chatroom.name));
                        foreach (var message in chatroom.Messages)
                            receiver.send(message);
                    };
                comm.DisconnectClient +=
                    delegate(ClientDisconnect clientDisconnect)// todo : store and print dates when chatter joined and disconnected
                    {
                        pendingConnections.Remove(comm);
                        chattersNotChattingYet.Remove(comm);
                        foreach (var chatter in chatters)
                            if (chatter.Key == comm)
                            {
                                chatters.Remove(chatter);
                                break;
                            }
                        ChatterDisconnected(comm);
                    };
                comm.AppendMessage +=
                    delegate(MessageToAppend messageToAppend)
                    {
                        foreach (var chatter in chatters)
                            if (chatter.Key == comm)
                                chatter.Value.addMessage(comm.username, messageToAppend.Message);
                    };
                new Thread(comm.doOperation).Start();
            }
        }

        public void stop()
        {
            // todo : send notification then disconnect every client then delete TCP server
            // todo : check what happens with the infinite while loops
        }

        public void acceptConnection(object chatter)
        {
            if (!(chatter is Receiver receiver))
                return;

            pendingConnections.Remove(receiver);
            chattersNotChattingYet.Add(receiver);
            ChatterAccepted(receiver);

            receiver.send(new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.successfullyConnected));
            receiver.send(new AvailableChatroomsList(chatrooms.names()));
        }
    }
}
