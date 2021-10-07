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
        Dictionary<Point, Chunk> chunks;

        public BackgroundLayer(Dictionary<Point, Chunk> chunks, Character character, int width, int height)
            : base(character, width, height)
        {
            this.chunks = chunks;
            character.Move += Character_Move;
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            visibleArea.Width = (double)width / tileSize;
            visibleArea.Height = (double)height / tileSize;
            visibleArea.X = character.Location.X - visibleArea.Width / 2;
            visibleArea.Y = character.Location.Y - visibleArea.Height / 2;
        }

        private void Character_Move(object sender, EventArgs e)
        {
            visibleArea.X = character.Location.X - visibleArea.Width / 2;
            visibleArea.Y = character.Location.Y - visibleArea.Height / 2;
        }

        public override Bitmap Draw()
        {
            int worldX, worldY, imageX, imageY;
            Tile tile;

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
                    tile = WorldMap.GetTile(chunks, worldX, worldY);

                    graphics.DrawImage(tile.CurrentImage, imageX, imageY, tileSize, tileSize);
                }
            }

            return CurrentImage;
        }
    }
}
