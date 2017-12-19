using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ChatterDisconnect : ServerToClientMessage
    {
        public DateTime DateDisconnected { get; }

        public ChatterDisconnect() => DateDisconnected = DateTime.Now;
    }
}
