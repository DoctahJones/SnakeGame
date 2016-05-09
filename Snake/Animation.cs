using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Animation
    {
        private GraphicsDevice graphicsDevice;

        // Image strip to make up animation.
        private Texture2D spriteStrip;
        // How many frames since last time we updated the frame of animation.
        private int elapsedTime;
        // How Long a single frame is displayed.
        private int frameTime;
        // How many frames in the image / Texture2D.
        private int frameCount;
        // The current frame being displayed.
        private int currentFrame;

        // Color of animation to display.
        private Color color;

        // The rectangle of the spritestrip to be displayed in the current frame of animation.
        private Rectangle sourceRect = new Rectangle();
        // The rectangle of the display we want to draw the current frame of animation to.
        private Rectangle destinationRect = new Rectangle();

        // Width of a single frame in spritesheet.
        public int FrameWidth { get; set; }
        // Height of a single frame in spritesheet.
        public int FrameHeight { get; set; }
        // The scale the frame will be drawn at.
        public float Scale { get; set; }
        /// <summary>
        /// The rotation of the animation.
        /// </summary>
        public float Rotation { get; set; }
        // Whether to keep animating through the frames of the animation.
        public bool Active { get; set; }
        // Whether or not to loop back to the beginning of the animation once it has completed.
        public bool Looping { get; set; }
        //The position the animation is drawn (the origin of the drawn rectangle).
        public Vector2 Position { get; set; }

        public Animation(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Initialize(Texture2D spritestrip, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            Scale = scale;
            Looping = looping;
            Position = position;
            this.spriteStrip = spritestrip;
            // Set time and current frame to 0.
            elapsedTime = 0;
            currentFrame = 0;
            Active = true;
            Rotation = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (Active == false) return;

            // Update the elapsed time
            this.elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }
                // Reset the elapsed time to zero
                elapsedTime = 0;
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            destinationRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(FrameWidth * Scale), (int)(FrameHeight * Scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                //spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
                Vector2 origin = new Vector2(FrameWidth / 2, FrameHeight / 2);
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color, Rotation, origin, SpriteEffects.None, 1);
            }
        }

        public Texture2D GetFrameAsTexture(int frameNumber)
        {
            if (frameNumber > frameCount)
            {
                throw new ArgumentOutOfRangeException("The frame number requested is larger than the number of frames in the animation.");
            }

            Rectangle sourceRectangle = new Rectangle(0, 0, FrameWidth, FrameHeight);

            Texture2D cropTexture = new Texture2D(graphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            spriteStrip.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }

    }
}
