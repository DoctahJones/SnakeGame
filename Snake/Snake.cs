using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Snake
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    public class Snake
    {
        /// <summary>
        /// List of positions where a snake body part is located. Head of snake is at head of list, tail at tail etc.
        /// </summary>
        private LinkedList<Point> snakePositions;
        /// <summary>
        /// Head that animates opening mouth to eat food. Default is facing upwards.
        /// </summary>
        private Animation head;
        /// <summary>
        /// Body texture that has straight body in first frame standing vertically, curved in second in L shape and tail in 3rd vertically.
        /// </summary>
        private Texture2D body;
        /// <summary>
        /// Width of a single frame of snake to make up the different parts.
        /// </summary>
        private int frameWidth;
        /// <summary>
        /// The size of 1 tile on a screen.
        /// </summary>
        private Point tileSize;
        /// <summary>
        /// The last time the snake moved.
        /// </summary>
        private TimeSpan lastTimeMoved;
        /// <summary>
        /// The direction the head is pointing at the moment.
        /// </summary>
        private Direction dirFacing;
        /// <summary>
        /// The direction we should change the snake to be facing the next time we update. The snake will then be moved in this direction on update if it isn't null.
        /// </summary>
        private Direction? dirToFaceOnUpdate;
        /// <summary>
        /// The initial length of the snake.
        /// </summary>
        private int initialLength;

        /// <summary>
        /// Length of the snake at the moment.
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// The delay in seconds between movements of the snake.
        /// </summary>
        public TimeSpan Speed { get; set; }
        /// <summary>
        /// The direction the head is pointing at the moment.
        /// </summary>
        public Direction DirectionFacing
        {
            get { return dirFacing; }
        }
        /// <summary>
        /// The direction the head is to be changed to face
        /// </summary>
        public Direction? DirectionToFace
        {
            set { dirToFaceOnUpdate = value; }
        }
        /// <summary>
        /// The position the head of the snake is at.
        /// </summary>
        public Point HeadPosition { get { return snakePositions.First.Value; } }
        /// <summary>
        /// Whether the snake is currently eating, if it is then when we next move the snake we don't remove the last element so the length increases.
        /// </summary>
        public bool IsEating { get; set; }
        /// <summary>
        /// The positions of the map that the snake has occupied.
        /// </summary>
        public LinkedList<Point> SnakePositions { get { return snakePositions; } }

        public Snake(Point tileSize)
        {
            this.tileSize = tileSize;
        }

        public void Initialize(Animation head, Texture2D bodySpritesheet, int frameWidthBody, int length, float speed)
        {
            this.head = head;
            this.body = bodySpritesheet;
            this.frameWidth = frameWidthBody;
            Speed = TimeSpan.FromSeconds(speed);
            this.initialLength = length;
            Length = length;
            snakePositions = new LinkedList<Point>();
            ResetSnakePosition();
        }

        public void ResetSnakePosition()
        {
            snakePositions.Clear();
            Length = this.initialLength;
            //create snake positions with head at position of the length -1 across the top and tail at 0,0.
            for (int i = initialLength - 1; i >= 0; i--)
            {
                snakePositions.AddLast(new Point(i, 0));
            }
            dirFacing = Direction.RIGHT;
            SetHeadAnimationPosition();
            RotateHead(dirFacing);
        }

        public void Update(GameTime gameTime, InputState input)
        {

            if (EnoughTimeHasPassedSinceLastTimeMoved(gameTime))
            {
                lastTimeMoved = gameTime.TotalGameTime;
                //change the direction the snake is facing.
                UpdateSnakeDirection();
                AddNewPieceToFrontOfSnake();
                SetHeadAnimationPosition();
                UpdateTailPiece();
            }
            //update head animation etc
            head.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            head.Draw(spriteBatch);

            DrawInnerBodySegments(spriteBatch);

            DrawTail(spriteBatch);
        }

        private void DrawInnerBodySegments(SpriteBatch spriteBatch)
        {
            LinkedListNode<Point> curr = snakePositions.First.Next;
            while (curr != snakePositions.Last)
            {
                DrawCurrentBodyPiece(curr, spriteBatch);
                curr = curr.Next;
            }
        }

        /// <summary>
        /// Sets the position of the animation using the coordinates of the head in tile coordinates.
        /// </summary>
        private void SetHeadAnimationPosition()
        {
            head.Position = new Vector2(snakePositions.First.Value.X * tileSize.X + (tileSize.X / 2), snakePositions.First.Value.Y * tileSize.Y + (tileSize.Y / 2));
        }

        private void AddNewPieceToFrontOfSnake()
        {
            switch (DirectionFacing)
            {
                case Direction.DOWN:
                    snakePositions.AddFirst(new Point(snakePositions.First.Value.X, snakePositions.First.Value.Y + 1));
                    break;
                case Direction.LEFT:
                    snakePositions.AddFirst(new Point(snakePositions.First.Value.X - 1, snakePositions.First.Value.Y));
                    break;
                case Direction.RIGHT:
                    snakePositions.AddFirst(new Point(snakePositions.First.Value.X + 1, snakePositions.First.Value.Y));
                    break;
                case Direction.UP:
                    snakePositions.AddFirst(new Point(snakePositions.First.Value.X, snakePositions.First.Value.Y - 1));
                    break;
            }
        }

        private void UpdateSnakeDirection()
        {
            dirFacing = dirToFaceOnUpdate ?? dirFacing;
            dirToFaceOnUpdate = null;
            RotateHead(dirFacing);
        }

        private void UpdateTailPiece()
        {
            //if snake isn't eating remove last item from list.
            if (!IsEating)
            {
                snakePositions.RemoveLast();
            }
            //snake has finished eating so next time we update continue removing item from rear.
            else
            {
                IsEating = !IsEating;
            }
        }

        private void DrawTail(SpriteBatch spriteBatch)
        {
            Rectangle destRect = CalculateDestinationRectangle(snakePositions.Last.Value.X, snakePositions.Last.Value.Y);
            Rectangle sourceRect = new Rectangle(2 * frameWidth, 0, frameWidth, body.Height);
            float rotation = CalculateRotationTail(snakePositions.Last.Value, snakePositions.Last.Previous.Value);
            Vector2 origin = new Vector2(frameWidth / 2, body.Height / 2);
            spriteBatch.Draw(body, destRect, sourceRect, Color.White, rotation, origin, SpriteEffects.None, 0);
        }

        private void DrawCurrentBodyPiece(LinkedListNode<Point> bodyPiece, SpriteBatch spriteBatch)
        {
            Rectangle destRect = CalculateDestinationRectangle(bodyPiece.Value.X, bodyPiece.Value.Y);

            float rotation;
            Rectangle sourceRect;
            Vector2 origin = new Vector2(frameWidth / 2, body.Height / 2);

            //calculate source rectangles from image and rotations for current item.
            if (IsCornerPiece(bodyPiece.Value, bodyPiece.Previous.Value, bodyPiece.Next.Value))
            {
                sourceRect = new Rectangle(1 * frameWidth, 0, frameWidth, body.Height);
                rotation = CalculateRotationCornerPiece(bodyPiece.Value, bodyPiece.Previous.Value, bodyPiece.Next.Value);
            }
            else
            {
                sourceRect = new Rectangle(0 * frameWidth, 0, frameWidth, body.Height);
                rotation = CalculateRotationStraightPiece(bodyPiece.Value, bodyPiece.Previous.Value, bodyPiece.Next.Value);
            }
            spriteBatch.Draw(body, destRect, sourceRect, Color.White, rotation, origin, SpriteEffects.None, 0);
        }

        private Rectangle CalculateDestinationRectangle(int x, int y)
        {
            return new Rectangle(x * tileSize.X + (tileSize.X / 2), y * tileSize.Y + (tileSize.Y / 2), tileSize.X, tileSize.Y);
        }


        private bool EnoughTimeHasPassedSinceLastTimeMoved(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastTimeMoved > Speed)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to rotate the head animation based on the direction passed in.
        /// </summary>
        /// <param name="direction">The direction the head should be rotated to be facing.</param>
        private void RotateHead(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    this.head.Rotation = (float)Math.PI;
                    break;
                case Direction.LEFT:
                    this.head.Rotation = (float)(Math.PI * 1.5);
                    break;
                case Direction.RIGHT:
                    this.head.Rotation = (float)(Math.PI * .5);
                    break;
                case Direction.UP:
                    this.head.Rotation = (float)0;
                    break;
            }
        }

        /// <summary>
        /// Method to check whether a point is a corner piece by checking whether the previous and next points are situated in a line or not.
        /// </summary>
        /// <param name="itemToCheck">The item we are checking to be a corner piece or not.</param>
        /// <param name="pointPrevious">The previous point position.</param>
        /// <param name="pointAfter">The next point position.</param>
        /// <returns>Return true of the item to check is a corner piece.</returns>
        private bool IsCornerPiece(Point itemToCheck, Point pointPrevious, Point pointAfter)
        {
            //if all the x's or y's are the same then that means the 3 points are in a line and therfore not a corner piece.
            if (itemToCheck.X == pointPrevious.X && itemToCheck.X == pointAfter.X)
            {
                return false;
            }
            if (itemToCheck.Y == pointPrevious.Y && itemToCheck.Y == pointAfter.Y)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calculate the rotation to be applied to a straight piece of body via checking 3 consecutive points.
        /// </summary>
        /// <param name="itemToCheck">The middle item of the 3 which is the point we want to calculate how rotated it should be.</param>
        /// <param name="pointPrevious">The point coming prior to the item we wish to check.</param>
        /// <param name="pointAfter">The point coming after the item we wish to check.</param>
        /// <returns>Returns the angle in radians the itemToCheck should be rotated.</returns>
        private float CalculateRotationStraightPiece(Point itemToCheck, Point pointPrevious, Point pointAfter)
        {
            //if snake is prev-cur-next
            if (pointPrevious.X == itemToCheck.X - 1 && pointAfter.X == itemToCheck.X + 1)
            {
                return (float)(Math.PI * 0.5);
            }
            //if snake is next-cur-prev
            else if (pointPrevious.X == itemToCheck.X + 1 && pointAfter.X == itemToCheck.X - 1)
            {
                return (float)(Math.PI * 0.5);
            }
            //if snake is (verctically top down) prev-cur-next
            else if (pointPrevious.Y == itemToCheck.Y - 1 && pointAfter.Y == itemToCheck.Y + 1)
            {
                return 0;
            }
            //if snake is (vertically top down) next-cur-prev
            else if (pointPrevious.Y == itemToCheck.Y + 1 && pointAfter.Y == itemToCheck.Y - 1)
            {
                return 0;
            }
            else
            {
                throw new ArithmeticException("Doesn't seem to be a straight piece of the body.");
            }
        }

        /// <summary>
        /// Calculate the rotation to be applied to a corner piece of the body via checking 3 consecutive points.
        /// </summary>
        /// <param name="itemToCheck">The middle item of the 3 which is the point we want to calculate how rotated it should be.</param>
        /// <param name="pointPrevious">The point coming prior to the item we wish to check.</param>
        /// <param name="pointAfter">The point coming after the item we wish to check.</param>
        /// <returns>Returns the angle in radians the itemToCheck should be rotated.</returns>
        private float CalculateRotationCornerPiece(Point itemToCheck, Point pointPrevious, Point pointAfter)
        {
            //if snake is prev
            //            cur-next
            if (pointPrevious.Y == itemToCheck.Y - 1 && pointAfter.X == itemToCheck.X + 1)
            {
                return 0;
            }
            //if snake is next
            //            cur-prev
            else if (pointPrevious.X == itemToCheck.X + 1 && pointAfter.Y == itemToCheck.Y - 1)
            {
                return 0;
            }
            //if snake is cur-next
            //            prev
            else if (pointPrevious.Y == itemToCheck.Y + 1 && pointAfter.X == itemToCheck.X + 1)
            {
                return (float)(Math.PI * 0.5);
            }
            //if snake is cur-prev
            //            next
            else if (pointPrevious.X == itemToCheck.X + 1 && pointAfter.Y == itemToCheck.Y + 1)
            {
                return (float)(Math.PI * 0.5);
            }
            //if snake is next-cur
            //                 prev
            else if (pointPrevious.Y == itemToCheck.Y + 1 && pointAfter.X == itemToCheck.X - 1)
            {
                return (float)Math.PI;
            }
            //if snake is prev-cur
            //                 next
            else if (pointPrevious.X == itemToCheck.X - 1 && pointAfter.Y == itemToCheck.Y + 1)
            {
                return (float)Math.PI;
            }
            //if snake is next
            //       prev-cur
            else if (pointPrevious.X == itemToCheck.X - 1 && pointAfter.Y == itemToCheck.Y - 1)
            {
                return (float)(Math.PI * 1.5);
            }
            //if snake is next
            //       prev-cur
            else if (pointPrevious.X == itemToCheck.X - 1 && pointAfter.Y == itemToCheck.Y - 1)
            {
                return (float)(Math.PI * 1.5);
            }
            //if snake is prev
            //       next-cur
            else if (pointPrevious.Y == itemToCheck.Y - 1 && pointAfter.X == itemToCheck.X - 1)
            {
                return (float)(Math.PI * 1.5);
            }
            else
            {
                throw new ArithmeticException("Doesn't seem to be a corner piece of the body.");
            }
        }

        /// <summary>
        /// Calcualte the rotation of the tail piece by looking at its location as well as the preceding item.
        /// </summary>
        /// <param name="tailPoint">The point corresponding to the tail piece which we want to calculate the rotation for.</param>
        /// <param name="penultimatePoint">The point that precedes the tail point.</param>
        /// <returns>Returns the angle in radians the tail should be rotated.</returns>
        private float CalculateRotationTail(Point tailPoint, Point penultimatePoint)
        {
            //looks like: prev
            //            tail
            if (tailPoint.Y == penultimatePoint.Y + 1 && tailPoint.X == penultimatePoint.X)
            {
                return 0;
            }
            else if (tailPoint.X == penultimatePoint.X - 1 && tailPoint.Y == penultimatePoint.Y)
            {
                return (float)(Math.PI * 0.5);
            }
            else if (tailPoint.Y == penultimatePoint.Y - 1 && tailPoint.X == penultimatePoint.X)
            {
                return (float)(Math.PI);
            }
            else if (tailPoint.X == penultimatePoint.X + 1 && tailPoint.Y == penultimatePoint.Y)
            {
                return (float)(Math.PI * 1.5);
            }
            else
            {
                throw new ArithmeticException("Something went wrong calculating tail rotation.");
            }
        }

        public void SetLastUpdateTimeToNow(GameTime gameTime)
        {
            this.lastTimeMoved = gameTime.TotalGameTime;
        }

    }
}
