using System;
using System.Net.Sockets;

namespace super_chainsaw_sharpChatClient
{
    class Client
    {
        public delegate void statusChanged();
        public event statusChanged Started;

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
            Started();
            while (true)
            {
                var rcvMsg = Net.rcvMsg(comm.GetStream());
                switch (rcvMsg)
                {
                    case ChatroomMessageAppended chatroomMessageAppended:
                        break;
                    case AvailableChatroomsList availableChatroomsList:
                        break;
                    case ConnectionStatusNotification connectionStatusNotification:
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

        public void sendMessage(string message)
        {
            Net.sendMsg(comm.GetStream(), new MessageToAppend(message));
        }
    }
}
