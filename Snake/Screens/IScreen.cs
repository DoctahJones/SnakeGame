using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake.Screens
{
    public interface IScreen
    {

        string ScreenName { get; }
        bool Active { get; set; }

        void Update(GameTime gameTime, InputState inputState);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }
}
