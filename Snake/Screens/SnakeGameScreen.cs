using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake.Screens
{
    public class SnakeGameScreen : IScreen
    {
        /// <summary>
        /// Screenmanager handling this screen.
        /// </summary>
        private ScreenManager screenManager;
        /// <summary>
        /// The background tiled screen.
        /// </summary>
        private TiledGameScreen backgroundTiledScreen;
        /// <summary>
        /// The snake that wiggles around the map!
        /// </summary>
       private Snake snake;
        /// <summary>
        /// The class that manages the food the snake eats.
        /// </summary>
        private SnakeFood snakeFood;

        /// <summary>
        /// The name identifier used by the screen.
        /// </summary>
        public string ScreenName { get; private set; }
        /// <summary>
        /// Whether the game is active or not, still draws the game if not active but does not update it.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Whether the game is paused.
        /// </summary>
        public bool Paused { get; set; }

        public GameEndResult GameEndResult {get; private set;}


        public SnakeGameScreen(ScreenManager screenManager, string name)
        {
            this.screenManager = screenManager;
            ScreenName = name;

        }

        public void Initialize(TiledGameScreen background, SnakeFood snakeFood, Snake snake)
        {
            this.backgroundTiledScreen = background;
            this.snakeFood = snakeFood;
            this.snake = snake;

            this.snakeFood.ResetFood(snake.SnakePositions.ToList());
            this.snakeFood.Active = true;
        }


        public void Update(GameTime gameTime, InputState inputState)
        {
            //this way the snake wont always move as soon as you unpause but instead will restart as if the timer just ticked when you restart the game.
            if (Paused)
            {
                this.snake.SetLastUpdateTimeToNow(gameTime);
                Paused = false;
            }

            UpdateControls(inputState);
            snake.Update(gameTime, inputState);
            UpdateCollisions();
            snakeFood.Update(gameTime);
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.backgroundTiledScreen.Draw(gameTime, spriteBatch);
            this.snakeFood.Draw(spriteBatch);
            this.snake.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Method to reset the snake positions and generate a food item in a new place.
        /// </summary>
        public void ResetSnakeScreen()
        {
            this.snake.ResetSnakePosition();
            this.snakeFood.ResetFood(snake.SnakePositions.ToList());
        }

        public void SetSnakeSpeed(float secondsPerUpdate)
        {
            snake.Speed = TimeSpan.FromSeconds(secondsPerUpdate);
        }

        /// <summary>
        /// Method to update the game depending on the user input.
        /// </summary>
        /// <param name="inputState">The state of the input methods.</param>
        private void UpdateControls(InputState inputState)
        {
            if (inputState.KeyWasUpNowDown(Keys.Escape))
            {
                PauseGame();
            }
            else if (IsInputKeyPressedDown(inputState))
            {
                //snake shouldn't try to move in opposite direction to the way it is currently heading.
                if (snake.DirectionFacing != Direction.UP)
                {
                    snake.DirectionToFace = Direction.DOWN;
                }
            }
            else if (IsInputKeyPressedRight(inputState))
            {
                if (snake.DirectionFacing != Direction.LEFT)
                {
                    snake.DirectionToFace = Direction.RIGHT;
                }
            }
            else if (IsInputKeyPressedUp(inputState))
            {
                if (snake.DirectionFacing != Direction.DOWN)
                {
                    snake.DirectionToFace = Direction.UP;
                }
            }
            else if (IsInputKeyPressedLeft(inputState))
            {
                if (snake.DirectionFacing != Direction.RIGHT)
                {
                    snake.DirectionToFace = Direction.LEFT;
                }
            }
        }

        private void PauseGame()
        {
            this.Paused = true;
            screenManager.DisableOtherScreens("pauseMenu");
            screenManager.EnableScreen("pauseMenu");
        }

        private bool IsInputKeyPressedDown(InputState inputState)
        {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Down) || inputState.CurrentKeyboardState.IsKeyDown(Keys.S) ||
                inputState.CurrentKeyboardState.IsKeyDown(Keys.NumPad2))
            {
                return true;
            }
            return false;
        }

        private bool IsInputKeyPressedRight(InputState inputState)
        {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Right) || inputState.CurrentKeyboardState.IsKeyDown(Keys.D) || inputState.CurrentKeyboardState.IsKeyDown(Keys.NumPad6))
            {
                return true;
            }
            return false;
        }

        private bool IsInputKeyPressedUp(InputState inputState)
        {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Up) || inputState.CurrentKeyboardState.IsKeyDown(Keys.W) ||
                inputState.CurrentKeyboardState.IsKeyDown(Keys.NumPad8))
            {
                return true;
            }
            return false;
        }

        private bool IsInputKeyPressedLeft(InputState inputState)
        {
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.Left) || inputState.CurrentKeyboardState.IsKeyDown(Keys.A) ||
                inputState.CurrentKeyboardState.IsKeyDown(Keys.NumPad4))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks whether the snake has collided with any edges, itself or an unpathable tile.
        /// </summary>
        private void UpdateCollisions()
        {
            UpdateSnakeCollisionsWithUnpathableItems();
            UpdateSnakeCollisionsWithFood();
        }

        private void UpdateSnakeCollisionsWithUnpathableItems()
        {
            if (SnakeCollidedWithOuterWalls())
            {
                SnakeDied();
            }
            else if (SnakeCollidesWithUnpathableTile())
            {
                SnakeDied();
            }
            else
            {
                if (SnakeCollidedWithItself())
                {
                    SnakeDied();
                }
            }
        }

        private bool SnakeCollidesWithUnpathableTile()
        {
            return !backgroundTiledScreen.GetCellInfo(snake.HeadPosition.X, snake.HeadPosition.Y).Pathable;
        }

        private bool SnakeCollidedWithItself()
        {
            LinkedList<Point> snakePositions = snake.SnakePositions;
            Point head = snakePositions.First.Value;
            LinkedListNode<Point> curr = snakePositions.First.Next;

            while (curr != null)
            {
                if (curr.Value == head)
                {
                    return true;
                }
                curr = curr.Next;
            }
            return false;
        }

        private bool SnakeCollidedWithOuterWalls()
        {
            if (snake.HeadPosition.X < 0 || snake.HeadPosition.X >= backgroundTiledScreen.ScreenSizeInTiles.X)
            {
                return true;
            }
            else if (snake.HeadPosition.Y < 0 || snake.HeadPosition.Y >= backgroundTiledScreen.ScreenSizeInTiles.Y)
            {
                return true;
            }
            return false;
        }

        private void SnakeDied()
        {
            GameEndResult = GameEndResult.LOST;
            screenManager.DisableOtherScreens("endGameScreen");
            screenManager.EnableScreen("endGameScreen");
        }

        private void SnakeGameWon()
        {
            GameEndResult = GameEndResult.WON;
            screenManager.DisableOtherScreens("endGameScreen");
            screenManager.EnableScreen("endGameScreen");
        }

        private void UpdateSnakeCollisionsWithFood()
        {
            if (snake.HeadPosition == snakeFood.Position)
            {
                snake.Length += 1;
                //if snake has filled screen then player has won the game what a hero eh.
                if (SnakeHasFilledScreen())
                {
                    SnakeGameWon();
                }
                snakeFood.EatCurrentActiveItem(snake.SnakePositions.ToList());
                snake.IsEating = true;
            }
        }

        private bool SnakeHasFilledScreen()
        {
            if (snake.Length == backgroundTiledScreen.ScreenSizeInTiles.X * backgroundTiledScreen.ScreenSizeInTiles.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
