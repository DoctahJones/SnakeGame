using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Particles
{
    public class ParticleManager
    {
        private List<Particle> particles;



        public ParticleManager()
        {
            particles = new List<Particle>();
        }

        public void Update()
        {
            for (int i = (particles.Count - 1); i >= 0; i--)
            {
                particles.ElementAt(i).Update();
                if (particles.ElementAt(i).TimeToLive < 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
        }


       
        public void GenerateParticles(Texture2D texture, int numberToSpawn, Vector2 location, float maxRotationPerUpdate, Vector2 maxMovementPerUpdate, float scale, int maxTTL)
        {
            Random r = new Random();

            for (int i = 0; i < numberToSpawn; i++)
            {
                float rotationOnUpdate = maxRotationPerUpdate * (float)(r.NextDouble() * 2f - 1f);
                float initialRotation = (float)r.NextDouble();
                Vector2 movement = new Vector2(maxMovementPerUpdate.X * (float)(r.NextDouble() * 2 - 1), maxMovementPerUpdate.Y * (float)(r.NextDouble() * 2 - 1));
                int ttl = (maxTTL / 2) + r.Next(maxTTL / 2);
                Particle p = new Particle(texture, location, initialRotation, rotationOnUpdate, movement, scale, ttl, Color.White);
                particles.Add(p);
            }

        }

        public void ClearParticles()
        {
            this.particles.Clear();
        }

    }
}
