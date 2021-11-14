using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public class Material : IDrawable
    {
        public static Material Unknown { get; private set; }
        public static Material Dirt { get; private set; }
        public static Material Stone { get; private set; }
        public static Material StoneFlower { get; private set; }
        public static Material Grass { get; private set; }
        public static Material Wheat { get; private set; }

        public string Name { get; private set; }
        public Bitmap CurrentImage { get; private set; }

        public Material(string name, Bitmap image)
        {
            Name = name;
            CurrentImage = image;
        }

        /// <summary>
        /// Instantiates all Materials needed by the game.
        /// </summary>
        public static void Initialize()
        {
            Unknown = new Material("Unknown", Images.Unknown);
            Dirt = new Material("Dirt", Images.Dirt);
            Stone = new Material("Stone", Images.Stone);
            StoneFlower = new Material("StoneFlower", Images.StoneFlower);
            Grass = new Material("Grass", Images.Grass);
            Wheat = new Material("Wheat", Images.Wheat);
        }

        public static Material GetMaterial(string name)
        {
            if (name == null) return null;
            if (name == Dirt.Name) return Dirt;
            if (name == Stone.Name) return Stone;
            if (name == StoneFlower.Name) return StoneFlower;
            if (name == Grass.Name) return Grass;
            if (name == Wheat.Name) return Wheat;
            else return Unknown;
        }
    }
}
