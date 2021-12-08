using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        /// <summary>
        /// Draws the layer's visual elements onto the provided Graphics object.
        /// </summary>
        public abstract void Draw(Graphics graphics);

        /// <summary>
        /// Reacts to a click event
        /// </summary>
        /// <returns>Whether the click event has been handled</returns>
        public abstract bool HandleClick(MouseEventArgs e);
    }
}
