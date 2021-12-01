using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Layers are responsible for drawing a group of game visuals onto a worldView.
    /// </summary>
    public abstract class Layer
    {
        protected WorldView worldView;

        public Layer(WorldView worldView)
        {
            this.worldView = worldView;
        }

        public abstract void Draw(Graphics graphics);
    }
}
