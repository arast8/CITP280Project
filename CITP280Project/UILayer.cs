using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Renders UI elements, which should be shown above all game objects. Currently it only draws a hunger bar.
    /// </summary>
    public class UILayer : Layer
    {
        private Rectangle hungerBarRect;
        private Font uiFont;
        private StringFormat centeredStringFormat;

        public UILayer(Player player, int width, int height) : base(player, width, height)
        {
            uiFont = new Font(FontFamily.GenericSansSerif, 12);
            centeredStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
        }

        public override Bitmap Draw()
        {
            graphics.Clear(Color.Transparent);

            // Draw hunger bar
            graphics.FillRectangle(Brushes.White, hungerBarRect);
            graphics.FillRectangle(Brushes.SaddleBrown,
                hungerBarRect.X,
                hungerBarRect.Y,
                Convert.ToInt32(hungerBarRect.Width * player.CurrentHunger),
                hungerBarRect.Height);
            graphics.DrawRectangle(Pens.Black, hungerBarRect);
            graphics.DrawString("Hunger", uiFont, Brushes.Black, hungerBarRect, centeredStringFormat);

            return CurrentImage;
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            hungerBarRect.Width = 200;
            hungerBarRect.Height = 25;
            hungerBarRect.X = Convert.ToInt32(width * 0.1);
            hungerBarRect.Y = Convert.ToInt32(height * 0.9 - hungerBarRect.Height);
        }
    }
}
