using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class MessageToAppend : ClientToServerMessage
    {
        public string Message { get; }

        public MessageToAppend(string message) => Message = message;

        public override string ToString() => Message;
    }
}
