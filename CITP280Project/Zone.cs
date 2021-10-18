using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public class Zone
    {
        /// <summary>
        /// Represents a group of Tiles. It might make Tiles easier to handle
        /// when implementing a way of saving and loading them, but I'm not sure yet.
        /// </summary>
        public const int ZoneSize = 32;
        public Point Location { get; }
        public Material[,] Tiles { get; } = new Material[ZoneSize, ZoneSize];

        public Zone(Point location)
        {
            if (location.X % ZoneSize == 0 && location.Y % ZoneSize == 0)
            {
                Location = location;

                for (int i = 0; i < ZoneSize; i++)
                    for (int j = 0; j < ZoneSize; j++)
                        Tiles[i, j] = Material.Dirt;
            }
            else
            {
                throw new Exception("Invalid chunk location: " + location);
            }
        }
    }
}
