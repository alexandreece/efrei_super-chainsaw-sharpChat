using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace super_chainsaw_sharpChatClient
{
    public class Server
    {
        private int port;

        public Server(int port)
        {
            this.port = port; 
        } 

        public void start()
        {
            var l = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            l.Start();

            while (true)
            {
                var comm = l.AcceptTcpClient();
                new Thread(new Receiver(comm).doOperation).Start();
            }
        }

        class Receiver
        {
            private TcpClient comm;

            public Receiver(TcpClient s)
            {
                comm = s;
            }

            public void doOperation()
            {
                while (true)
                {
                    var rcvMsg = Net.rcvMsg(comm.GetStream());
                    switch (rcvMsg)
                    {
                        case ChatroomToJoin chatroomToJoin:
                            break;
                        case CredentialsToConnect credentialsToConnect:
                            break;
                        case MessageToAppend messageToAppend:
                            Console.WriteLine("Server received message: " + messageToAppend);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                    }
                }
            }
        }
    }
}
