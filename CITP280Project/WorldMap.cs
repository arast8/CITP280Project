using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CITP280Project
{
    /// <summary>
    /// Performs operations on a Dictionary mapping Points to Chunks.
    /// This should be a class that is instantiated and manages a dictionary of its own,
    /// but that's not how I made my UML class diagram.
    /// </summary>
    public class WorldMap
    {
        public static Chunk GetChunk(Dictionary<Point, Chunk> chunks, int x, int y)
        {
            int chunkX = MathHelper.Floor(x, Chunk.ChunkSize);
            int chunkY = MathHelper.Floor(y, Chunk.ChunkSize);

            var location = new Point(chunkX, chunkY);

            if (chunks.ContainsKey(location))
                return chunks[location];
            else
            {
                var newChunk = new Chunk(location);
                chunks[location] = newChunk;
                return newChunk;
            }
        }

        public static Tile GetTile(Dictionary<Point, Chunk> chunks, int x, int y)
        {
            var chunk = GetChunk(chunks, x, y);
            return chunk.Tiles[x - chunk.Location.X, y - chunk.Location.Y];
        }

        public static Tile GetTile(Point location) => GetTile(location.X, location.Y);
        public static Tile GetTile(float x, float y) => GetTile(Convert.ToInt32(Math.Floor(x)), Convert.ToInt32(Math.Floor(y)));
    }
}
