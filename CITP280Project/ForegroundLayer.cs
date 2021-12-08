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
    /// Draws things above the ground, including the Player.
    /// </summary>
    public class ForegroundLayer : Layer
    {
        public ForegroundLayer(WorldView worldView) : base(worldView)
        { }

        public override void Draw(Graphics graphics)
        {
            World world = worldView.World;
            CartesianRect visibleArea = worldView.VisibleArea;
            Player player = worldView.Player;
            int tileSize = worldView.TileSize;

            // Calculates player image coordinates in a way that the player is always in the center of the screen.
            int playerImageX = Convert.ToInt32(graphics.VisibleClipBounds.Width / 2.0 - tileSize / 2.0);
            int playerImageY = Convert.ToInt32(graphics.VisibleClipBounds.Height / 2.0 - tileSize / 2.0);
            bool playerIsDrawn = false;


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
                if (!playerIsDrawn && imageY > playerImageY)
                {
                    graphics.DrawImage(player.CurrentImage, playerImageX, playerImageY, tileSize, tileSize);
                    playerIsDrawn = true;
                }

                for (worldX = worldStartX, imageX = imageStartX; imageX < graphics.VisibleClipBounds.Width; worldX++, imageX += tileSize)
                {
                    material = world.GetPlayerLevelMaterial(worldX, worldY);

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
            { // dig player-level material
                var playerLevelMaterial = world.GetPlayerLevelMaterial(worldLocation);

                if (playerLevelMaterial != null && playerLevelMaterial.CanBePlayerLevel)
                {
                    player.Inventory[player.SelectedInventoryIndex] = playerLevelMaterial;
                    world.SetPlayerLevelMaterial(worldLocation, null);
                    return true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            { // place player-level material
                var inventoryMaterial = player.Inventory[player.SelectedInventoryIndex];
                var groundMaterial = world.GetGroundMaterial(worldLocation);

                if (inventoryMaterial != null && inventoryMaterial.CanBePlayerLevel && groundMaterial != null)
                {
                    world.SetPlayerLevelMaterial(worldLocation, inventoryMaterial);
                    return true;
                }
            }

            return false;
        }
    }
}
