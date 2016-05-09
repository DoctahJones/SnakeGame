using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class FontDisplayInfo
    {

        public Color ColorNormal { get; set; }
        public Color ColorSelected { get; set; }
        public SpriteFont FontNormal { get; set; }
        public SpriteFont FontSelected { get; set; }

        public FontDisplayInfo(SpriteFont normalFont, Color normalColor)
        {
            FontNormal = normalFont;
            ColorNormal = normalColor;
        }


    }
}
