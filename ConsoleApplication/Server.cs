using System;
using System.Collections.Generic;
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

        private List<Chatroom> chatrooms;
        private List<Receiver> chattersNotChattingYet = new List<Receiver>();
        private List<KeyValuePair<Receiver, Chatroom> > chatters;

        public Server(int port) => this.port = port;

        public void start()
        {
            var l = new TcpListener(new IPAddress(localhost), port);
            l.Start();
            Started();

            while (true)
            {
                var comm = new Receiver(l.AcceptTcpClient());
                chattersNotChattingYet.Add(comm);
                comm.JoinChatroom +=
                    delegate(Receiver receiver, string chatroomString)
                    {
                        Chatroom chatroom = null;
                        {
                            foreach (var c in chatrooms)
                                if (c.name == chatroomString)
                                    chatroom = c;
                            if (chatroom == null)
                                throw new ArgumentNullException(nameof(chatroom));
                        }

                        chattersNotChattingYet.Remove(receiver);// todo : also consider the case where the chatter was already in another chatroom
                        chatters.Add(new KeyValuePair<Receiver, Chatroom>(receiver, chatroom));

                        chatroom.ChatroomMessageAppended +=
                            delegate(ChatroomMessageAppended appended)
                            {
                                foreach (var chatter in chatters)
                                    if (chatter.Value == chatroom)
                                        Net.sendMsg(chatter.Key.comm.GetStream(), appended);
                            };
                    };
                new Thread(comm.doOperation).Start();
            }
        }

        class Receiver
        {
            public delegate void joinChatroom(Receiver receiver, string chatroom);
            public event joinChatroom JoinChatroom;

            public TcpClient comm { get; }

            public Receiver(TcpClient s) => comm = s;

            public void doOperation()
            {
                while (true)
                {
                    var rcvMsg = Net.rcvMsg(comm.GetStream());
                    switch (rcvMsg)
                    {
                        case ChatroomToJoin chatroomToJoin:
                            JoinChatroom(this, chatroomToJoin.Chatroom);
                            break;
                        case CredentialsToConnect credentialsToConnect:
                            break;
                        case MessageToAppend messageToAppend:
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
