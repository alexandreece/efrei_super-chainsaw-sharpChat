using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public abstract class SerializedMessage
    {
        public void writeTo(Stream s) => new BinaryFormatter().Serialize(s, this);

        public static SerializedMessage readFrom(Stream s) => (SerializedMessage) new BinaryFormatter().Deserialize(s);
    }
}
