using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Input.Keyboard;
using System.Windows;
using System.Drawing;

namespace CITP280Project
{
    /// <summary>
    /// Represents a person in the game world.
    /// </summary>
    public class Player : IDrawable
    {
        private Bitmap imgFacingRight;
        private Bitmap imgFacingLeft;
        private Bitmap currentImage;
        private double moveSpeed = 4; // tiles per second
        private DateTime lastMoveTime;
        private bool isMoving;

        // Declared here only for performance
        private TimeSpan delta;
        private DateTime now;
        private double moveDistance;
        private double vertical;
        private double horizontal;
        private double radians;
        private double moveX;
        private double moveY;

        public readonly object StateChangeLock = new object();
        // state variables
        public string Name { get; set; }
        public Point<double> Location { get; set; } = new Point<double>(0, 0);
        public int Heading { get; set; }
        public double Hunger { get; set; } = 0.75;
        public bool IsSaved { get; set; }

        public Bitmap CurrentImage
        {
            get
            {
                if (Heading > 90 && Heading < 270)
                    currentImage = imgFacingLeft;
                else if (Heading < 90 || Heading > 270)
                    currentImage = imgFacingRight;

                return currentImage;
            }
        }

        private MovedEventHandler movedDelegate;
        public event MovedEventHandler Moved
        {
            add => movedDelegate += value;
            remove => movedDelegate -= value;
        }

        public Player()
        {
            imgFacingLeft = Images.PlayerFacingLeft;
            imgFacingRight = Images.PlayerFacingRight;
            currentImage = imgFacingRight;
        }

        public Player(string name) : this()
        {
            Name = name;

            var rng = new Random();
            Location = new Point<double>(rng.NextDouble(-16, 16), rng.NextDouble(-16, 16));
        }

        public Player(string name, Point<double> location, double hunger, bool isSaved) : this()
        {
            Name = name;
            Location = location;
            Hunger = hunger;
            IsSaved = isSaved;
        }

        public void StartMove()
        {
            if (!isMoving)
            {
                lastMoveTime = DateTime.Now;
                isMoving = true;
            }
        }

        public void StopMove()
        {
            if (isMoving)
                isMoving = false;
        }

        /// <summary>
        /// If the player is moving, calculates how far they should have traveled since the last call
        /// and moves the player by that amount, also invoking the Moved event.
        /// </summary>
        public void Move()
        {
            if (isMoving)
            {
                now = DateTime.Now;
                delta = now - lastMoveTime;
                if (lastMoveTime != default)
                    moveDistance = moveSpeed * delta.TotalSeconds;

                if (IsKeyDown(KeyMap.Slow))
                    moveDistance /= 2;
                else if (IsKeyDown(KeyMap.Fast))
                    moveDistance *= 1.75;

                vertical = 0;
                horizontal = 0;

                if (IsKeyDown(KeyMap.Up))
                    vertical += moveDistance;

                if (IsKeyDown(KeyMap.Down))
                    vertical -= moveDistance;

                if (IsKeyDown(KeyMap.Left))
                    horizontal -= moveDistance;

                if (IsKeyDown(KeyMap.Right))
                    horizontal += moveDistance;

                if (horizontal != 0 || vertical != 0)
                {
                    lock (StateChangeLock)
                    {
                        if (horizontal > 0 && vertical == 0)
                            Heading = 0;
                        else if (horizontal > 0 && vertical > 0)
                            Heading = 45;
                        else if (horizontal == 0 && vertical > 0)
                            Heading = 90;
                        else if (horizontal < 0 && vertical > 0)
                            Heading = 135;
                        else if (horizontal < 0 && vertical == 0)
                            Heading = 180;
                        else if (horizontal < 0 && vertical < 0)
                            Heading = 225;
                        else if (horizontal == 0 && vertical < 0)
                            Heading = 270;
                        else if (horizontal > 0 && vertical < 0)
                            Heading = 315;

                        radians = Math.PI / 180 * Heading;
                        moveX = Math.Cos(radians) * moveDistance;
                        moveY = Math.Sin(radians) * moveDistance;

                        Location = new Point<double>(Location.X + moveX, Location.Y + moveY);
                        IsSaved = false;
                    }

                    movedDelegate?.Invoke(this, Location, Heading);
                }

                lastMoveTime = now;
            }
        }
    }
}
