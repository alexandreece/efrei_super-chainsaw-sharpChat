using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    public partial class Form1 : Form
    {
        TcpClient client;

        public Form1()
        {
            InitializeComponent();

            client = null;
            serverAddress.Text = "127.0.0.1";
        }

        private void connect_Click(object sender, EventArgs e)
        {
            chatrooms.Items.Clear();

            if (client == null)
                client = new TcpClient();
            messages.Text = "Connected to \"";

            try
            {
                client.Connect(serverAddress.Text, Int32.Parse(serverPort.Text));
                new Thread(Receive).Start();
            }
            catch (Exception)
            {
                messages.Text = "Error connecting to \"";
            }

            messages.Text += serverAddress.Text + ":" + serverPort.Text + "\".";
        }

        private void envoyer_Click(object sender, EventArgs e)
        {
            Byte[] message = System.Text.Encoding.ASCII.GetBytes(this.message.Text);
            client.GetStream().Write(message, 0, message.Length);
        }

        private void Receive()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Byte[] message = new Byte[256];
                client.GetStream().Read(message, 0, message.Length);

                this.message.Text += System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
            }
        }
    }
}
