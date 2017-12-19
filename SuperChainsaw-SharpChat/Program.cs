using System;
using System.Windows.Forms;
using SuperChainsaw_SharpChat.UI;

namespace SuperChainsaw_SharpChat
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatForm());
        }
    }
}
