using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using Point = System.Drawing.Point;
using PointD = System.Windows.Point;

namespace CITP280Project
{
    /// <summary>
    /// The MathHelper static class is for methods that perform calculations.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Rounds a number down to the nearest multiple of another number.
        /// </summary>
        public static int Floor(double value, int multiple)
        {
            int roundedValue = Convert.ToInt32((int)(value / multiple) * multiple);

            if (value < 0 && value % multiple != 0)
                roundedValue -= multiple;

            return roundedValue;
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        public static double Distance(double x1, double y1, double x2, double y2) => Sqrt(Pow(y2 - y1, 2) + Pow(x2 - x1, 2));
        public static double Distance(Point p1, Point p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static double Distance(PointD p1, PointD p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static double Distance(Point p1, PointD p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
        public static double Distance(PointD p1, Point p2) => Distance(p1.X, p1.Y, p2.X, p2.Y);
    }
}
