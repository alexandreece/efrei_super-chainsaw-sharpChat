using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    public partial class Form1 : Form
    {
        private Client client;
        private Server server;

        private RtfWriter rtfWriter;
        private Colors TextColors { get; }

        private enum ColorNames
        {// warning: these names must always match the order in which the colors are added to the list
         // (RTF format is such that 0 is always black and the first color in the list is indexed by 1)

            black,
            red,
            green,
            blue
        }

        public Form1()
        {
            InitializeComponent();

            client = null;
            serverAddress.Text = "127.0.0.1";
            TextColors = new Colors().add(new Color(100, 20, 20))// warning: upon modifying this list of colors,
                                     .add(new Color(20, 100, 20))// also modify the enumeration of names so that
                                     .add(new Color(20, 20, 100));// the names still match the colors in the list
        }

        private void startServer()
        {
            new Thread(new Server(4455).start).Start();
        }

        private void connect_Click(object sender, EventArgs e)
        {
            chatrooms.Items.Clear();

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
