using System.Collections.Generic;

namespace SuperChainsaw_SharpChat.UI
{
    internal class Colors : List<Color>
    {
        public Colors add(Color color)
        {
            Add(color);

            return this;
        }
    }
}
