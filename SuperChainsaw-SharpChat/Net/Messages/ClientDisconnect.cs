using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ClientDisconnect : ClientToServerMessage
    {
        public DateTime DateDisconnected { get; }

        public ClientDisconnect() => DateDisconnected = DateTime.Now;
    }
}
