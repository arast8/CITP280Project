using System.Windows.Forms;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace CITP280Project
{
    /// <summary>
    /// Draws the ground.
    /// </summary>
    public class BackgroundLayer : Layer
    {
        public BackgroundLayer(WorldView worldView) : base(worldView)
        { }

        public override void Draw(Graphics graphics)
        {
            World world = worldView.World;
            CartesianRect visibleArea = worldView.VisibleArea;
            int tileSize = worldView.TileSize;

            int worldX, worldY, imageX, imageY, worldStartX, worldStartY, imageStartX, imageStartY;
            Material material;

            // Converts between the coordinate systems of the game world and the window/image
            // by calculating where to draw the upper-left-most tile and then incrementing from there in a loop
            // to get the Material at each location and draw its image.
            worldStartX = Convert.ToInt32(Math.Floor(visibleArea.Left));
            worldStartY = Convert.ToInt32(Math.Ceiling(visibleArea.Top)) - 1;
            imageStartX = Convert.ToInt32(Math.Floor((worldStartX - visibleArea.Left) * tileSize));
            imageStartY = Convert.ToInt32(Math.Ceiling((visibleArea.Top - worldStartY) * tileSize)) - tileSize;

            for (worldY = worldStartY, imageY = imageStartY; imageY < graphics.VisibleClipBounds.Height; worldY--, imageY += tileSize)
            {
                for (worldX = worldStartX, imageX = imageStartX; imageX < graphics.VisibleClipBounds.Width; worldX++, imageX += tileSize)
                {
                    material = world.GetGroundMaterial(worldX, worldY);

                    if (material != null)
                        graphics.DrawImage(material.CurrentImage, imageX, imageY, tileSize, tileSize);
                }
            }
        }

        public override bool HandleClick(MouseEventArgs e)
        {
            var worldLocation = worldView.ToWorldLocation(e.Location);
            var player = worldView.Player;
            var world = worldView.World;

            if (e.Button == MouseButtons.Left)
            { // dig ground material
                var material = world.GetGroundMaterial(worldLocation);

                if (material != null && material.CanBeGround)
                {
                    player.Inventory[player.SelectedInventoryIndex] = material;
                    world.SetGroundMaterial(worldLocation, null);
                    return true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            { // place ground material
                var material = player.Inventory[player.SelectedInventoryIndex];

                if (material != null && material.CanBeGround)
                {
                    world.SetGroundMaterial(worldLocation, material);
                    return true;
                }
            }

            return false;
        }
    }
}
