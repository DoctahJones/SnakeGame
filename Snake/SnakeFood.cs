using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Particles;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class SnakeFood
    {
        /// <summary>
        /// Array of animations for each of the different types of food.
        /// </summary>
        private Animation[] foodItems;
        /// <summary>
        /// The position in the list of food items of the currently displayed food.
        /// </summary>
        private int activeItem = 0;
        /// <summary>
        /// random used to choose new food item.
        /// </summary>
        private Random random = new Random();

        private ParticleManager particleManager;

        /// <summary>
        /// The size of 1 tile on a screen.
        /// </summary>
        private Point tileSize;
        /// <summary>
        /// The maximum width and height of tiles on the map.
        /// </summary>
        private int maxWidth, maxHeight;
        
        /// <summary>
        /// The position/tile the active food is located in in tile coords. Point is top left corner.
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Whether food is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Constructs a SnakeFood class to store details about the food on the map.
        /// </summary>
        /// <param name="tileSize">Point containing width and height of a tile.</param>
        /// <param name="maxWidth">The number of tiles wide on the screen.</param>
        /// <param name="maxHeight">The number of tiles high on the screen.</param>
        public SnakeFood(ParticleManager particleManager, Point tileSize, int maxWidth, int maxHeight)  {
            this.particleManager = particleManager;
            this.tileSize = tileSize;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// Initialize the SnakeFood class with the textures for the food items.
        /// </summary>
        /// <param name="foodTextures">Array of textures for foods.</param>
        /// <param name="frameWidth">The width of one frame of the animation in the texture.</param>
        /// <param name="frameHeight">The height of one frame in the texture.</param>
        public void Initialize(GraphicsDevice graphicsDevice, Texture2D[] foodTextures, int frameWidth, int frameHeight) 
        {
            if (foodTextures == null || foodTextures.Length == 0)
            {
                throw new ArgumentNullException("No food textures.");
            }
            foodItems = new Animation[foodTextures.Length];
            for (int i = 0; i < foodTextures.Length; i++)
            {
                foodItems[i] = new Animation(graphicsDevice);
                if (frameWidth != frameHeight)
                {
                    throw new ArgumentException("Texture not square.");
                }
                if (foodTextures[i].Height != frameHeight)
                {
                    throw new ArgumentException("Height of texture differs from frame height.");
                }
                if (foodTextures[i].Width % frameWidth != 0)
                {
                    throw new ArgumentException("Width of texture not divisible by frame width.");
                }
                float scale = (float)tileSize.X / (float)frameWidth;

                foodItems[i].Initialize(foodTextures[i], Vector2.Zero, frameWidth, frameHeight, foodTextures[i].Width/ frameWidth, 500, Color.White, scale, true);
            }   
        }

        /// <summary>
        /// Update the SnakeFood each frame.
        /// </summary>
        /// <param name="gameTime">The time passed in the game.</param>
        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                foodItems[activeItem].Position = new Vector2(Position.X * tileSize.X + tileSize.X / 2, Position.Y * tileSize.Y + tileSize.Y / 2);
                foodItems[activeItem].Update(gameTime);
            }
            particleManager.Update(); 
        }

        /// <summary>
        /// Draw the SnakeFood each frame.
        /// </summary>
        /// <param name="spriteBatch">the spritebatch to draw to.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                foodItems[activeItem].Draw(spriteBatch);
            }
            particleManager.Draw(spriteBatch);
        }

        /// <summary>
        /// Method that makes the current food disappear, spawns particles in its place and a new random food item appear at a tile location where there is not currently anything blocking it.
        /// </summary>
        /// <param name="blockedPoints">List of points where a new food item cant be placed.</param>
        public void EatCurrentActiveItem(List<Point> blockedPoints)
        {
            SpawnParticlesOfActiveItem();
            SetNewRandomActiveFood(blockedPoints);
        }

        private void SpawnParticlesOfActiveItem()
        {
            Texture2D particleTexture = foodItems[this.activeItem].GetFrameAsTexture(0);
            Vector2 location = new Vector2(Position.X * tileSize.X + (tileSize.X / 2), Position.Y * tileSize.Y + (tileSize.Y / 2));
            particleManager.GenerateParticles(particleTexture, 7, location, 0.1f, new Vector2(1.3f, 1.3f), 0.3f, 100);
        }

        public void ResetFood(List<Point> blockedPoints)
        {
            SetNewRandomActiveFood(blockedPoints);
            this.particleManager.ClearParticles();
        }

        private void SetNewRandomActiveFood(List<Point> blockedPoints)
        {
            foodItems[activeItem].Active = false;
            activeItem = random.Next(foodItems.Length);
            Position = FindEmptyTile(blockedPoints);
            foodItems[activeItem].Position = new Vector2(Position.X * tileSize.X + tileSize.X / 2, Position.Y * tileSize.Y + tileSize.Y / 2);
            foodItems[activeItem].Active = true;
        }

        
        /// <summary>
        /// Method to find an empty Point where there is nothing already.
        /// </summary>
        /// <param name="blockedPoints">List of points can't put an item.</param>
        /// <returns></returns>
        private Point FindEmptyTile(List<Point> blockedPoints)
        {
            List<Point> available = new List<Point>();
            for (int i = 0; i < maxWidth; i++)
            {
                for (int j = 0; j < maxHeight; j++)
                {
                    Point curr = new Point(i,j);
                    if (!blockedPoints.Contains(curr))
                    {
                        available.Add(curr);
                    }
                }
            }
            if (available.Count < 1)
            {
                throw new Exception("No free places to create new food.");
            }
            int r = random.Next(available.Count);
            return available[r];
        }

    }
}