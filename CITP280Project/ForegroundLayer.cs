using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Renders game objects that should be shown above the background. Currently only draws the character.
    /// </summary>
    public class ForegroundLayer : Layer
    {
        private int playerSize = 80;

        public ForegroundLayer(Player player, int width, int height) : base(player, width, height)
        { }

        public override Bitmap Draw()
        {
            int imageX = Convert.ToInt32(CurrentImage.Width / 2.0 - playerSize / 2.0);
            int imageY = Convert.ToInt32(CurrentImage.Height / 2.0 - playerSize / 2.0);

            graphics.Clear(Color.Transparent);
            graphics.DrawImage(player.CurrentImage, imageX, imageY, playerSize, playerSize);

            return CurrentImage;
        }
    }
}
