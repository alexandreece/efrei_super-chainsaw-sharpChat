namespace super_chainsaw_sharpChatClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        private void InitializeComponent()
        {
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.messages = new System.Windows.Forms.RichTextBox();
            this.message = new System.Windows.Forms.TextBox();
            this.send = new System.Windows.Forms.Button();
            this.chatrooms = new System.Windows.Forms.ListBox();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.serverPort = new System.Windows.Forms.TextBox();
            this.connect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.messages.Location = new System.Drawing.Point(120, 41);
            this.messages.Name = "messages";
            this.messages.Size = new System.Drawing.Size(152, 183);
            this.messages.TabIndex = 0;
            this.messages.Text = "";

            this.message.Location = new System.Drawing.Point(13, 230);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(177, 20);
            this.message.TabIndex = 1;

            this.send.Location = new System.Drawing.Point(196, 227);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(75, 23);
            this.send.TabIndex = 2;
            this.send.Text = "Send";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.envoyer_Click);

            this.chatrooms.FormattingEnabled = true;
            this.chatrooms.Location = new System.Drawing.Point(14, 64);
            this.chatrooms.Name = "chatrooms";
            this.chatrooms.Size = new System.Drawing.Size(100, 160);
            this.chatrooms.TabIndex = 3;

            this.serverAddress.Location = new System.Drawing.Point(51, 13);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(88, 20);
            this.serverAddress.TabIndex = 4;

            this.serverPort.Location = new System.Drawing.Point(145, 13);
            this.serverPort.Name = "serverPort";
            this.serverPort.Size = new System.Drawing.Size(45, 20);
            this.serverPort.TabIndex = 5;

            this.connect.Location = new System.Drawing.Point(196, 12);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 6;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);

            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Available chatrooms:";

            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Server:";

            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(137, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = ":";

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.serverPort);
            this.Controls.Add(this.serverAddress);
            this.Controls.Add(this.chatrooms);
            this.Controls.Add(this.send);
            this.Controls.Add(this.message);
            this.Controls.Add(this.messages);
            this.Name = "Form1";
            this.Text = "Super Chainsaw Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.RichTextBox messages;
        private System.Windows.Forms.TextBox message;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.ListBox chatrooms;
        private System.Windows.Forms.TextBox serverAddress;
        private System.Windows.Forms.TextBox serverPort;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
