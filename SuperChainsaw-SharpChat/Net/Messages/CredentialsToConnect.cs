using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class CredentialsToConnect : ClientToServerMessage
    {
        public string Username { get; }
        public string Password { get; }

        public CredentialsToConnect(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
