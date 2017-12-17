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

        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

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
            comm = new TcpClient(hostname, port);
            Connecting(hostname, port);
            while (Thread.CurrentThread.IsAlive)
            {
                var rcvMsg = SerializedMessage.readFrom(comm.GetStream());
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
            // todo : send notification to server so that it releases the username for future chatters
            // todo : disconnect client, stop infinite loops and send notification signal to write RTF message
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
