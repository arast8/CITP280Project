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
        protected Character character;

        public Bitmap CurrentImage { get; protected set; }

        public Layer(Character character, int width, int height)
        {
            this.character = character;
            Resize(width, height);
        }

        public virtual void Resize(int width, int height)
        {
            CurrentImage = new Bitmap(width, height);
            graphics = Graphics.FromImage(CurrentImage);
        }

        public abstract Bitmap Draw();
    }
}
