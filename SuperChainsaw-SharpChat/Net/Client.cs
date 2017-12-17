using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.Net
{
    class Client
    {
        public delegate void statusChanged(string hostname, int port);
        public event statusChanged Connecting;// server waiting for username
        public event statusChanged Connected;
        public event statusChanged Rejected;
        public event statusChanged Pending;
        public event statusChanged UsernameAlreadyTaken;
        public event statusChanged UsernameCannotBeEmpty;
        public event statusChanged Disconnected;

        public delegate void chatroomCreationStatus(string name);
        public event chatroomCreationStatus ChatroomCreated;
        public event chatroomCreationStatus ChatterStillPending;
        public event chatroomCreationStatus ChatroomNameCannotBeEmpty;
        public event chatroomCreationStatus ChatroomNameAlreadyExists;

        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public delegate void chatterChangedChatroom(ChatterChangedChatroom chatterChangedChatroom);
        public event chatterChangedChatroom ChatterChangedChatroom;

        public delegate void serverChatroomsList(List<string> serverChatroomsList);
        public event serverChatroomsList ServerChatroomsList;

        public delegate void chatterDisconnect(ChatterDisconnect chatterDisconnect);
        public event chatterDisconnect ChatterDisconnect;

        private string hostname;
        private int port;
        TcpClient comm;

        public Client(string h, int p)
        {
            hostname = h;
            port = p;
        }

        public void start()
        {
            try
            {
                comm = new TcpClient(hostname, port);
            }
            catch (Exception)
            {
                Disconnected(hostname, port);
                return;
            }
            Connecting(hostname, port);
            while (Thread.CurrentThread.IsAlive)
            {
                SerializedMessage rcvMsg;
                try
                {
                    rcvMsg = SerializedMessage.readFrom(comm.GetStream());
                }
                catch (Exception)
                {
                    return;
                }
                switch (rcvMsg)
                {
                    case ConnectionStatusNotification connectionStatusNotification:
                        switch (connectionStatusNotification.Status)
                        {
                            case ConnectionStatusNotification.connectionStatus.successfullyConnected:
                                Connected(hostname, port);
                                break;

                            case ConnectionStatusNotification.connectionStatus.usernameCannotBeEmpty:
                                UsernameCannotBeEmpty(hostname, port);
                                break;

                            case ConnectionStatusNotification.connectionStatus.usernameAlreadyTaken:
                                UsernameAlreadyTaken(hostname, port);
                                break;

                            case ConnectionStatusNotification.connectionStatus.connectionRejected:
                                Rejected(hostname, port);
                                break;

                            case ConnectionStatusNotification.connectionStatus.pendingConnection:
                                Pending(hostname, port);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;

                    case ChatroomCreationStatusNotification chatroomCreationStatusNotification:
                        switch (chatroomCreationStatusNotification.Status)
                        {
                            case ChatroomCreationStatusNotification.chatroomStatus.successfullyCreated:
                                ChatroomCreated(chatroomCreationStatusNotification.ToString());
                                break;

                            case ChatroomCreationStatusNotification.chatroomStatus.chatterStillPending:
                                ChatterStillPending(chatroomCreationStatusNotification.ToString());
                                break;

                            case ChatroomCreationStatusNotification.chatroomStatus.nameCannotBeEmpty:
                                ChatroomNameCannotBeEmpty(chatroomCreationStatusNotification.ToString());
                                break;

                            case ChatroomCreationStatusNotification.chatroomStatus.nameAlreadyExists:
                                ChatroomNameAlreadyExists(chatroomCreationStatusNotification.ToString());
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;

                    case AvailableChatroomsList availableChatroomsList:
                        ServerChatroomsList(availableChatroomsList.Chatrooms);
                        break;

                    case ChatterChangedChatroom chatterChangedChatroom:
                        ChatterChangedChatroom(chatterChangedChatroom);
                        break;

                    case ChatroomMessageAppended chatroomMessageAppended:
                        ChatroomMessageAppended(chatroomMessageAppended);
                        break;

                    case ChatterDisconnect chatterDisconnect:
                        ChatterDisconnect(chatterDisconnect);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                }
            }
        }

        public void stop()
        {
            if (comm == null)
                return;

            sendDisconnect();// warn the server so that it can release the username for future chatters
            comm.Close();
            comm = null;
        }

        private void sendDisconnect()
        {
            send(new ClientDisconnect());
        }

        private void send(SerializedMessage serializedMessage)
        {
            serializedMessage.writeTo(comm.GetStream());
        }

        public void sendUsername(string username)
        {
            send(new CredentialsToConnect(username, ""));
        }

        public void sendMessage(string message)
        {
            send(new MessageToAppend(message));
        }

        public void createChatroom(string chatroom)
        {
            send(new ChatroomToCreate(chatroom));
        }

        public void joinChatroom(string chatroom)
        {
            send(new ChatroomToJoin(chatroom));
        }
    }
}
