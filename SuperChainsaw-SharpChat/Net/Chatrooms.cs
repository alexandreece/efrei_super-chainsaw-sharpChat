using System.Collections.Generic;

namespace SuperChainsaw_SharpChat.Net
{
    public class Chatrooms : List<Chatroom>
    {
        public delegate void manageChatrooms(Chatroom chatroom);
        public event manageChatrooms ChatroomAdded;

        public new void Add(Chatroom chatroom)
        {
            base.Add(chatroom);
            ChatroomAdded(chatroom);
        }

        public List<string> names()
        {
            List<string> chatroomsNames = new List<string>();
            foreach (var chatroom in this)
                chatroomsNames.Add(chatroom.name);

            return chatroomsNames;
        }

        public Chatroom fromName(string chatroomName)
        {
            foreach (var chatroom in this)
                if (chatroom.name == chatroomName)
                    return chatroom;

            return null;
        }

        public bool isNameTaken(string chatroomName)
        {
            foreach (var chatroom in this)
                if (chatroom.name == chatroomName)
                    return true;

            return false;
        }
    }
}
