using System;
using System.Net.Sockets;

namespace super_chainsaw_sharpChatClient
{
    class Client
    {
        public delegate void statusChanged(string hostname, int port);
        public event statusChanged Connecting;// server waiting for username
        public event statusChanged Connected;
        public event statusChanged UsernameAlreadyTaken;

        public delegate void chatroomMessageAppended(ChatroomMessageAppended chatroomMessageAppended);
        public event chatroomMessageAppended ChatroomMessageAppended;

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
            while (true)
            {
                var rcvMsg = Net.rcvMsg(comm.GetStream());
                switch (rcvMsg)
                {
                    case ChatroomMessageAppended chatroomMessageAppended:
                        ChatroomMessageAppended(chatroomMessageAppended);
                        break;

                    case AvailableChatroomsList availableChatroomsList:
                        break;

                    case ConnectionStatusNotification connectionStatusNotification:
                        switch (connectionStatusNotification.Status)
                        {
                            case ConnectionStatusNotification.connectionStatus.succesfullyConnected:
                                Connected(hostname, port);
                                break;

                            case ConnectionStatusNotification.connectionStatus.usernameAlreadyTaken:
                                UsernameAlreadyTaken(hostname, port);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                }
            }
        }

        public void stop()
        {
            // todo : disconnect client, stop infinite loops and send notification signal to write RTF message
        }

        public void sendUsername(string username)
        {
            Net.sendMsg(comm.GetStream(), new CredentialsToConnect(username, ""));
        }

        public void sendMessage(string message)
        {
            Net.sendMsg(comm.GetStream(), new MessageToAppend(message));
        }

        public void createChatroom(string chatroom)
        {
            Net.sendMsg(comm.GetStream(), new ChatroomToCreate(chatroom));
        }

        public void joinChatroom(string chatroom)
        {
            Net.sendMsg(comm.GetStream(), new ChatroomToJoin(chatroom));
        }
    }
}
