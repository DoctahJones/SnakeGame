using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snake.Screens
{
    public class ScreenManager
    {
        private Dictionary<string, IScreen> screens = new Dictionary<string, IScreen>();
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public ScreenManager(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            foreach (string s in screens.Keys)
            {
                if (screens[s].Active)
                {
                    screens[s].Update(gameTime, inputState);
                    break;  //prevents updating later screen in list if we enable that screen this frame update.
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (string s in screens.Keys)
            {
                if (screens[s].Active)
                {
                    screens[s].Draw(gameTime, spriteBatch);
                }
            }
        }



        /// <summary>
        /// Get a screen from its identifier in the collection.
        /// </summary>
        /// <param name="screenName">Identifier of the screen</param>
        /// <returns>Screen if it exists.</returns>
        public IScreen GetScreen(string screenName)
        {
            return screens[screenName];
        }

        /// <summary>
        /// Add screen to collection if there is no screen identified by same string so far.
        /// </summary>
        /// <param name="screenName">string to associate with screen to get from collection</param>
        /// <param name="screen">A type of screen.</param>
        /// <returns>true if added, else false</returns>
        public void AddScreen(string screenName, IScreen screen)
        {
            if (screenName == null)
            {
                throw new ArgumentNullException("No Screen Name specified.");
            }
            if (screens.ContainsKey(screenName))
            {
                throw new ArgumentException("A screen named {0} already exists.", screenName);
            }

            screens.Add(screenName, screen);
        }

        /// <summary>
        /// Remove a screen from the list by using its name.
        /// </summary>
        /// <param name="screenName">Name of screen to remove.</param>
        public void RemoveScreen(string screenName)
        {
            if (screenName == null) return;

            screens.Remove(screenName);
        }

        public void DisableScreen(string screenName)
        {
            if (screenName == null) return;
            if (!screens.ContainsKey(screenName)) return;

            screens[screenName].Active = false;
        }

        /// <summary>
        /// Disables all screens apart from the one which is passed in, does not activate the one passed in.
        /// </summary>
        /// <param name="screenName">Screen to not be disabled.</param>
        public void DisableOtherScreens(string screenName)
        {
            foreach (string s in screens.Keys)
            {
                if (screenName != s)
                {
                    screens[s].Active = false;
                }
            }
        }

        /// <summary>
        /// Enables the screen with the specified name.
        /// </summary>
        /// <param name="screenName">the name associated with the screen.</param>
        public void EnableScreen(string screenName)
        {
            if (screenName == null) return;
            if (!screens.ContainsKey(screenName)) return;

            screens[screenName].Active = true;
        }

    }
}
