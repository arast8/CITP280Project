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
        private int characterSize = 80;

        public ForegroundLayer(Character character, int width, int height) : base(character, width, height)
        { }

        public override Bitmap Draw()
        {
            int imageX = Convert.ToInt32(CurrentImage.Width / 2.0 - characterSize / 2.0);
            int imageY = Convert.ToInt32(CurrentImage.Height / 2.0 - characterSize / 2.0);

            graphics.Clear(Color.Transparent);
            graphics.DrawImage(character.CurrentImage, imageX, imageY, characterSize, characterSize);

            return CurrentImage;
        }
    }
}
