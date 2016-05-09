using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Screens
{
    public class TiledGameScreen : IScreen
    {
        /// <summary>
        /// The screen manager that handles this screen.
        /// </summary>
        private ScreenManager screenManager;
        /// <summary>
        /// 2D array of the cells of the screen.[x][y] starting at 0,0 top left.
        /// </summary>
        private TiledGameScreenCell[,] cells;

        /// <summary>
        /// The size of each of the tiles.
        /// </summary>
        public Point TileSize { get; private set; }
        /// <summary>
        /// The number of tiles wide and high the screen is.
        /// </summary>
        public Point ScreenSizeInTiles { get; private set; }
        /// <summary>
        /// The name used to identify this screen.
        /// </summary>
        public string ScreenName { get; private set; }
        /// <summary>
        /// Whether the current screen is active or not.
        /// </summary>
        public bool Active { get; set; }

        public TiledGameScreen(ScreenManager screenManager, string name)
        {
            this.screenManager = screenManager;
            ScreenName = name;
            Active = false;
        }

        public void Initialize(TiledGameScreenCell[,] cellData)
        {
            if (cellData == null)
            {
                throw new ArgumentNullException("Cell data is null.");
            }
            cells = cellData;
            ScreenSizeInTiles = new Point(cellData.GetLength(0), cellData.GetLength(1));
            TileSize = new Point(screenManager.ScreenWidth / ScreenSizeInTiles.X, screenManager.ScreenHeight / ScreenSizeInTiles.Y);

        }


        public virtual void Update(GameTime gameTime, InputState inputState)
        {
            return;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j] != null)
                    {
                        Rectangle destRec = new Rectangle((int)TileSize.X * i + (TileSize.X / 2), (int)TileSize.Y * j + (TileSize.Y / 2), (int)TileSize.X, (int)TileSize.Y);
                        Vector2 origin = cells[i, j].Texture.Bounds.Center.ToVector2();
                        spriteBatch.Draw(cells[i, j].Texture, destRec, null, cells[i, j].TileColor, cells[i, j].Rotation, origin, SpriteEffects.None, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Method to get the details about a specific cell on the map by using its coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the cell we want to get details about, this is the horizontal coord, with 0 on the left.</param>
        /// <param name="y">The y coordinate of the cell we want to get details about, this is the vertical coord, with 0 on the top.</param>
        /// <returns>A TiledGameScreenCell object with details about the cell.</returns>
        public TiledGameScreenCell GetCellInfo(int x, int y)
        {
            if (x > ScreenSizeInTiles.X || x < 0)
            {
                throw new ArgumentOutOfRangeException(x + " is not a valid value of x.");
            }
            if (y > ScreenSizeInTiles.Y || y < 0)
            {
                throw new ArgumentOutOfRangeException(y + " is not a valid value of y.");
            }
            return cells[x, y];
        }


    }
}
