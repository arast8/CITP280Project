using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Layers are responsible for rendering a group of game visuals.
    /// </summary>
    public abstract class Layer : IDrawable
    {
        protected Graphics graphics;
        protected Player player;

        public Bitmap CurrentImage { get; protected set; }

        public Layer(Player player, int width, int height)
        {
            this.player = player;
            Resize(width, height);
        }

        public abstract Bitmap Draw();

        public virtual void Resize(int width, int height)
        {
            CurrentImage = new Bitmap(width, height);
            graphics = Graphics.FromImage(CurrentImage);
        }
    }
}
