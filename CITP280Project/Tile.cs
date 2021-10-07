using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Represents a 1x1 area of the game world coordinate system.
    /// </summary>
    public class Tile : IDrawable
    {
        public Bitmap CurrentImage { get; }
        public Point Location { get; }
        public bool Passable { get; }

        public Tile(Bitmap image, Point location, bool passable)
        {
            CurrentImage = image;
            Location = location;
            Passable = passable;
        }
    }
}
