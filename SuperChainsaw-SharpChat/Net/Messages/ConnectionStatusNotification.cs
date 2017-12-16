using System;

namespace SuperChainsaw_SharpChat.Net.Messages
{
    [Serializable]
    public class ConnectionStatusNotification : ServerToClientMessage
    {
        public enum connectionStatus
        {
            successfullyConnected,
            usernameCannotBeEmpty,
            usernameAlreadyTaken,
            pendingConnection,
            usernameNotFound,
            wrongPassword
        }

        public connectionStatus Status { get; }

        public ConnectionStatusNotification(connectionStatus status) => Status = status;
    }
}
