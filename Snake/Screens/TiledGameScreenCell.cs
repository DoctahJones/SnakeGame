using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake.Screens
{
    public class TiledGameScreenCell
    {
        /// <summary>
        /// Whether the tile can be passed over.
        /// </summary>
        public bool Pathable { get; private set; }
        /// <summary>
        /// The texture of the tile.
        /// </summary>
        public Texture2D Texture { get; private set; }
        /// <summary>
        /// The color of the tile.
        /// </summary>
        public Color TileColor { get; set; }
        /// <summary>
        /// The rotation of the texture of the tile.
        /// </summary>
        public float Rotation { get; set; }

        public TiledGameScreenCell(Texture2D texture, bool pathable, Color tileColor, float rotation)
        {
            Pathable = pathable;
            Texture = texture;
            TileColor = tileColor;
            Rotation = rotation;
        }
    }
}
