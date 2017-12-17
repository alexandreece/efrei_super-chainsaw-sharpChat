using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SuperChainsaw_SharpChat.Net;
using SuperChainsaw_SharpChat.Net.Messages;

namespace SuperChainsaw_SharpChat.UI
{
    public sealed class ChatForm : Form
    {
        private void InitializeComponent()
        {
            connectedClientsGroupBox = new GroupBox();
            transferServerButton = new Button();
            disconnectClientButton = new Button();
            connectedClientsList = new ListBox();
            chatroomsGroupBox = new GroupBox();
            removeChatroom = new Button();
            newChatroomGroupBox = new GroupBox();
            createChatroomButton = new Button();
            createChatroomButton.Click +=
                delegate
                {
                    client.createChatroom(chatroomName.Text);
                    chatroomName.Clear();
                };
            chatroomName = new TextBox();
            chatroomsList = new ListBox();
            chatroomsList.SelectedIndexChanged +=
                delegate
                {
                    if (chatroomsList.SelectedItem != null)
                        client.joinChatroom(chatroomsList.SelectedItem.ToString());
                };
            connectionGroupBox = new GroupBox();
            colonLabel = new Label();
            atLabel = new Label();
            connectDisconnectClientButton = new Button();
            connectDisconnectClientButton.Click +=
                delegate
                {
                    connectDisconnectClientButton.Enabled = false;// prevents the user from click a second time before the client is connected

                    if (client == null)
                    {
                        client = new Client(serverAddress.Text, int.Parse(serverPort.Text));// Client(address, (int)serverPort.Value);
                        messagesWriter = new MessagesWriter();
                        client.Connecting +=
                            delegate(string hostname, int port)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("connecting", hostname + ":" + port).RtfText));

                                client.sendUsername(username.Text);

                                connectDisconnectClientButton.Enabled = true;
                            };
                        client.Connected +=
                            delegate(string hostname, int port)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("client connected", hostname + ":" + port).RtfText));
                            };
                        client.Pending +=
                            delegate(string hostname, int port)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("client pending", hostname + ":" + port).RtfText));
                            };
                        client.UsernameCannotBeEmpty +=
                            delegate
                            {
                                messages.Rtf = messagesWriter.notify("connection failed", "username cannot be empty", MessagesWriter.ColorNames.issueOrBadEnd).RtfText;

                                stopClient();
                            };
                        client.UsernameAlreadyTaken +=
                            delegate
                            {
                                messages.Rtf = messagesWriter.notify("connection failed", "username already taken", MessagesWriter.ColorNames.issueOrBadEnd).RtfText;

                                stopClient();
                            };
                        client.Disconnected +=
                            delegate
                            {
                                messages.Rtf = messagesWriter.notify("connection failed", "could not reach the server", MessagesWriter.ColorNames.issueOrBadEnd).RtfText;

                                stopClient();
                            };
                        client.ServerChatroomsList +=
                            delegate(List<string> serverChatroomsList)
                            {
                                chatroomsList.Invoke(new Action(() => chatroomsList.Items.Clear()));
                                foreach (var serverChatroom in serverChatroomsList)
                                    chatroomsList.Invoke(new Action(() => chatroomsList.Items.Add(serverChatroom)));
                            };
                        client.ChatterChangedChatroom +=
                            delegate(ChatterChangedChatroom chatterChangedChatroom)
                            {
                                messagesWriter = new MessagesWriter();
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("you joined a chatroom", chatterChangedChatroom.Chatroom).RtfText));
                            };
                        client.ChatroomMessageAppended +=
                            delegate(ChatroomMessageAppended chatroomMessageAppended)
                            {
                                var username = chatroomMessageAppended.Username;
                                DateTime dateAdded = chatroomMessageAppended.DateAdded;
                                var message = chatroomMessageAppended.Message;

                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.usernameAtDate(username, dateAdded)
                                                                                              .message(message).RtfText));
                            };
                        new Thread(client.start).Start();
                    }
                    else stopClient();
                };
            serverPort = new TextBox();
            serverAddress = new TextBox();
            this.username = new TextBox();
            serverGroupBox = new GroupBox();
            startStopServerButton = new Button();
            startStopServerButton.Click +=
                delegate
                {
                    if (server == null)
                    {
                        server = new Server(int.Parse(newServerPort.Text));// server = new Server((int)newServerPort.Value);
                        messagesWriter = new MessagesWriter();
                        server.Started +=
                            delegate(int port)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("server started", "on port " + port).RtfText));
                                if (serverPort.Text.Length == 0)
                                    serverPort.Invoke(new Action(() => serverPort.Text = port.ToString()));
                            };
                        server.ChatterPending +=
                            delegate(Server.Receiver receiver)
                            {
                                pendingConnections.Invoke(new Action(() => pendingConnections.Items.Add(receiver)));
                                StringBuilder pendingConnectionsList = new StringBuilder();
                                foreach (var pendingConnection in pendingConnections.Items)
                                    if (pendingConnection != receiver)
                                        pendingConnectionsList.Append(pendingConnection).Append(", ");
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("new pending connection for '" + receiver + "'", pendingConnectionsList.ToString().Length == 0 ? "" : "there are other pending connections: " + pendingConnectionsList).RtfText));
                            };
                        server.ChatterAccepted +=
                            delegate(Server.Receiver receiver)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("server notification", "accepted: '" + receiver + "' now connected").RtfText));

                                pendingConnections.Items.Remove(receiver);
                                connectedClientsList.Items.Add(receiver);
                            };
                        server.ChatterChangedChatroom +=
                            delegate(Server.Receiver receiver)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("server notification", "'" + receiver.username + "' now in chatroom '" + receiver.chatroom + "'").RtfText));

                                connectedClientsList.Invoke(new Action(() => connectedClientsList.Items[connectedClientsList.Items.IndexOf(receiver)] = receiver));// refresh the displayed text
                            };
                        server.ChatterDisconnected +=
                            delegate(Server.Receiver receiver)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("chatter disconnected", "'" + receiver.username + "' left the server", MessagesWriter.ColorNames.issueOrBadEnd).RtfText));

                                pendingConnections.Invoke(new Action(() => pendingConnections.Items.Remove(receiver)));
                                connectedClientsList.Invoke(new Action(() => connectedClientsList.Items.Remove(receiver)));
                            };
                        server.ChatroomAdded +=
                            delegate(Chatroom chatroom)
                            {
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("server notification", "chatroom '" + chatroom + "' created").RtfText));

        //                        chatroomsList.Invoke(new Action(() => chatroomsList.Items.Add(chatroom)));
                            };
                        new Thread(server.start).Start();
                    }
                    else stopServer();
                };
            newServerPort = new TextBox();// this.newServerPort = new System.Windows.Forms.NumericUpDown();
            pendingConnectionsGroupBox = new GroupBox();
            rejectPendingConnectionButton = new Button();
            pendingConnections = new ListBox();
            acceptPendingConnectionButton = new Button();
            acceptPendingConnectionButton.Click +=
                delegate
                {
                    server.acceptConnection(pendingConnections.SelectedItem);
                };
            messages = new AutoScrollingRichTextBox();
            this.message = new TextBox();
            sendMessageButton = new Button();
            sendMessageButton.Click +=
                delegate
                {
                    foreach (var message in message.Lines)
                        if (message.Length > 0)
                            client.sendMessage(message);
                    message.Clear();
                };
            connectedClientsGroupBox.SuspendLayout();
            chatroomsGroupBox.SuspendLayout();
            newChatroomGroupBox.SuspendLayout();
            connectionGroupBox.SuspendLayout();
            serverGroupBox.SuspendLayout();
            pendingConnectionsGroupBox.SuspendLayout();
            SuspendLayout();

            connectedClientsGroupBox.Controls.Add(transferServerButton);
            connectedClientsGroupBox.Controls.Add(disconnectClientButton);
            connectedClientsGroupBox.Controls.Add(connectedClientsList);
            connectedClientsGroupBox.Location = new Point(8, 237);
            connectedClientsGroupBox.Name = "connectedClientsGroupBox";
            connectedClientsGroupBox.Size = new Size(200, 294);
            connectedClientsGroupBox.TabIndex = 0;
            connectedClientsGroupBox.TabStop = false;
            connectedClientsGroupBox.Text = "Connected clients";

            transferServerButton.Location = new Point(87, 265);
            transferServerButton.Name = "transferServerButton";
            transferServerButton.Size = new Size(104, 23);
            transferServerButton.TabIndex = 2;
            transferServerButton.Text = "Transfer server";
            transferServerButton.UseVisualStyleBackColor = true;

            disconnectClientButton.Location = new Point(6, 265);
            disconnectClientButton.Name = "disconnectClientButton";
            disconnectClientButton.Size = new Size(75, 23);
            disconnectClientButton.TabIndex = 1;
            disconnectClientButton.Text = "Disconnect";
            disconnectClientButton.UseVisualStyleBackColor = true;

            connectedClientsList.FormattingEnabled = true;
            connectedClientsList.Location = new Point(7, 19);
            connectedClientsList.Name = "connectedClientsList";
            connectedClientsList.Size = new Size(187, 238);
            connectedClientsList.TabIndex = 0;

            chatroomsGroupBox.Controls.Add(removeChatroom);
            chatroomsGroupBox.Controls.Add(newChatroomGroupBox);
            chatroomsGroupBox.Controls.Add(chatroomsList);
            chatroomsGroupBox.Location = new Point(233, 17);
            chatroomsGroupBox.Name = "chatroomsGroupBox";
            chatroomsGroupBox.Size = new Size(200, 533);
            chatroomsGroupBox.TabIndex = 0;
            chatroomsGroupBox.TabStop = false;
            chatroomsGroupBox.Text = "Active chatrooms";

            removeChatroom.Location = new Point(6, 498);
            removeChatroom.Name = "removeChatroom";
            removeChatroom.Size = new Size(188, 23);
            removeChatroom.TabIndex = 2;
            removeChatroom.Text = "Remove";
            removeChatroom.UseVisualStyleBackColor = true;

            newChatroomGroupBox.Controls.Add(createChatroomButton);
            newChatroomGroupBox.Controls.Add(chatroomName);
            newChatroomGroupBox.Location = new Point(7, 20);
            newChatroomGroupBox.Name = "newChatroomGroupBox";
            newChatroomGroupBox.Size = new Size(187, 72);
            newChatroomGroupBox.TabIndex = 1;
            newChatroomGroupBox.TabStop = false;

            createChatroomButton.Location = new Point(7, 37);
            createChatroomButton.Name = "createChatroomButton";
            createChatroomButton.Size = new Size(174, 23);
            createChatroomButton.TabIndex = 1;
            createChatroomButton.Text = "Create chatroom";
            createChatroomButton.UseVisualStyleBackColor = true;

            chatroomName.Location = new Point(7, 11);
            chatroomName.Name = "chatroomName";
            chatroomName.Size = new Size(174, 20);
            chatroomName.TabIndex = 0;

            chatroomsList.FormattingEnabled = true;
            chatroomsList.Location = new Point(6, 98);
            chatroomsList.Name = "chatroomsList";
            chatroomsList.Size = new Size(188, 394);
            chatroomsList.TabIndex = 0;

            connectionGroupBox.Controls.Add(colonLabel);
            connectionGroupBox.Controls.Add(atLabel);
            connectionGroupBox.Controls.Add(connectDisconnectClientButton);
            connectionGroupBox.Controls.Add(serverPort);
            connectionGroupBox.Controls.Add(serverAddress);
            connectionGroupBox.Controls.Add(this.username);
            connectionGroupBox.Location = new Point(439, 17);
            connectionGroupBox.Name = "connectionGroupBox";
            connectionGroupBox.Size = new Size(433, 51);
            connectionGroupBox.TabIndex = 1;
            connectionGroupBox.TabStop = false;
            connectionGroupBox.Text = "username @ address : port";

            colonLabel.AutoSize = true;
            colonLabel.Location = new Point(231, 22);
            colonLabel.Name = "colonLabel";
            colonLabel.Size = new Size(10, 13);
            colonLabel.TabIndex = 5;
            colonLabel.Text = ":";

            atLabel.AutoSize = true;
            atLabel.Location = new Point(125, 22);
            atLabel.Name = "atLabel";
            atLabel.Size = new Size(18, 13);
            atLabel.TabIndex = 4;
            atLabel.Text = "@";

            connectDisconnectClientButton.Location = new Point(291, 17);
            connectDisconnectClientButton.Name = "connectDisconnectClientButton";
            connectDisconnectClientButton.Size = new Size(136, 23);
            connectDisconnectClientButton.TabIndex = 3;
            connectDisconnectClientButton.Text = "Connect / Disconnect";
            connectDisconnectClientButton.UseVisualStyleBackColor = true;

            serverPort.Location = new Point(241, 19);
            serverPort.Name = "serverPort";
            serverPort.Size = new Size(44, 20);
            serverPort.TabIndex = 2;

            serverAddress.Location = new Point(143, 19);
            serverAddress.Name = "serverAddress";
            serverAddress.Size = new Size(88, 20);
            serverAddress.TabIndex = 1;
            serverAddress.TextAlign = HorizontalAlignment.Center;

            this.username.Location = new Point(6, 19);
            this.username.Name = "username";
            this.username.Size = new Size(119, 20);
            this.username.TabIndex = 0;
            this.username.TextAlign = HorizontalAlignment.Right;

            serverGroupBox.Controls.Add(startStopServerButton);
            serverGroupBox.Controls.Add(newServerPort);
            serverGroupBox.Controls.Add(pendingConnectionsGroupBox);
            serverGroupBox.Controls.Add(connectedClientsGroupBox);
            serverGroupBox.Location = new Point(13, 13);
            serverGroupBox.Name = "serverGroupBox";
            serverGroupBox.Size = new Size(214, 537);
            serverGroupBox.TabIndex = 2;
            serverGroupBox.TabStop = false;
            serverGroupBox.Text = "Server";

            startStopServerButton.Location = new Point(112, 17);
            startStopServerButton.Name = "startStopServerButton";
            startStopServerButton.Size = new Size(75, 23);
            startStopServerButton.TabIndex = 2;
            startStopServerButton.Text = "Start / Stop";
            startStopServerButton.UseVisualStyleBackColor = true;

            newServerPort.Location = new Point(6, 19);
            newServerPort.Name = "newServerPort";
            newServerPort.Size = new Size(100, 20);
            newServerPort.TabIndex = 1;

            pendingConnectionsGroupBox.Controls.Add(rejectPendingConnectionButton);
            pendingConnectionsGroupBox.Controls.Add(pendingConnections);
            pendingConnectionsGroupBox.Controls.Add(acceptPendingConnectionButton);
            pendingConnectionsGroupBox.Location = new Point(6, 46);
            pendingConnectionsGroupBox.Name = "pendingConnectionsGroupBox";
            pendingConnectionsGroupBox.Size = new Size(200, 185);
            pendingConnectionsGroupBox.TabIndex = 0;
            pendingConnectionsGroupBox.TabStop = false;
            pendingConnectionsGroupBox.Text = "Pending connections";

            rejectPendingConnectionButton.Location = new Point(89, 146);
            rejectPendingConnectionButton.Name = "rejectPendingConnectionButton";
            rejectPendingConnectionButton.Size = new Size(75, 23);
            rejectPendingConnectionButton.TabIndex = 2;
            rejectPendingConnectionButton.Text = "Reject";
            rejectPendingConnectionButton.UseVisualStyleBackColor = true;

            pendingConnections.FormattingEnabled = true;
            pendingConnections.Location = new Point(6, 19);
            pendingConnections.Name = "pendingConnections";
            pendingConnections.Size = new Size(187, 121);
            pendingConnections.TabIndex = 0;

            acceptPendingConnectionButton.Location = new Point(6, 146);
            acceptPendingConnectionButton.Name = "acceptPendingConnectionButton";
            acceptPendingConnectionButton.Size = new Size(75, 23);
            acceptPendingConnectionButton.TabIndex = 1;
            acceptPendingConnectionButton.Text = "Accept";
            acceptPendingConnectionButton.UseVisualStyleBackColor = true;

            messages.ForeColor = SystemColors.MenuHighlight;
            messages.Location = new Point(445, 74);
            messages.Name = "messages";
            messages.ReadOnly = true;
            messages.Size = new Size(427, 262);
            messages.TabIndex = 3;

            this.message.Location = new Point(440, 343);
            this.message.Multiline = true;
            this.message.Name = "message";
            this.message.Size = new Size(432, 177);
            this.message.TabIndex = 4;

            sendMessageButton.Location = new Point(445, 526);
            sendMessageButton.Name = "sendMessageButton";
            sendMessageButton.Size = new Size(427, 23);
            sendMessageButton.TabIndex = 5;
            sendMessageButton.Text = "Send";
            sendMessageButton.UseVisualStyleBackColor = true;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 562);
            Controls.Add(sendMessageButton);
            Controls.Add(this.message);
            Controls.Add(messages);
            Controls.Add(serverGroupBox);
            Controls.Add(connectionGroupBox);
            Controls.Add(chatroomsGroupBox);
            Name = "ChatForm";
            Text = "SuperChainsaw SharpChat";
            Load += ChatForm_Load;
            FormClosing +=
                delegate(object sender, FormClosingEventArgs e)
                {
                    if (e.CloseReason != CloseReason.UserClosing)
                        return;

                    if (client != null)
                    {
                        var dialogResult = MessageBox.Show("Do you wish to close it before quitting?", "Client still running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);

                        e.Cancel = (dialogResult == DialogResult.Cancel);

                        switch (dialogResult)
                        {
                            case DialogResult.Cancel:
                                return;

                            case DialogResult.Yes:
                                stopClient();
                                break;

                            case DialogResult.No:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    if (server != null)
                    {
                        var dialogResult = MessageBox.Show("Do you wish to shut it down before quitting?\n\nWarning: every chatter will be disconnected and unable to chat.", "Server still running", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);

                        e.Cancel = (dialogResult == DialogResult.Cancel);

                        switch (dialogResult)
                        {
                            case DialogResult.Cancel:
                                return;

                            case DialogResult.Yes:
                                stopServer();
                                break;

                            case DialogResult.No:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                };
            connectedClientsGroupBox.ResumeLayout(false);
            chatroomsGroupBox.ResumeLayout(false);
            newChatroomGroupBox.ResumeLayout(false);
            newChatroomGroupBox.PerformLayout();
            connectionGroupBox.ResumeLayout(false);
            connectionGroupBox.PerformLayout();
            serverGroupBox.ResumeLayout(false);
            serverGroupBox.PerformLayout();
            pendingConnectionsGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        // identations represent layouts
        private GroupBox serverGroupBox;
            private TextBox newServerPort;// private System.Windows.Forms.NumericUpDown newServerPort;
            private Button startStopServerButton;
            private GroupBox pendingConnectionsGroupBox;
                private ListBox pendingConnections;
                private Button acceptPendingConnectionButton;
                private Button rejectPendingConnectionButton;
            private GroupBox connectedClientsGroupBox;
                private ListBox connectedClientsList;
                private Button disconnectClientButton;
                private Button transferServerButton;
        private GroupBox chatroomsGroupBox;
            private GroupBox newChatroomGroupBox;
                private TextBox chatroomName;
                private Button createChatroomButton;
            private ListBox chatroomsList;
            private Button removeChatroom;
        private GroupBox connectionGroupBox;
            private TextBox username;
            private Label atLabel;
            private TextBox serverAddress;
            private Label colonLabel;
            private TextBox serverPort;
            private Button connectDisconnectClientButton;
        private AutoScrollingRichTextBox messages;
        private TextBox message;
        private Button sendMessageButton;

        private Client client;
        private Server server;

        private MessagesWriter messagesWriter;

        public ChatForm()
        {
            InitializeComponent();

            SizeChanged += delegate { placeControls(); };
            MinimumSize = new Size(887, 370);// todo : resolve flickering than occurs without this line or find a way to compute those values dynamically

            client = null;
//            newServerPort.Minimum = 1000;
//            newServerPort.Maximum = 65000;
            newServerPort.Text = "8080";
            serverAddress.Text = "127.0.0.1";
        }

        private void stopServer()
        {
            server.stop();
            server = null;
        }

        private void stopClient()
        {
            chatroomsList.Items.Clear();

            client.stop();
            client = null;

            connectDisconnectClientButton.Enabled = true;
        }

        private void placeControls()
        {// todo : this code works but is still under work (to be finished) because there are still constants in the locations and sizes

            const int innerFormMargin = 13;
            const int interControlMargin = 6;
            const int innerTopGroupBoxMargin = 19;
            const int innerTopGroupBoxEmptyMargin = 12;
            const int innerTopGroupBoxLabelMargin = 22;

            const int groupBoxWidth = 200;
            const int listBoxMinHeight = 17; // warning : this size allows the list box to display only one line, and it can't display anything when it is smaller than that
            const int controlHeight = 23; // todo : only 20 for fields, 23 only for buttons

            var width = ClientSize.Width;
            var height = ClientSize.Height;

            var nextX = innerFormMargin;
            var nextY = innerFormMargin;

            serverGroupBox.Location = new Point(nextX, nextY);
            serverGroupBox.Size = new Size(groupBoxWidth, height - 2 * innerFormMargin);
            nextX += serverGroupBox.Width + interControlMargin;
            {
                var nextServerGroupBoxX = interControlMargin;
                var nextServerGroupBoxY = innerTopGroupBoxMargin;
                newServerPort.Location = new Point(nextServerGroupBoxX, nextServerGroupBoxY);
                newServerPort.Size = new Size(100, 20);
                nextServerGroupBoxX += newServerPort.Width + interControlMargin;
                startStopServerButton.Location = new Point(nextServerGroupBoxX, 17);
                startStopServerButton.Size = new Size(75, 23);
                nextServerGroupBoxX = interControlMargin;
                nextServerGroupBoxY += newServerPort.Height + interControlMargin;
                pendingConnectionsGroupBox.Location = new Point(nextServerGroupBoxX, nextServerGroupBoxY);
                pendingConnectionsGroupBox.Size = new Size(serverGroupBox.Width - 2 * interControlMargin, (serverGroupBox.Height - newServerPort.Height - 3 * interControlMargin - innerTopGroupBoxMargin) * 2 / 7);
                nextServerGroupBoxY += pendingConnectionsGroupBox.Height + interControlMargin;
                {
                    var nextPendingConnectionsGroupBoxX = interControlMargin;
                    var nextPendingConnectionsGroupBoxY = innerTopGroupBoxMargin;
                    pendingConnections.Location = new Point(nextPendingConnectionsGroupBoxX, nextPendingConnectionsGroupBoxY);
                    int pendingConnectionsHeight = pendingConnectionsGroupBox.Height - acceptPendingConnectionButton.Height - 2 * interControlMargin - innerTopGroupBoxMargin;
                    pendingConnections.Size = new Size(pendingConnectionsGroupBox.Width - 2 * interControlMargin, pendingConnectionsHeight);
                    if (pendingConnectionsHeight < listBoxMinHeight)
                        ClientSize = new Size(ClientSize.Width, height + (listBoxMinHeight - pendingConnectionsHeight));
                    nextPendingConnectionsGroupBoxY += pendingConnections.Height + interControlMargin;
                    acceptPendingConnectionButton.Location = new Point(nextPendingConnectionsGroupBoxX, nextPendingConnectionsGroupBoxY);
                    acceptPendingConnectionButton.Size = new Size(75, 23);
                    nextPendingConnectionsGroupBoxX += acceptPendingConnectionButton.Width + interControlMargin;
                    rejectPendingConnectionButton.Location = new Point(nextPendingConnectionsGroupBoxX, nextPendingConnectionsGroupBoxY);
                    rejectPendingConnectionButton.Size = new Size(75, 23);
                }
                connectedClientsGroupBox.Location = new Point(nextServerGroupBoxX, nextServerGroupBoxY);
                connectedClientsGroupBox.Size = new Size(serverGroupBox.Width - 2 * interControlMargin, (serverGroupBox.Height - newServerPort.Height - 3 * interControlMargin - innerTopGroupBoxMargin) * 5 / 7);
                {
                    var nextConnectedClientsGroupBoxX = interControlMargin;
                    var nextConnectedClientsGroupBoxY = innerTopGroupBoxMargin;
                    connectedClientsList.Location = new Point(nextConnectedClientsGroupBoxX, nextConnectedClientsGroupBoxY);
                    connectedClientsList.Size = new Size(connectedClientsGroupBox.Width - 2 * interControlMargin, connectedClientsGroupBox.Height - disconnectClientButton.Height - 2 * interControlMargin - innerTopGroupBoxMargin);
                    nextConnectedClientsGroupBoxY += connectedClientsList.Height + interControlMargin;
                    disconnectClientButton.Location = new Point(nextConnectedClientsGroupBoxX, nextConnectedClientsGroupBoxY);
                    disconnectClientButton.Size = new Size(75, 23);
                    nextConnectedClientsGroupBoxX += disconnectClientButton.Width + interControlMargin;
                    transferServerButton.Location = new Point(nextConnectedClientsGroupBoxX, nextConnectedClientsGroupBoxY);
                    transferServerButton.Size = new Size(95, 23);
                }
            }
            chatroomsGroupBox.Location = new Point(nextX, nextY);
            chatroomsGroupBox.Size = new Size(groupBoxWidth, height - 2 * innerFormMargin);
            nextX += chatroomsGroupBox.Width + interControlMargin;
            {
                int nextChatroomsGroupBoxX = interControlMargin;
                int nextChatroomsGroupBoxY = innerTopGroupBoxEmptyMargin;
                newChatroomGroupBox.Location = new Point(nextChatroomsGroupBoxX, nextChatroomsGroupBoxY);
                newChatroomGroupBox.Size = new Size(187, 72);
                nextChatroomsGroupBoxY += newChatroomGroupBox.Height + interControlMargin;
                {
                    int nextNewChatroomGroupBoxX = interControlMargin;
                    int nextNewChatroomGroupBoxY = innerTopGroupBoxEmptyMargin;
                    chatroomName.Location = new Point(nextNewChatroomGroupBoxX, nextNewChatroomGroupBoxY);
                    chatroomName.Size = new Size(174, 20);
                    nextNewChatroomGroupBoxY += chatroomName.Height + interControlMargin;
                    createChatroomButton.Location = new Point(nextNewChatroomGroupBoxX, nextNewChatroomGroupBoxY);
                    createChatroomButton.Size = new Size(174, 23);
                }
                chatroomsList.Location = new Point(nextChatroomsGroupBoxX, nextChatroomsGroupBoxY);
                chatroomsList.Size = new Size(188, chatroomsGroupBox.Height - newChatroomGroupBox.Height - removeChatroom.Height - innerTopGroupBoxEmptyMargin - 3 * interControlMargin);
                nextChatroomsGroupBoxY += chatroomsList.Height + interControlMargin;
                removeChatroom.Location = new Point(nextChatroomsGroupBoxX, nextChatroomsGroupBoxY);
                removeChatroom.Size = new Size(188, 23);
            }
            var connectionGroupBoxWidth = width - serverGroupBox.Width - chatroomsGroupBox.Width - 2 * interControlMargin - 2 * innerFormMargin;
            connectionGroupBox.Location = new Point(nextX, nextY);
            connectionGroupBox.Size = new Size(connectionGroupBoxWidth, innerTopGroupBoxMargin + controlHeight + interControlMargin);
            nextY += connectionGroupBox.Height + interControlMargin;
            {
                int nextConnectionGroupBoxX = interControlMargin;
                int nextConnectionGroupBoxY = innerTopGroupBoxMargin;
                username.Location = new Point(interControlMargin, innerTopGroupBoxMargin);
                username.Size = new Size(119, 20);
                nextConnectionGroupBoxX += username.Width;
                atLabel.Location = new Point(nextConnectionGroupBoxX, innerTopGroupBoxLabelMargin);
                atLabel.Size = new Size(18, 13);
                nextConnectionGroupBoxX += atLabel.Width;
                serverAddress.Location = new Point(nextConnectionGroupBoxX, innerTopGroupBoxMargin);
                serverAddress.Size = new Size(88, 20);
                nextConnectionGroupBoxX += serverAddress.Width;
                colonLabel.Location = new Point(nextConnectionGroupBoxX, innerTopGroupBoxLabelMargin);
                colonLabel.Size = new Size(10, 13);
                nextConnectionGroupBoxX += colonLabel.Width;
                serverPort.Location = new Point(nextConnectionGroupBoxX, innerTopGroupBoxMargin);
                serverPort.Size = new Size(44, 20);
                nextConnectionGroupBoxX += serverPort.Width + interControlMargin;
                connectDisconnectClientButton.Location = new Point(nextConnectionGroupBoxX, 17);
                var connectDisconnectClientButtonWidth = connectionGroupBoxWidth - username.Width - atLabel.Width - serverAddress.Width - colonLabel.Width - serverPort.Width - 3 * interControlMargin;
                if (connectDisconnectClientButtonWidth < 136)
                    connectDisconnectClientButtonWidth = 136;
                connectDisconnectClientButton.Size = new Size(connectDisconnectClientButtonWidth, 23);
                nextConnectionGroupBoxX += connectDisconnectClientButton.Width + interControlMargin;
                if (connectionGroupBoxWidth < nextConnectionGroupBoxX)
                    ClientSize = new Size(width + (nextConnectionGroupBoxX - connectionGroupBoxWidth), ClientSize.Height);
            }
            messages.Location = new Point(nextX, nextY);
            messages.Size = new Size(connectionGroupBoxWidth, (height - connectionGroupBox.Height - sendMessageButton.Height - 3 * interControlMargin - 2 * innerFormMargin) * 5 / 7);
            nextY += messages.Height + interControlMargin;
            message.Location = new Point(nextX, nextY);
            message.Size = new Size(connectionGroupBoxWidth, (height - connectionGroupBox.Height - sendMessageButton.Height - 3 * interControlMargin - 2 * innerFormMargin) * 2 / 7);
            nextY += message.Height + interControlMargin;
            sendMessageButton.Location = new Point(nextX, nextY);
            sendMessageButton.Size = new Size(connectionGroupBoxWidth, controlHeight);
            nextX += sendMessageButton.Width + interControlMargin;
            nextY += sendMessageButton.Height + interControlMargin;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            messagesWriter = new MessagesWriter();
/*
            // below is a 'random' text for messages area's and RtfWriter's demonstration purposes ; wording of notifications in the final version may differ
            messages.Rtf = messagesWriter.color((int)ColorNames.notification).text("Welcome in the newly created chatroom 'Default'.")
                    .newline().color((int)ColorNames.technicalDetails).text("creator and hosting server: 'arnaud@127.0.0.1:8080'")
                    .newline().newline().color((int)ColorNames.messageHeader).text("arnaud @ 12:47")
                    .newline().color((int)ColorNames.messageContent).text("bonjour, monde !")
                    .newline().newline().color((int)ColorNames.notification).text("'arnaud@127.0.0.1:8080' was successfully connected")
                    .newline().newline().color((int)ColorNames.issueOrBadEnd).text("'arnaud' left the room").RtfText;
*/
        }
    }
}
