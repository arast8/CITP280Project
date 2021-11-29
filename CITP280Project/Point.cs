using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Point is a 2-dimensional coordinate pair that can be made of any type of number.
    /// </summary>
    /// <typeparam name="T">The type of number to use for coordinates</typeparam>
    /// <remarks>
    /// T should be restricted to only numeric types, but I haven't found a way
    /// of doing that yet. As a result, you can put non-numeric structs in X
    /// and Y, and methods that perform calculations needed to be extension
    /// methods with specific types used as the type parameter. Those are in
    /// <see cref="PointExtensions"/>.
    /// </remarks>
    public struct Point<T> where T : struct
    {
        public T X;
        public T Y;

        public Point(T x, T y)
        {
            X = x;
            Y = y;
        }
    }

    public static class PointExtensions
    {
        /// <summary>
        /// Returns the distance between this and another Point.
        /// </summary>
        public static double DistanceTo(this Point<int> p1, Point<int> p2) => MathHelper.Distance(p1.X, p1.Y, p2.X, p2.Y);

        /// <summary>
        /// Returns the distance between this and another Point.
        /// </summary>
        public static double DistanceTo(this Point<int> p1, Point<double> p2) => MathHelper.Distance(p1.X, p1.Y, p2.X, p2.Y);

        /// <summary>
        /// Returns the distance between this and another Point.
        /// </summary>
        public static double DistanceTo(this Point<double> p1, Point<int> p2) => MathHelper.Distance(p1.X, p1.Y, p2.X, p2.Y);

        /// <summary>
        /// Returns the distance between this and another Point.
        /// </summary>
        public static double DistanceTo(this Point<double> p1, Point<double> p2) => MathHelper.Distance(p1.X, p1.Y, p2.X, p2.Y);
    }
}
