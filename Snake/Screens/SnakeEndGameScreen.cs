using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Screens
{
    public enum GameEndResult
    {
        WON, LOST
    }

    public class SnakeEndGameScreen : IScreen
    {
        private ScreenManager screenManager;

        private SnakeGameScreen snakeScreen;

        private SpriteFont font;

        private Color fontColor;

        private Rectangle textRectangle;

        public string ScreenName { get; private set; }

        public bool Active { get; set; }



        public SnakeEndGameScreen(ScreenManager screenManager, string name)
        {
            this.screenManager = screenManager;
            ScreenName = name;
        }

        public void Initialize(SnakeGameScreen snakeGameScreen, SpriteFont font, Color fontColor)
        {
            this.snakeScreen = snakeGameScreen;
            this.font = font;
            this.fontColor = fontColor;
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            textRectangle = CalculateTextRectangle();
            UpdateControls(inputState);
        }
        private Rectangle CalculateTextRectangle()
        {
            Vector2 vec = font.MeasureString(GetEndGameText());
            Rectangle rect = new Rectangle();
            rect.Width = (int)vec.X;
            rect.Height = (int)vec.Y;
            rect.X = (screenManager.ScreenWidth / 2) - (rect.Width / 2);
            rect.Y = (screenManager.ScreenHeight / 2) - (rect.Height / 2); 
            
            return rect;
        }

        private void UpdateControls(InputState inputState)
        {
            if (inputState.OldMouseState.LeftButton == ButtonState.Released && inputState.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                SetScreenMainMenu();
            }
            else if (inputState.KeyWasUpNowDown(Keys.Enter))
            {
                SetScreenMainMenu();
            }
        }

        private void SetScreenMainMenu()
        {
            screenManager.DisableOtherScreens("mainMenu");
            screenManager.EnableScreen("mainMenu");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            snakeScreen.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, GetEndGameText(), new Vector2(textRectangle.X, textRectangle.Y), fontColor);
        }

        private string GetEndGameText()
        {
            switch (snakeScreen.GameEndResult)
            {
                case GameEndResult.WON :
                    return "You won the game!";
                case GameEndResult.LOST :
                    return "You just lost the game!";
                default :
                    return "You broke the game!";
            }
        }
    }
}
