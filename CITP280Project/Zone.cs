using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Represents a 16x16 unit area of a World.
    /// </summary>
    public class Zone
    {
        public const int ZoneSize = 16;
        private static Random rng;

        public Point Location { get; }
        public Material[,] Ground { get; } = new Material[ZoneSize, ZoneSize];
        public Material[,] PlayerLevel { get; } = new Material[ZoneSize, ZoneSize];
        public Biome Biome { get; private set; }

        public Zone(Point location)
        {
            if (location.X % ZoneSize == 0 && location.Y % ZoneSize == 0)
            {
                Location = location;

                rng = new Random();
                var biomeNumber = rng.Next(5);

                if (biomeNumber < 3)
                    InitGrassBiome();
                else
                    InitStoneBiome();
            }
            else
            {
                throw new Exception("Invalid chunk location: " + location);
            }
        }

        /// <summary>
        /// Sets all of Ground to Grass.
        /// 1/20 chance of inserting Wheat at a location in PlayerLevel.
        /// </summary>
        private void InitGrassBiome()
        {
            Biome = Biome.Grass;

            for (int i = 0; i<ZoneSize; i++)
            {
                for (int j = 0; j<ZoneSize; j++)
                {
                    Ground[i, j] = Material.Grass;

                    if (rng.Next(20) == 0)
                        PlayerLevel[i, j] = Material.Wheat;
                }
            }
        }

        /// <summary>
        /// Sets all of Ground to Stone.
        /// 1/20 chance of inserting StoneFlower at a location in PlayerLevel.
        /// </summary>
        private void InitStoneBiome()
        {
            Biome = Biome.Stone;

            for (int i = 0; i < ZoneSize; i++)
            {
                for (int j = 0; j < ZoneSize; j++)
                {
                    Ground[i, j] = Material.Stone;

                    if (rng.Next(20) == 0)
                        PlayerLevel[i, j] = Material.StoneFlower;
                }
            }
        }
    }
}
