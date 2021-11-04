using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Extension methods for System.Random.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random double greater than or equal to lowerBound and less than upperBound.
        /// </summary>
        public static double NextDouble(this Random r, double lowerBound, double upperBound)
        {
            if (upperBound < lowerBound)
                throw new ArgumentException("upperBound cannot be smaller than lowerBound.");

            double range = Math.Abs(upperBound - lowerBound);

            return lowerBound + r.NextDouble() * range;
        }
    }
}
