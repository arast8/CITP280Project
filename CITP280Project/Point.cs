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
    /// and Y.
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

        /// <summary>
        /// Returns the distance between two Points.
        /// </summary>
        public double DistanceTo<T2>(Point<T2> p2) where T2 : struct
        {
            if (this is Point<int> p1Int)
            {
                if (p2 is Point<int> p2Int)
                    return MathHelper.Distance(p1Int.X, p1Int.Y, p2Int.X, p2Int.Y);
                else if (p2 is Point<double> p2Double)
                    return MathHelper.Distance(p1Int.X, p1Int.Y, p2Double.X, p2Double.Y);
            }
            else if (this is Point<double> p1Double)
            {
                if (p2 is Point<int> p2Int)
                    return MathHelper.Distance(p1Double.X, p1Double.Y, p2Int.X, p2Int.Y);
                else if (p2 is Point<double> p2Double)
                    return MathHelper.Distance(p1Double.X, p1Double.Y, p2Double.X, p2Double.Y);
            }

            throw new ArgumentException("Either this or p2 has an unsupported type parameter. Valid type parameters are int and double.");
        }
    }
}
