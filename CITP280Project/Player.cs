using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Input.Keyboard;
using System.Windows;
using System.Drawing;
using Point = System.Windows.Point;

namespace CITP280Project
{
    /// <summary>
    /// Represents a person in the game world.
    /// </summary>
    public class Player : IDrawable
    {
        private Bitmap imgFacingRight;
        private Bitmap imgFacingLeft;
        private double moveSpeed = 2; // tiles per second
        private DateTime lastMoveTime;
        private bool isMoving;

        // Declared here only for performance
        private TimeSpan delta;
        private DateTime now;
        private double moveDistance;
        private Vector vector;
        private double vertical;
        private double horizontal;

        public string Name { get; set; }
        public Point Location { get; private set; } = new Point(0, 0);
        public Bitmap CurrentImage { get; private set; }
        public double CurrentHunger { get; private set; } = 0.75;

        public event EventHandler<EventArgs> Moved;

        public Player(string name)
        {
            Name = name;

            imgFacingLeft = Images.PlayerFacingLeft;
            imgFacingRight = Images.PlayerFacingRight;

            CurrentImage = imgFacingRight;
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

                vector = new Vector(horizontal, vertical);
                Location += vector;

                if (vector.X > 0)
                    CurrentImage = imgFacingRight;
                else if (vector.X < 0)
                    CurrentImage = imgFacingLeft;

                lastMoveTime = now;
                Moved?.Invoke(this, new EventArgs());
            }
        }
    }
}
