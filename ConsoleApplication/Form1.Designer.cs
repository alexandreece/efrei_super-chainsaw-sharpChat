using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    class AutoScrollingRichTextBox : RichTextBox
    {
        public new string Rtf
        {
            get => base.Rtf;
            set
            {
                base.Rtf = value;
                scrollToBottom();
            }
        }

        private void scrollToBottom()
        {
            SelectionStart = Text.Length;
            ScrollToCaret();
        }
    }

    sealed partial class ChatForm
    {
        private void InitializeComponent()
        {
            this.connectedClientsGroupBox = new System.Windows.Forms.GroupBox();
            this.transferServerButton = new System.Windows.Forms.Button();
            this.disconnectClientButton = new System.Windows.Forms.Button();
            this.connectedClientsList = new System.Windows.Forms.ListBox();
            this.ChatroomsGroupBox = new System.Windows.Forms.GroupBox();
            this.removeChatroom = new System.Windows.Forms.Button();
            this.newChatroomGroupBox = new System.Windows.Forms.GroupBox();
            this.createChatroomButton = new System.Windows.Forms.Button();
            createChatroomButton.Click +=
                delegate(object sender, EventArgs args)
                {
                    client.createChatroom(chatroomName.Text);
                };
            this.chatroomName = new System.Windows.Forms.TextBox();
            this.chatroomsList = new System.Windows.Forms.ListBox();
            chatroomsList.SelectedIndexChanged +=
                delegate(object sender, EventArgs args)
                {
                    client.joinChatroom(chatroomsList.SelectedItem.ToString());
                };
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.colonLabel = new System.Windows.Forms.Label();
            this.atLabel = new System.Windows.Forms.Label();
            this.connectDisconnectClientButton = new System.Windows.Forms.Button();
            this.connectDisconnectClientButton.Click +=
                delegate(object o, EventArgs args)
                {
                    if (client == null)
                    {
                        client = new Client(serverAddress.Text, int.Parse(serverPort.Text));// Client(address, (int)serverPort.Value);
                        client.Connecting +=
                            delegate(string hostname, int port)
                            {
                                client.sendUsername(username.Text);
                            };
                        client.Connected +=
                            delegate(string hostname, int port)
                            {
                                messagesWriter = new MessagesWriter();
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("client connected", hostname + ":" + port).RtfText));
                            };
                        client.Pending +=
                            delegate(string hostname, int port)
                            {
                                messagesWriter = new MessagesWriter();
                                messages.Invoke(new Action(() => messages.Rtf = messagesWriter.notify("client pending", hostname + ":" + port).RtfText));
                            };
                        client.UsernameCannotBeEmpty +=
                            delegate(string hostname, int port)
                            {
                                messagesWriter = new MessagesWriter();
                                messages.Rtf = messagesWriter.notify("connection failed", "username cannot be empty").RtfText;

                                client.stop();
                                client = null;
                            };
                        client.UsernameAlreadyTaken +=
                            delegate(string hostname, int port)
                            {
                                messagesWriter = new MessagesWriter();
                                messages.Rtf = messagesWriter.notify("connection failed", "username already taken").RtfText;

                                client.stop();
                                client = null;
                            };
                        client.ServerChatroomsList +=
                            delegate(List<string> serverChatroomsList)
                            {
                                chatroomsList.Items.Clear();
                                foreach (var serverChatroom in serverChatroomsList)
                                    chatroomsList.Items.Add(serverChatroom);
                            };
                        client.ChatroomMessageAppended +=
                            delegate(ChatroomMessageAppended chatroomMessageAppended)
                            {
                                var username = chatroomMessageAppended.Username;
                                DateTime dateAdded = chatroomMessageAppended.DateAdded;
                                var message = chatroomMessageAppended.Message;

                                messages.Rtf = messagesWriter.usernameAtDate(username, dateAdded)
                                                             .message(message).RtfText;
                            };
                        new Thread(client.start).Start();
                    }
                    else
                    {
                        client.stop();
                        client = null;
                    }
                };
            this.serverPort = new System.Windows.Forms.TextBox();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.serverGroupBox = new System.Windows.Forms.GroupBox();
            this.startStopServerButton = new System.Windows.Forms.Button();
            this.startStopServerButton.Click += startStopServer;
            this.newServerPort = new System.Windows.Forms.TextBox();// this.newServerPort = new System.Windows.Forms.NumericUpDown();
            this.pendingConnectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.rejectPendingConnectionButton = new System.Windows.Forms.Button();
            this.pendingConnections = new System.Windows.Forms.ListBox();
            this.acceptPendingConnectionButton = new System.Windows.Forms.Button();
            acceptPendingConnectionButton.Click +=
                delegate(object sender, EventArgs args)
                {
                    server.acceptConnection(pendingConnections.SelectedItem);
                };
            this.messages = new AutoScrollingRichTextBox();
            this.message = new System.Windows.Forms.TextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            sendMessageButton.Click +=
                delegate(object sender, EventArgs args)
                {
                    foreach (var message in message.Lines)
                        if (message.Length > 0)
                            client.sendMessage(message);
                    message.Clear();
                };
            this.connectedClientsGroupBox.SuspendLayout();
            this.ChatroomsGroupBox.SuspendLayout();
            this.newChatroomGroupBox.SuspendLayout();
            this.connectionGroupBox.SuspendLayout();
            this.serverGroupBox.SuspendLayout();
            this.pendingConnectionsGroupBox.SuspendLayout();
            this.SuspendLayout();

            this.connectedClientsGroupBox.Controls.Add(this.transferServerButton);
            this.connectedClientsGroupBox.Controls.Add(this.disconnectClientButton);
            this.connectedClientsGroupBox.Controls.Add(this.connectedClientsList);
            this.connectedClientsGroupBox.Location = new System.Drawing.Point(8, 237);
            this.connectedClientsGroupBox.Name = "connectedClientsGroupBox";
            this.connectedClientsGroupBox.Size = new System.Drawing.Size(200, 294);
            this.connectedClientsGroupBox.TabIndex = 0;
            this.connectedClientsGroupBox.TabStop = false;
            this.connectedClientsGroupBox.Text = "Connected clients";

            this.transferServerButton.Location = new System.Drawing.Point(87, 265);
            this.transferServerButton.Name = "transferServerButton";
            this.transferServerButton.Size = new System.Drawing.Size(104, 23);
            this.transferServerButton.TabIndex = 2;
            this.transferServerButton.Text = "Transfer server to";
            this.transferServerButton.UseVisualStyleBackColor = true;

            this.disconnectClientButton.Location = new System.Drawing.Point(6, 265);
            this.disconnectClientButton.Name = "disconnectClientButton";
            this.disconnectClientButton.Size = new System.Drawing.Size(75, 23);
            this.disconnectClientButton.TabIndex = 1;
            this.disconnectClientButton.Text = "Disconnect";
            this.disconnectClientButton.UseVisualStyleBackColor = true;

            this.connectedClientsList.FormattingEnabled = true;
            this.connectedClientsList.Location = new System.Drawing.Point(7, 19);
            this.connectedClientsList.Name = "connectedClientsList";
            this.connectedClientsList.Size = new System.Drawing.Size(187, 238);
            this.connectedClientsList.TabIndex = 0;

            this.ChatroomsGroupBox.Controls.Add(this.removeChatroom);
            this.ChatroomsGroupBox.Controls.Add(this.newChatroomGroupBox);
            this.ChatroomsGroupBox.Controls.Add(this.chatroomsList);
            this.ChatroomsGroupBox.Location = new System.Drawing.Point(233, 17);
            this.ChatroomsGroupBox.Name = "ChatroomsGroupBox";
            this.ChatroomsGroupBox.Size = new System.Drawing.Size(200, 533);
            this.ChatroomsGroupBox.TabIndex = 0;
            this.ChatroomsGroupBox.TabStop = false;
            this.ChatroomsGroupBox.Text = "Active chatrooms";

            this.removeChatroom.Location = new System.Drawing.Point(6, 498);
            this.removeChatroom.Name = "removeChatroom";
            this.removeChatroom.Size = new System.Drawing.Size(188, 23);
            this.removeChatroom.TabIndex = 2;
            this.removeChatroom.Text = "Remove";
            this.removeChatroom.UseVisualStyleBackColor = true;

            this.newChatroomGroupBox.Controls.Add(this.createChatroomButton);
            this.newChatroomGroupBox.Controls.Add(this.chatroomName);
            this.newChatroomGroupBox.Location = new System.Drawing.Point(7, 20);
            this.newChatroomGroupBox.Name = "newChatroomGroupBox";
            this.newChatroomGroupBox.Size = new System.Drawing.Size(187, 72);
            this.newChatroomGroupBox.TabIndex = 1;
            this.newChatroomGroupBox.TabStop = false;

            this.createChatroomButton.Location = new System.Drawing.Point(7, 37);
            this.createChatroomButton.Name = "createChatroomButton";
            this.createChatroomButton.Size = new System.Drawing.Size(174, 23);
            this.createChatroomButton.TabIndex = 1;
            this.createChatroomButton.Text = "Create chatroom";
            this.createChatroomButton.UseVisualStyleBackColor = true;

            this.chatroomName.Location = new System.Drawing.Point(7, 11);
            this.chatroomName.Name = "chatroomName";
            this.chatroomName.Size = new System.Drawing.Size(174, 20);
            this.chatroomName.TabIndex = 0;

            this.chatroomsList.FormattingEnabled = true;
            this.chatroomsList.Location = new System.Drawing.Point(6, 98);
            this.chatroomsList.Name = "chatroomsList";
            this.chatroomsList.Size = new System.Drawing.Size(188, 394);
            this.chatroomsList.TabIndex = 0;

            this.connectionGroupBox.Controls.Add(this.colonLabel);
            this.connectionGroupBox.Controls.Add(this.atLabel);
            this.connectionGroupBox.Controls.Add(this.connectDisconnectClientButton);
            this.connectionGroupBox.Controls.Add(this.serverPort);
            this.connectionGroupBox.Controls.Add(this.serverAddress);
            this.connectionGroupBox.Controls.Add(this.username);
            this.connectionGroupBox.Location = new System.Drawing.Point(439, 17);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(433, 51);
            this.connectionGroupBox.TabIndex = 1;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "username @ address : port";

            this.colonLabel.AutoSize = true;
            this.colonLabel.Location = new System.Drawing.Point(231, 22);
            this.colonLabel.Name = "colonLabel";
            this.colonLabel.Size = new System.Drawing.Size(10, 13);
            this.colonLabel.TabIndex = 5;
            this.colonLabel.Text = ":";

            this.atLabel.AutoSize = true;
            this.atLabel.Location = new System.Drawing.Point(125, 22);
            this.atLabel.Name = "atLabel";
            this.atLabel.Size = new System.Drawing.Size(18, 13);
            this.atLabel.TabIndex = 4;
            this.atLabel.Text = "@";

            this.connectDisconnectClientButton.Location = new System.Drawing.Point(291, 17);
            this.connectDisconnectClientButton.Name = "connectDisconnectClientButton";
            this.connectDisconnectClientButton.Size = new System.Drawing.Size(136, 23);
            this.connectDisconnectClientButton.TabIndex = 3;
            this.connectDisconnectClientButton.Text = "Connect / Disconnect";
            this.connectDisconnectClientButton.UseVisualStyleBackColor = true;

            this.serverPort.Location = new System.Drawing.Point(241, 19);
            this.serverPort.Name = "serverPort";
            this.serverPort.Size = new System.Drawing.Size(44, 20);
            this.serverPort.TabIndex = 2;

            this.serverAddress.Location = new System.Drawing.Point(143, 19);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(88, 20);
            this.serverAddress.TabIndex = 1;
            this.serverAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.username.Location = new System.Drawing.Point(6, 19);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(119, 20);
            this.username.TabIndex = 0;
            this.username.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.serverGroupBox.Controls.Add(this.startStopServerButton);
            this.serverGroupBox.Controls.Add(this.newServerPort);
            this.serverGroupBox.Controls.Add(this.pendingConnectionsGroupBox);
            this.serverGroupBox.Controls.Add(this.connectedClientsGroupBox);
            this.serverGroupBox.Location = new System.Drawing.Point(13, 13);
            this.serverGroupBox.Name = "serverGroupBox";
            this.serverGroupBox.Size = new System.Drawing.Size(214, 537);
            this.serverGroupBox.TabIndex = 2;
            this.serverGroupBox.TabStop = false;
            this.serverGroupBox.Text = "Server";

            this.startStopServerButton.Location = new System.Drawing.Point(112, 17);
            this.startStopServerButton.Name = "startStopServerButton";
            this.startStopServerButton.Size = new System.Drawing.Size(75, 23);
            this.startStopServerButton.TabIndex = 2;
            this.startStopServerButton.Text = "Start / Stop";
            this.startStopServerButton.UseVisualStyleBackColor = true;

            this.newServerPort.Location = new System.Drawing.Point(6, 19);
            this.newServerPort.Name = "newServerPort";
            this.newServerPort.Size = new System.Drawing.Size(100, 20);
            this.newServerPort.TabIndex = 1;

            this.pendingConnectionsGroupBox.Controls.Add(this.rejectPendingConnectionButton);
            this.pendingConnectionsGroupBox.Controls.Add(this.pendingConnections);
            this.pendingConnectionsGroupBox.Controls.Add(this.acceptPendingConnectionButton);
            this.pendingConnectionsGroupBox.Location = new System.Drawing.Point(6, 46);
            this.pendingConnectionsGroupBox.Name = "pendingConnectionsGroupBox";
            this.pendingConnectionsGroupBox.Size = new System.Drawing.Size(200, 185);
            this.pendingConnectionsGroupBox.TabIndex = 0;
            this.pendingConnectionsGroupBox.TabStop = false;
            this.pendingConnectionsGroupBox.Text = "Pending connections";

            this.rejectPendingConnectionButton.Location = new System.Drawing.Point(89, 146);
            this.rejectPendingConnectionButton.Name = "rejectPendingConnectionButton";
            this.rejectPendingConnectionButton.Size = new System.Drawing.Size(75, 23);
            this.rejectPendingConnectionButton.TabIndex = 2;
            this.rejectPendingConnectionButton.Text = "Reject";
            this.rejectPendingConnectionButton.UseVisualStyleBackColor = true;

            this.pendingConnections.FormattingEnabled = true;
            this.pendingConnections.Location = new System.Drawing.Point(6, 19);
            this.pendingConnections.Name = "pendingConnections";
            this.pendingConnections.Size = new System.Drawing.Size(187, 121);
            this.pendingConnections.TabIndex = 0;

            this.acceptPendingConnectionButton.Location = new System.Drawing.Point(6, 146);
            this.acceptPendingConnectionButton.Name = "acceptPendingConnectionButton";
            this.acceptPendingConnectionButton.Size = new System.Drawing.Size(75, 23);
            this.acceptPendingConnectionButton.TabIndex = 1;
            this.acceptPendingConnectionButton.Text = "Accept";
            this.acceptPendingConnectionButton.UseVisualStyleBackColor = true;

            this.messages.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.messages.Location = new System.Drawing.Point(445, 74);
            this.messages.Name = "messages";
            this.messages.ReadOnly = true;
            this.messages.Size = new System.Drawing.Size(427, 262);
            this.messages.TabIndex = 3;
            this.messages.Text = "arnaud@127.0.0.1:8080 was successfully connected";

            this.message.Location = new System.Drawing.Point(440, 343);
            this.message.Multiline = true;
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(432, 177);
            this.message.TabIndex = 4;

            this.sendMessageButton.Location = new System.Drawing.Point(445, 526);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(427, 23);
            this.sendMessageButton.TabIndex = 5;
            this.sendMessageButton.Text = "Send";
            this.sendMessageButton.UseVisualStyleBackColor = true;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.sendMessageButton);
            this.Controls.Add(this.message);
            this.Controls.Add(this.messages);
            this.Controls.Add(this.serverGroupBox);
            this.Controls.Add(this.connectionGroupBox);
            this.Controls.Add(this.ChatroomsGroupBox);
            this.Name = "ChatForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(closing);
            this.connectedClientsGroupBox.ResumeLayout(false);
            this.ChatroomsGroupBox.ResumeLayout(false);
            this.newChatroomGroupBox.ResumeLayout(false);
            this.newChatroomGroupBox.PerformLayout();
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.serverGroupBox.ResumeLayout(false);
            this.serverGroupBox.PerformLayout();
            this.pendingConnectionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox connectedClientsGroupBox;
        private System.Windows.Forms.ListBox connectedClientsList;
        private System.Windows.Forms.GroupBox ChatroomsGroupBox;
        private System.Windows.Forms.ListBox chatroomsList;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.Label colonLabel;
        private System.Windows.Forms.Label atLabel;
        private System.Windows.Forms.Button connectDisconnectClientButton;
        private System.Windows.Forms.TextBox serverPort;
        private System.Windows.Forms.TextBox serverAddress;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.GroupBox serverGroupBox;
        private System.Windows.Forms.GroupBox pendingConnectionsGroupBox;
        private System.Windows.Forms.Button rejectPendingConnectionButton;
        private System.Windows.Forms.ListBox pendingConnections;
        private System.Windows.Forms.Button acceptPendingConnectionButton;
        private System.Windows.Forms.Button transferServerButton;
        private System.Windows.Forms.Button disconnectClientButton;
        private System.Windows.Forms.Button startStopServerButton;
        private System.Windows.Forms.TextBox newServerPort;// private System.Windows.Forms.NumericUpDown newServerPort;
        private System.Windows.Forms.Button removeChatroom;
        private System.Windows.Forms.GroupBox newChatroomGroupBox;
        private System.Windows.Forms.Button createChatroomButton;
        private System.Windows.Forms.TextBox chatroomName;
        private AutoScrollingRichTextBox messages;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.Button sendMessageButton;
    }
}
