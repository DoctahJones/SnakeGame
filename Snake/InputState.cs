using Microsoft.Xna.Framework.Input;
using System;


namespace Snake
{
    public struct InputState
    {
        public KeyboardState OldKeyboardState, CurrentKeyboardState;
        public MouseState OldMouseState, CurrentMouseState;

        public InputState(KeyboardState oldKeyboardState, KeyboardState currentKeyboardState, MouseState oldMouseState, MouseState currentMouseState)
        {
            OldKeyboardState = oldKeyboardState;
            CurrentKeyboardState = currentKeyboardState;
            OldMouseState = oldMouseState;
            CurrentMouseState = currentMouseState;
        }

        public bool KeyWasUpNowDown(Keys key)
        {
            if (!OldKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        public bool HasMouseMoved()
        {
            if (OldMouseState.Position != CurrentMouseState.Position)
            {
                return true;
            }
            return false;
        }
    }
}
