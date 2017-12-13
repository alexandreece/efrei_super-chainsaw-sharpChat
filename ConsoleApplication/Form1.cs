using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    public partial class ChatForm : Form
    {
        private Client client;
        private Server server;

        private RtfWriter rtfWriter;
        private Colors TextColors { get; }

        private enum ColorNames
        {// warning: these names must always match the order in which the colors are added to the list
         // (RTF format is such that 0 is always black and the first color in the list is indexed by 1)

            black,
            green,
            darkblue,
            lightblue,
            grey,
            red
        }

        public ChatForm()
        {
            InitializeComponent();
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);

            placeControls();

            client = null;
//            newServerPort.Minimum = 1000;
  //          newServerPort.Maximum = 65000;
            newServerPort.Text = "8080";
            serverAddress.Text = "127.0.0.1";
            TextColors = new Colors().add(new Color(20, 100, 20))// warning: upon modifying this list of colors,
                                     .add(new Color(20, 20, 100))// also modify the enumeration of names so that
                                     .add(new Color(20, 120, 150))// the names still match the colors in the list
                                     .add(new Color(80, 80, 80))
                                     .add(new Color(200, 20, 20));
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {/*
            string str = @"{\rtf1{\colortbl;\red20\green100\blue20;\red20\green20\blue100;\red20\green120\blue150;\red80\green80\blue80;\red200\green20\blue20;}";
            str += @"\cf3 Welcome in the newly created chatroom 'Default'.\par\cf4 creator and hosting server: 'arnaud@127.0.0.1:8080'\par\par\cf2 arnaud @ 12:47\par\cf1 bonjour, monde ! \par\par\cf3 'arnaud@127.0.0.1:8080' was successfully connected\par\par\cf5 'arnaud' left the room";

            messages.Rtf = str;*/
            rtfWriter = new RtfWriter(TextColors);

            // below is a 'random' text for messages area's and RtfWriter's demonstration purposes ; wording of notifications in the final version may differ
            messages.Rtf = rtfWriter.color((int)ColorNames.lightblue).text("Welcome in the newly created chatroom 'Default'.")
                    .newline().color((int)ColorNames.grey).text("creator and hosting server: 'arnaud@127.0.0.1:8080'")
                    .newline().newline().color((int)ColorNames.darkblue).text("arnaud @ 12:47")
                    .newline().color((int)ColorNames.green).text("bonjour, monde !")
                    .newline().newline().color((int)ColorNames.lightblue).text("'arnaud@127.0.0.1:8080' was successfully connected")
                    .newline().newline().color((int)ColorNames.red).text("'arnaud' left the room").RtfText;
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
            rtfWriter = new RtfWriter(TextColors);
            messages.Rtf = rtfWriter.color(0).text("server started").RtfText;
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
            rtfWriter = new RtfWriter(TextColors);
            messages.Rtf = rtfWriter.color((int)ColorNames.green).text("Connected to \"").RtfText;

            try
            {
//                client.Connect(serverAddress.Text, Int32.Parse(serverPort.Text));
                new Thread(Receive).Start();
            }
            catch (Exception)
            {
                rtfWriter = new RtfWriter(TextColors);
                messages.Rtf = rtfWriter.color((int)ColorNames.red).text("Error connecting to \"").RtfText;
            }

            messages.Rtf = rtfWriter.text(serverAddress.Text + ":" + serverPort.Text + "\".").RtfText;
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
