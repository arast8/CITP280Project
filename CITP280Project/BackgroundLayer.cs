using System.Windows.Forms;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace CITP280Project
{
    /// <summary>
    /// Renders Tiles. It shows an area of the world always centered on the character,
    /// so it tracks when the character moves.
    /// </summary>
    public class BackgroundLayer : Layer
    {
        private int tileSize = 80; // pixels per tile
        public CartesianRect visibleArea;
        private World world;

        public BackgroundLayer(World world , Player player, int width, int height)
            : base(player, width, height)
        {
            this.world = world;
            player.Moved += Player_Moved;
        }

        public override Bitmap Draw()
        {
            int worldX, worldY, imageX, imageY;
            Material material;

            Point worldStart = new Point();
            Point imageStart = new Point();

            // Converts between the coordinate systems of the game world and the window/image
            // by calculating where to draw the upper-left-most tile and then incrementing from there in a loop
            // to get each relevant Tile and draw its image.
            worldStart.X = Convert.ToInt32(Math.Floor(visibleArea.Left));
            worldStart.Y = Convert.ToInt32(Math.Ceiling(visibleArea.Top)) - 1;
            imageStart.X = Convert.ToInt32(Math.Floor((worldStart.X - visibleArea.Left) * tileSize));
            imageStart.Y = Convert.ToInt32(Math.Ceiling((visibleArea.Top - worldStart.Y) * tileSize)) - tileSize;

            for (worldY = worldStart.Y, imageY = imageStart.Y; imageY < CurrentImage.Height; worldY--, imageY += tileSize)
            {
                for (worldX = worldStart.X, imageX = imageStart.X; imageX < CurrentImage.Width; worldX++, imageX += tileSize)
                {
                    material = world.GetMaterial(worldX, worldY);

                    graphics.DrawImage(material.CurrentImage, imageX, imageY, tileSize, tileSize);
                }
            }

            return CurrentImage;
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            visibleArea.Width = (double)width / tileSize;
            visibleArea.Height = (double)height / tileSize;
            visibleArea.X = player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = player.Location.Y - visibleArea.Height / 2;
        }

        private void Player_Moved(object sender, EventArgs e)
        {
            visibleArea.X = player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = player.Location.Y - visibleArea.Height / 2;
        }
    }
}
