using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace super_chainsaw_sharpChatClient
{
    class Layout : Control
    {
        private List<KeyValuePair<Control, int> > controls = new List<KeyValuePair<Control, int> >();
        private Point startingPosition;
        private int totalLength;
        private int interControlMargin = 6;
        private bool horizontalLayout;
        private int fixedControlsLength = 0;
        private int marginControlsWeight = 0;

        public Layout(bool horizontalLayout)
        {
            this.horizontalLayout = horizontalLayout;
        }

        public Layout(int totalLength, bool horizontalLayout)
        {
            this.totalLength = totalLength;
            this.horizontalLayout = horizontalLayout;
        }

        public Layout add(Control control, int weight)
        {
            controls.Add(new KeyValuePair<Control, int>(control, weight));

            if (horizontalLayout)
            {
                if (Height < control.Height)
                    Height = control.Height;
            }
            else
            {
                if (Width < control.Width)
                    Width = control.Width;
            }

            marginControlsWeight += weight;

            if (weight == 0)
            {
                if (horizontalLayout)
                    fixedControlsLength += control.Width;
                else fixedControlsLength += control.Height;

                fixedControlsLength += interControlMargin;
            }

            return this;
        }

        public Layout placeAll()
        {
            Point nextPosition = startingPosition;

            int margin = totalLength - fixedControlsLength;
            foreach (KeyValuePair<Control, int> control in controls)
            {
                control.Key.Location = nextPosition;

                if (horizontalLayout)
                {
                    if (control.Value != 0)
                        control.Key.Width = margin * control.Value / marginControlsWeight;

                    if (control.Key is Layout layout)
                    {
                        layout.totalLength = control.Key.Width;
                        layout.placeAll();
                    }

                    int width = control.Key.Width + interControlMargin;
                    nextPosition += new Size(width, 0);

                    Width += width;
                }
                else
                {
                    if (control.Value != 0)
                        control.Key.Height = margin * control.Value / marginControlsWeight;

                    if (control.Key is Layout layout)
                    {
                        layout.totalLength = control.Key.Height;
                        layout.placeAll();
                    }

                    int height = control.Key.Height + interControlMargin;
                    nextPosition += new Size(0, height);

                    Height += height;
                }
            }

            return this;
        }
    }
}
