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
        public bool CanBeGround { get; private set; }
        public bool CanBePlayerLevel { get; private set; }

        public Material(string name, Bitmap image, bool canBeGround, bool canBePlayerLevel)
        {
            Name = name;
            CurrentImage = image;
            CanBeGround = canBeGround;
            CanBePlayerLevel = canBePlayerLevel;
        }

        /// <summary>
        /// Instantiates all Materials needed by the game.
        /// </summary>
        public static void Initialize()
        {
            Unknown = new Material("Unknown", Images.Unknown, true, true);
            Dirt = new Material("Dirt", Images.Dirt, true, false);
            Stone = new Material("Stone", Images.Stone, true, false);
            StoneFlower = new Material("StoneFlower", Images.StoneFlower, false, true);
            Grass = new Material("Grass", Images.Grass, true, false);
            Wheat = new Material("Wheat", Images.Wheat, false, true);
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
