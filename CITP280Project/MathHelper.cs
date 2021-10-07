using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        public static int Floor(double value, int multiple)
        {
            int roundedValue = Convert.ToInt32((int)(value / multiple) * multiple);

            if (value < 0 && value % multiple != 0)
                roundedValue -= multiple;

            return roundedValue;
        }
    }
}
