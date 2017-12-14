using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    public sealed partial class ChatForm : Form
    {
        private Client client;
        private Server server;

        private MessagesWriter messagesWriter;

        public ChatForm()
        {
            InitializeComponent();
            Text = "SuperChainsaw SharpChat";

            placeControls();
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);

            client = null;
//            newServerPort.Minimum = 1000;
  //          newServerPort.Maximum = 65000;
            newServerPort.Text = "8080";
            serverAddress.Text = "127.0.0.1";
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

        private void closing(object sender, FormClosingEventArgs e)
        {
            // todo : check if serveur or client are running and warn
        }

        private void startStopServer(object sender, EventArgs e)
        {
            if (server == null)
            {
                server = new Server(int.Parse(newServerPort.Text));// server = new Server((int)newServerPort.Value);
                server.Started += serverStarted;
                server.ChatroomAdded +=
                    delegate(Chatroom chatroom)
                    {
                        chatroomsList.Items.Add(chatroom);
                    };
                new Thread(server.start).Start();
            }
            else
            {
                server.stop();
                server = null;
            }
        }

        private void serverStarted()
        {
            messagesWriter = new MessagesWriter();
            messages.Rtf = messagesWriter.color(0).text("server started").RtfText;

            if (serverPort.Text == "")
                serverPort.Text = newServerPort.Text;// todo : (mostly) for the sake of ideology, replace set value with actual port declared by server in some event parameter
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            placeControls();
        }

        private void placeControls()
        {
            // todo : place here the lines that place and size the controls
            // todo : + resize appropriate controls according to new form size
            // todo : + add conditions on minimum height and width and resize form if it became too small
        }

        private void connect_Click(object sender, EventArgs e)
        {
//            chatrooms.Items.Clear();

            if (client == null)
                client = new Client("127.0.0.1", 4455);
            new Thread(client.start).Start();
            messagesWriter = new MessagesWriter();
//            messages.Rtf = messagesWriter.color((int)ColorNames.green).text("Connected to \"").RtfText;

            try
            {
//                client.Connect(serverAddress.Text, Int32.Parse(serverPort.Text));
                new Thread(Receive).Start();
            }
            catch (Exception)
            {
                messagesWriter = new MessagesWriter();
//                messages.Rtf = messagesWriter.color((int)ColorNames.red).text("Error connecting to \"").RtfText;
            }

            messages.Rtf = messagesWriter.text(serverAddress.Text + ":" + serverPort.Text + "\".").RtfText;
        }

        private void envoyer_Click(object sender, EventArgs e)
        {
            var message = System.Text.Encoding.ASCII.GetBytes(this.message.Text);
//            client.GetStream().Write(message, 0, message.Length);
        }

        private void Receive()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                var message = new Byte[256];
//                client.GetStream().Read(message, 0, message.Length);

                this.message.Text += System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
            }
        }
    }
}
