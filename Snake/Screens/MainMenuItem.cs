using Microsoft.Xna.Framework;
using System;

namespace Snake.Screens
{
    public class MainMenuItem
    {
        public delegate void ClickHandler(MainMenuItem item, EventArgs e);
        public event ClickHandler Click;
        private EventArgs e = null;

        public Rectangle Location { get; set; }
        public string Text { get; private set; }

        public MainMenuItem(string text)
        {
            Text = text;
        }

        public void ItemClicked()
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

    }
}
