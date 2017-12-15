using System.Collections.Generic;

namespace super_chainsaw_sharpChatClient
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

        public Chatroom fromName(string chatroomString)
        {
            foreach (var chatroom in this)
                if (chatroom.name == chatroomString)
                    return chatroom;

            return null;
        }
    }
}
