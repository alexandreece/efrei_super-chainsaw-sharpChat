using System;
using System.Collections.Generic;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class AvailableChatroomsList : ServerToClientMessage
    {
        public List<string> Chatrooms { get; }

        public AvailableChatroomsList(List<string> chatrooms) => Chatrooms = chatrooms;
    }
}
