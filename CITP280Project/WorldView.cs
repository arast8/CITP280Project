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
    public class WorldView
    {
        private const int TILE_SIZE_MAX = 150;
        private const int TILE_SIZE_MIN = 30;
        private const int TILE_SIZE_STEP = 10;

        private readonly Layer[] layers;
        private CartesianRect visibleArea;
        private Graphics graphics;

        public Player Player { get; set; }
        public CartesianRect VisibleArea { get => visibleArea; }
        public World World { get; private set; }
        public int TileSize { get; private set; } = 80; // pixels

        public WorldView(World world, Player player, Graphics graphics)
        {
            World = world;
            Player = player;

            layers = new Layer[] {
                new BackgroundLayer(this),
                new ForegroundLayer(this),
                new UILayer(this),
            };

            SetGraphics(graphics);

            player.Moved += Player_Moved;
        }

        public void Draw()
        {
            graphics.Clear(Color.Black);

            foreach (Layer layer in layers)
                layer.Draw(graphics);
        }

        public void SetGraphics(Graphics graphics)
        {
            this.graphics = graphics;

            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            CalculateVisibleArea();
        }

        private void CalculateVisibleArea()
        {
            visibleArea.Width = (double)graphics.VisibleClipBounds.Width / TileSize;
            visibleArea.Height = (double)graphics.VisibleClipBounds.Height / TileSize;
            visibleArea.X = Player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = Player.Location.Y - visibleArea.Height / 2;
        }

        private void Player_Moved(object sender, Point<double> location, int heading)
        {
            visibleArea.X = Player.Location.X - visibleArea.Width / 2;
            visibleArea.Y = Player.Location.Y - visibleArea.Height / 2;
        }

        /// <summary>
        /// Converts coordinates on GameWindow to World coordinates.
        /// </summary>
        public Point<double> ToWorldLocation(Point locationOnWindow)
        {
            return new Point<double>(
                visibleArea.Left + locationOnWindow.X / (double)TileSize,
                visibleArea.Top - locationOnWindow.Y / (double)TileSize);
        }

        public void ZoomIn()
        {
            if (TileSize + TILE_SIZE_STEP <= TILE_SIZE_MAX)
                TileSize += TILE_SIZE_STEP;

            CalculateVisibleArea();
        }

        public void ZoomOut()
        {
            if (TileSize - TILE_SIZE_STEP >= TILE_SIZE_MIN)
                TileSize -= TILE_SIZE_STEP;

            CalculateVisibleArea();
        }
    }
}
