using System;
using System.Net.Sockets;

namespace super_chainsaw_sharpChatClient
{
    class Client
    {
        private string hostname;
        private int port;

        public Client(string h, int p)
        {
            hostname = h;
            port = p;
        }

        public void start()
        {
            var comm = new TcpClient(hostname, port);
            while (true)
            {
                Net.sendMsg(comm.GetStream(), new MessageToAppend("hello"));

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
    }
}
