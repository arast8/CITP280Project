using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;
using PointD = System.Windows.Point;

namespace CITP280Project
{
    /// <summary>
    /// WorldView is responsible for creating an image of a World to show the user.
    /// It is composed of different Layers, which draw different types of game visuals.
    /// </summary>
    public class WorldView : IDrawable
    {
        private Layer[] layers;
        private CartesianRect visibleArea;

        public Bitmap CurrentImage { get; set; }
        public Player Player { get; set; }
        public Graphics Graphics { get; private set; }
        public CartesianRect VisibleArea { get => visibleArea; }
        public World World { get; private set; }
        public int TileSize { get; private set; } = 80; // pixels

        public WorldView(World world, Player player, int width, int height)
        {
            World = world;
            Player = player;

            layers = new Layer[] {
                new BackgroundLayer(this),
                new ForegroundLayer(this),
                new UILayer(this),
            };

            Resize(width, height);

            player.Moved += Player_Moved;
        }

        public Bitmap GetImage()
        {
            Graphics.Clear(Color.Black);

            foreach (Layer layer in layers)
                layer.Draw();

            return CurrentImage;
        }

        public void Resize(int width, int height)
        {
            CurrentImage?.Dispose();
            Graphics?.Dispose();
            CurrentImage = new Bitmap(width, height);
            Graphics = Graphics.FromImage(CurrentImage);
            Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            visibleArea.Width = (double)width / TileSize;
            visibleArea.Height = (double)height / TileSize;
            visibleArea.X = Player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = Player.Location.Y - visibleArea.Height / 2;
        }

        private void Player_Moved(object sender, EventArgs e)
        {
            visibleArea.X = Player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = Player.Location.Y - visibleArea.Height / 2;
        }

        /// <summary>
        /// Converts coordinates on GameWindow to World coordinates.
        /// </summary>
        public PointD ToWorldLocation(Point locationOnWindow)
        {
            return new PointD(
                visibleArea.Left + locationOnWindow.X / (double)TileSize,
                visibleArea.Top - locationOnWindow.Y / (double)TileSize);
        }
    }
}
