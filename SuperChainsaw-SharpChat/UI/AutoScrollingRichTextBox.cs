using System.Windows.Forms;

namespace SuperChainsaw_SharpChat.UI
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
}
