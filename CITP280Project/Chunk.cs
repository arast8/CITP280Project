using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public class Chunk
    {
        /// <summary>
        /// Represents a group of Tiles. It might make Tiles easier to handle
        /// when implementing a way of saving and loading them, but I'm not sure yet.
        /// </summary>
        public const int ChunkSize = 16;
        public Point Location;
        public readonly Tile[,] Tiles = new Tile[ChunkSize, ChunkSize];

        public Chunk(Point location)
        {
            if (location.X % ChunkSize == 0 && location.Y % ChunkSize == 0)
            {
                Location = location;

                for (int i = 0; i < ChunkSize; i++)
                    for (int j = 0; j < ChunkSize; j++)
                        Tiles[i, j] = new Tile(Properties.Resources.Dirt, new Point(Location.X + i, Location.Y + j), true);
            }
            else
            {
                throw new Exception("Invalid chunk location: " + location);
            }
        }
    }
}
