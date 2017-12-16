using System;
using System.Net.Sockets;
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

            private readonly TcpClient _comm;

            public string username { get; set; }
            public Chatroom chatroom { get; set; }

            public Receiver(TcpClient s) => _comm = s;

            public void doOperation()
            {
                while (true)
                {
                    var rcvMsg = SerializedMessage.readFrom(_comm.GetStream());
                    switch (rcvMsg)
                    {
                        case CredentialsToConnect credentialsToConnect:
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

                        default:
                            throw new ArgumentOutOfRangeException(nameof(rcvMsg));
                    }
                }
            }

            public override string ToString() => username + (chatroom == null ? "" : " (" + chatroom + ")");// text shown in notifications and list boxes

            public void send(SerializedMessage serializedMessage)
            {
                serializedMessage.writeTo(_comm.GetStream());
            }
        }
    }
}
