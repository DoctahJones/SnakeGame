using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake.Particles
{
    public class Particle
    {
        private Texture2D particleTexture;
        private Vector2 position;
        private float rotation;
        private float rotationPerUpdate;
        private Vector2 movementPerUpdate;
        private float scale;
        private Color color;

        private Rectangle textureSourceRectangle;
        private Vector2 textureOrigin;

        public int TimeToLive { get; private set; }

        public Particle(Texture2D texture, Vector2 position, float rotation, float rotationPerUpdate, Vector2 movementPerUpdate, float scale, int timeToLive, Color color)
        {
            this.particleTexture = texture;
            this.position = position;
            this.rotation = rotation;
            this.rotationPerUpdate = rotationPerUpdate;
            this.movementPerUpdate = movementPerUpdate;
            this.scale = scale;
            this.TimeToLive = timeToLive;
            this.color = color;

            this.textureSourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            this.textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update()
        {
            TimeToLive--;
            this.position += this.movementPerUpdate;
            rotation += rotationPerUpdate;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(particleTexture, position, textureSourceRectangle, color, rotation, textureOrigin, scale, SpriteEffects.None, 1f);
        }
    }
}
