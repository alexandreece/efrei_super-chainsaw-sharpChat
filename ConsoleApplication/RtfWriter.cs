using System.Collections.Generic;

namespace super_chainsaw_sharpChatClient
{
    internal class Color
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    internal class Colors : List<Color>
    {
        public Colors add(Color color)
        {
            Add(color);

            return this;
        }
    }

    internal class RtfWriter
    {
        public string RtfText { get; private set; }

        public RtfWriter(Colors colors)
        {
            RtfText = @"{\rtf1{\colortbl;";
            foreach (var color in colors)
                RtfText += @"\red" + color.R
                         + @"\green" + color.G
                         + @"\blue" + color.B
                         + ";";
            RtfText += "}";
        }

        public RtfWriter color(int colorIndex)
        {
            RtfText += @"\cf" + colorIndex + " ";

            return this;
        }

        public RtfWriter fontSize(int fontSize)
        {
            RtfText += @"\fs" + fontSize + " ";

            return this;
        }

        public RtfWriter newline()
        {
            RtfText += @"\par ";

            return this;
        }

        public RtfWriter text(string text)
        {
            RtfText += text.Replace(@"\", @"\\");

            return this;
        }
    }
}
