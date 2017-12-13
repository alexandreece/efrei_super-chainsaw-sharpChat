﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace super_chainsaw_sharpChatClient
{
    public class Server
    {
        public delegate void update();
        public event update Started;

        private int port;
        private byte[] localhost = {127, 0, 0, 1};

        public Server(int port) => this.port = port;

        public void start()
        {
            var l = new TcpListener(new IPAddress(localhost), port);
            l.Start();
            Started();

            while (true)
            {
                var comm = l.AcceptTcpClient();
                new Thread(new Receiver(comm).doOperation).Start();
            }
        }

        class Receiver
        {
            private TcpClient comm;

            public Receiver(TcpClient s) => comm = s;

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

        public void stop()
        {
            // todo : send notification then disconnect every client then delete TCP server
            // todo : check what happens with the infinite while loops
        }
    }
}