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
        public event statusChanged Pending;
        public event statusChanged UsernameAlreadyTaken;
        public event statusChanged UsernameCannotBeEmpty;
        public event statusChanged Disconnected;

        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

        public delegate void chatterChangedChatroom(ChatterChangedChatroom chatterChangedChatroom);
        public event chatterChangedChatroom ChatterChangedChatroom;

        public delegate void serverChatroomsList(List<string> serverChatroomsList);
        public event serverChatroomsList ServerChatroomsList;

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

                            case ConnectionStatusNotification.connectionStatus.pendingConnection:
                                Pending(hostname, port);
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
