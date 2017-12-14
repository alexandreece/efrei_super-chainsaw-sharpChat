using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace super_chainsaw_sharpChatClient
{
    public class Net
    {
        public static void sendMsg(Stream s, Message msg)
        {
            var bf = new BinaryFormatter();
            bf.Serialize(s, msg);
        }

        public static Message rcvMsg(Stream s)
        {
            var bf = new BinaryFormatter();
            return (Message)bf.Deserialize(s);
        }
    }
}
