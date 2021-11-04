using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    /// <summary>
    /// Draws UI elements.
    /// </summary>
    public class UILayer : Layer
    {
        private Rectangle hungerBarRect;
        private Font uiFont;
        private StringFormat centeredStringFormat;

        public UILayer(WorldView worldView) : base(worldView)
        {
            uiFont = new Font(FontFamily.GenericSansSerif, 12);
            centeredStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
        }

        public override void Draw()
        {
            Graphics graphics = worldView.Graphics;
            Player player = worldView.Player;

            hungerBarRect.Width = 200;
            hungerBarRect.Height = 25;
            hungerBarRect.X = Convert.ToInt32(graphics.VisibleClipBounds.Width * 0.1);
            hungerBarRect.Y = Convert.ToInt32(graphics.VisibleClipBounds.Height * 0.9 - hungerBarRect.Height);

            // Draw hunger bar
            graphics.FillRectangle(Brushes.White, hungerBarRect);
            graphics.FillRectangle(Brushes.SaddleBrown,
                hungerBarRect.X,
                hungerBarRect.Y,
                Convert.ToInt32(hungerBarRect.Width * player.CurrentHunger),
                hungerBarRect.Height);
            graphics.DrawRectangle(Pens.Black, hungerBarRect);
            graphics.DrawString("Hunger", uiFont, Brushes.Black, hungerBarRect, centeredStringFormat);
        }
    }
}
