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
    public class Character : IDrawable
    {
        private double moveSpeed = 2; // tiles per second
        private Vector vector;
        private DateTime lastMoveTime;
        private TimeSpan delta;
        private DateTime now;
        private double moveDistance;
        public bool isMoving;
        private Bitmap facingRight;
        private Bitmap facingLeft;

        public event EventHandler<EventArgs> Move;
        public Point Location { get; private set; } = new Point(0, 0);
        public Bitmap CurrentImage { get; private set; }
        public string Name { get; set; }
        public double CurrentHunger { get; private set; } = 0.75;

        public Character(string name)
        {
            Name = name;

            facingRight = Properties.Resources.Character;
            facingLeft = (Bitmap)facingRight.Clone();
            facingLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);

            CurrentImage = facingRight;
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
        /// If the character is moving, calculates how far they should have traveled since the last call
        /// and moves the character by that amount, also invoking the Move event.
        /// </summary>
        public void KeyboardMove()
        {
            if (isMoving)
            {
                now = DateTime.Now;
                delta = now - lastMoveTime;
                if (lastMoveTime != default)
                    moveDistance = moveSpeed * delta.TotalSeconds;

                double vertical = 0;
                double horizontal = 0;

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
                    CurrentImage = facingRight;
                else if (vector.X < 0)
                    CurrentImage = facingLeft;

                lastMoveTime = now;
                Move?.Invoke(this, new EventArgs());
            }
        }
    }
}
