using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Represents a rectangle in a Cartesian plane where up is the positive y direction.
    /// That is the coordinate system used by this game.
    /// The (X, Y) point is at the lower left corner, as opposed to System.Windows.Rect or System.Drawing.Rectangle,
    /// where (X, Y) is the upper left corner.
    /// </summary>
    public struct CartesianRect
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double Top { get => Y + Height; }
        public double Right { get => X + Width; }
        public double Bottom { get => Y; }
        public double Left { get => X; }
        public double CenterX { get => X + Width / 2; }
        public double CenterY { get => Y + Height / 2; }

        public CartesianRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return string.Format("{{ X = {0:F}, Y = {1:F}, Width = {2:F}, Height = {3:F} }}", X, Y, Width, Height);
        }
    }
}
