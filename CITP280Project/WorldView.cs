using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public class WorldView : IDrawable
    {
        private Player player;
        private Layer[] layers;
        private World world;
        private Graphics graphics;

        public Bitmap CurrentImage { get; set; }

        public WorldView(World world, Player player, int width, int height)
        {
            this.world = world;
            this.player = player;

            layers = new Layer[] {
                new BackgroundLayer(world, player, width, height),
                new ForegroundLayer(player, width, height),
                new UILayer(player, width, height),
            };

            Resize(width, height);
        }

        public Bitmap GetImage()
        {
            foreach (Layer layer in layers)
                graphics.DrawImage(layer.Draw(), Point.Empty);

            return CurrentImage;
        }
        
        public void Resize(int width, int height)
        {
            CurrentImage?.Dispose();
            graphics?.Dispose();
            CurrentImage = new Bitmap(width, height);
            graphics = Graphics.FromImage(CurrentImage);

            foreach (Layer layer in layers)
                layer.Resize(width, height);
        }
    }
}
