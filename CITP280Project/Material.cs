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
        public string Name { get; private set; }
        public Bitmap CurrentImage { get; private set; }
        public static Material Unknown { get; private set; }
        public static Material Dirt { get; private set; }
        public static Material Stone { get; private set; }

        public Material(string name, Bitmap image)
        {
            Name = name;
            CurrentImage = image;
        }

        public static void Initialize()
        {
            Unknown = new Material("Unknown", Images.Unknown);
            Dirt = new Material("Dirt", Images.Dirt);
            Stone = new Material("Stone", Images.Stone);
        }
    }
}
