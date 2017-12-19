using System;
using System.Net.Sockets;
using System.Threading;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.Net
{
    public partial class Server
    {
        public class Receiver
        {
            public delegate void connectChatter(string username);
            public event connectChatter ConnectChatter;

            public delegate void manageChatroom(string chatroom);
            public event manageChatroom CreateChatroom;
            public event manageChatroom JoinChatroom;

            public delegate void appendMessage(MessageToAppend messageToAppend);
            public event appendMessage AppendMessage;

            public delegate void disconnectClient(ClientDisconnect clientDisconnect);
            public event disconnectClient DisconnectClient;

            private readonly TcpClient _comm;
            private Thread thread;

            public string username { get; set; }
            public Chatroom chatroom { get; set; }
            public DateTime DateConnected { get; private set; }
            public DateTime DateDisconnected { get; private set; }

            public Receiver(TcpClient s) => _comm = s;

            public void doOperation()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    SerializedMessage rcvMsg;
                    try
                    {
                        rcvMsg = SerializedMessage.readFrom(_comm.GetStream());
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    switch (rcvMsg)
                    {
                        case CredentialsToConnect credentialsToConnect:
                            DateConnected = DateTime.Now;
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

                        case ClientDisconnect clientDisconnect:
                            DateDisconnected = DateTime.Now;
                            DisconnectClient(clientDisconnect);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                    }
                }
            }

            public override string ToString() => username + (chatroom == null ? " " + DateConnected.atDate() : " (" + chatroom + ")");// text shown in notifications and list boxes

            public void send(SerializedMessage serializedMessage)
            {
                serializedMessage.writeTo(_comm.GetStream());
            }

            private void sendDisconnect()
            {
                send(new ChatterDisconnect());
            }

            public void stop()
            {
                sendDisconnect();// warn the server so that it can release the username for future chatters
                _comm.Close();
            }

            public void wait()
            {
                thread.Join();
            }

            public void start(Thread thread)
            {
                this.thread = thread;
                thread.Start();
            }

            public void connectionAccepted()
            {
                DateConnected = DateTime.Now;
            }

            public void connectionRejected()
            {
                DateDisconnected = DateTime.Now;
            }
        }
    }
}
