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
    /// Draws UI elements.
    /// </summary>
    public class UILayer : Layer
    {
        private static readonly Font uiFont = new Font(FontFamily.GenericSansSerif, 12);
        private static readonly StringFormat centeredStringFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        private Rectangle hungerBarRect;
        private Rectangle inventoryRect;
        private RectangleF[] inventoryItemRectsOuter = new RectangleF[Player.INVENTORY_LENGTH];
        private RectangleF[] inventoryItemRectsInner = new RectangleF[Player.INVENTORY_LENGTH];
        private Pen borderPen = new Pen(Color.Black, 2);
        private Pen selectionBorderPen = new Pen(Color.Black, 4);
        private RectangleF previousVisibleClipBounds;

        public UILayer(WorldView worldView) : base(worldView)
        { }

        public override void Draw(Graphics graphics)
        {
            Player player = worldView.Player;

            if (graphics.VisibleClipBounds != previousVisibleClipBounds)
            {
                CalculateUIElementPositions(graphics.VisibleClipBounds);
                previousVisibleClipBounds = graphics.VisibleClipBounds;
            }

            // Draw hunger bar
            graphics.FillRectangle(Brushes.White, hungerBarRect);
            graphics.FillRectangle(Brushes.SaddleBrown,
                hungerBarRect.X,
                hungerBarRect.Y,
                Convert.ToInt32(hungerBarRect.Width * player.Hunger),
                hungerBarRect.Height);
            graphics.DrawRectangle(borderPen, hungerBarRect);
            graphics.DrawString("Hunger", uiFont, Brushes.Black, hungerBarRect, centeredStringFormat);

            // Draw Inventory
            graphics.FillRectangle(Brushes.White, inventoryRect);

            RectangleF outlineRect;
            Pen inventorySlotPen;

            for (int i = 0; i < player.Inventory.Length; i++)
            {
                outlineRect = inventoryItemRectsOuter[i];

                if (i == player.SelectedInventoryIndex)
                    inventorySlotPen = selectionBorderPen;
                else
                    inventorySlotPen = borderPen;

                graphics.DrawRectangle(inventorySlotPen, outlineRect.X, outlineRect.Y, outlineRect.Width, outlineRect.Height);

                if (player.Inventory[i] != null)
                    graphics.DrawImage(player.Inventory[i].CurrentImage, inventoryItemRectsInner[i]);
            }
        }

        private void CalculateUIElementPositions(RectangleF bounds)
        {
            float width = bounds.Width;
            float height = bounds.Height;

            // Inventory
            if (width < 400)
            {
                inventoryRect.Width = 400;
                inventoryRect.X = 0;
            }
            else if (width < 700)
            {
                inventoryRect.Width = Convert.ToInt32(width);
                inventoryRect.X = 0;
            }
            else
            {
                inventoryRect.Width = 700;
                inventoryRect.X = Convert.ToInt32((width - inventoryRect.Width) / 2);
            }

            inventoryRect.Height = inventoryRect.Width / worldView.Player.Inventory.Length;
            inventoryRect.Y = Convert.ToInt32(height - inventoryRect.Height);

            float margin = inventoryRect.Height * 0.1f;

            for (int i = 0; i < inventoryItemRectsOuter.Length; i++)
            {
                inventoryItemRectsOuter[i].Width = inventoryRect.Width / (float)inventoryItemRectsOuter.Length;
                inventoryItemRectsOuter[i].Height = inventoryRect.Height;
                inventoryItemRectsOuter[i].X = inventoryRect.X + i * inventoryItemRectsOuter[i].Width;
                inventoryItemRectsOuter[i].Y = inventoryRect.Y;

                inventoryItemRectsInner[i].Width = inventoryItemRectsOuter[i].Width - 2 * margin;
                inventoryItemRectsInner[i].Height = inventoryItemRectsInner[i].Width;
                inventoryItemRectsInner[i].X = inventoryItemRectsOuter[i].X + margin;
                inventoryItemRectsInner[i].Y = inventoryItemRectsOuter[i].Y + margin;
            }

            // Hunger bar
            hungerBarRect.Width = Convert.ToInt32(inventoryRect.Width * 0.4375);
            hungerBarRect.Height = Convert.ToInt32(hungerBarRect.Width / 8.0);
            hungerBarRect.X = inventoryRect.X;
            hungerBarRect.Y = Convert.ToInt32(inventoryRect.Y - margin - hungerBarRect.Height);
        }

        public override bool HandleClick(MouseEventArgs e)
        {
            bool handled;

            if (inventoryRect.Contains(e.Location))
            {
                for (int i = 0; i < inventoryItemRectsOuter.Length; i++)
                {
                    if (inventoryItemRectsOuter[i].Contains(e.Location))
                    {
                        worldView.Player.SelectedInventoryIndex = i;
                        break;
                    }
                }

                handled = true;
            }
            else if (hungerBarRect.Contains(e.Location))
            {
                handled = true;
            }
            else
            {
                handled = false;
            }

            return handled;
        }
    }
}
