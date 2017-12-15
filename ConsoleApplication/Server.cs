﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace super_chainsaw_sharpChatClient
{
    public class Server
    {
        public delegate void update(int port);
        public event update Started;

        public delegate void manageChatrooms(Chatroom chatroom);
        public event manageChatrooms ChatroomAdded;

        private int port;
        private byte[] localhost = {127, 0, 0, 1};

        private Chatrooms chatrooms = new Chatrooms();
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
                        Net.sendMsg(chatterNotChattingYet.comm.GetStream(), new AvailableChatroomsList(chatrooms.names()));
                    foreach (var chatter in chatters)
                        Net.sendMsg(chatter.Key.comm.GetStream(), new AvailableChatroomsList(chatrooms.names()));
                };
        }

        public void start()
        {
            var l = new TcpListener(new IPAddress(localhost), port);
            l.Start();
            Started(port);

            while (true)
            {
                var comm = new Receiver(l.AcceptTcpClient());
                chattersNotChattingYet.Add(comm);
                comm.ConnectChatter +=
                    delegate(string username)
                    {
                        if (true /* todo : check if username is already taken */)
                        {
                            comm.username = username;
                            Net.sendMsg(comm.comm.GetStream(), new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.successfullyConnected));
                            Net.sendMsg(comm.comm.GetStream(), new AvailableChatroomsList(chatrooms.names()));
                        }
                        else Net.sendMsg(comm.comm.GetStream(), new ConnectionStatusNotification(ConnectionStatusNotification.connectionStatus.usernameAlreadyTaken));
                    };
                comm.CreateChatroom +=
                    delegate(string chatroom)
                    {
                        chatrooms.Add(new Chatroom(chatroom));// todo : store information that <comm> created this chatroom (print their username)
                    };
                comm.JoinChatroom +=
                    delegate(string chatroomString)
                    {
                        Chatroom chatroom = chatrooms.fromName(chatroomString);
                        if (chatroom == null)
                            throw new ArgumentNullException(nameof(chatroom));

                        chattersNotChattingYet.Remove(comm);// todo : also consider the case where the chatter was already in another chatroom
                        chatters.Add(new KeyValuePair<Receiver, Chatroom>(comm, chatroom));

                        chatroom.ChatroomMessageAppended +=
                            delegate(ChatroomMessageAppended appended)
                            {
/* todo : must be wrong (check later) because every chatter already subscribed when joining the chatroom so no need to send with foreach (will send duplicates?)
                                foreach (var chatter in chatters)
                                    if (chatter.Value == chatroom)
                                        Net.sendMsg(chatter.Key.comm.GetStream(), appended);
*/                              Net.sendMsg(comm.comm.GetStream(), appended);
// todo : chack if better to do this way (joining chatter subscribe to chatroom's signal) or chatroom's signal be handled in chatroom's creation handler and sending to chatters in list using foreach loop
// considering the fact that the chatters must also be properly unsubscribed upon quitting chatroom in order to not get messages coming from several chatrooms at the same time
                            };
                    };
                comm.AppendMessage +=
                    delegate(MessageToAppend messageToAppend)
                    {
                        foreach (var chatter in chatters)
                            if (chatter.Key == comm)
                            {
                                chatter.Value.addMessage(comm.username, messageToAppend.Message);
                            }
                    };
                new Thread(comm.doOperation).Start();
            }
        }

        class Receiver
        {
            public delegate void connectChatter(string username);
            public event connectChatter ConnectChatter;

            public delegate void manageChatroom(string chatroom);
            public event manageChatroom CreateChatroom;
            public event manageChatroom JoinChatroom;

            public delegate void appendMessage(MessageToAppend messageToAppend);
            public event appendMessage AppendMessage;

            public TcpClient comm { get; }

            public string username { get; set; }

            public Receiver(TcpClient s) => comm = s;

            public void doOperation()
            {
                while (true)
                {
                    var rcvMsg = Net.rcvMsg(comm.GetStream());
                    switch (rcvMsg)
                    {
                        case CredentialsToConnect credentialsToConnect:
                            ConnectChatter(credentialsToConnect.Username);
                            break;

                        case ChatroomToCreate chatroomToCreate:
                            CreateChatroom(chatroomToCreate.Chatroom);
                            break;

                        case ChatroomToJoin chatroomToJoin:
                            JoinChatroom(chatroomToJoin.Chatroom);
                            break;

                        case MessageToAppend messageToAppend:
                            AppendMessage(messageToAppend);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                    }
                }
            }
        }

        public void stop()
        {
            // todo : send notification then disconnect every client then delete TCP server
            // todo : check what happens with the infinite while loops
        }
    }
}
