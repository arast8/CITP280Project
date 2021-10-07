using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Input.Keyboard;

namespace CITP280Project
{
    /// <summary>
    /// The main game window. It runs a timer that triggers game events to happen every few milliseconds.
    /// It causes layers to be redrawn, combined, and drawn to the window.
    /// It also responds to keypresses by telling the character to move.
    /// </summary>
    public partial class Game : Form
    {
        private Graphics bufferGraphics;
        private Graphics formGraphics;
        private Character character;
        private Layer[] layers;
        private Bitmap bufferImage;
        private Dictionary<Point, Chunk> chunks;

        public Game()
        {
            InitializeComponent();
            character = new Character("test character");
            chunks = new Dictionary<Point, Chunk>();

            layers = new Layer[] {
                new BackgroundLayer(chunks, character, ClientRectangle.Width, ClientRectangle.Height),
                new ForegroundLayer(character, ClientRectangle.Width, ClientRectangle.Height),
                new UILayer(character, ClientRectangle.Width, ClientRectangle.Height),
            };

            timerTick.Start();

            bufferImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            bufferGraphics = Graphics.FromImage(bufferImage);
        }

        /// <summary>
        /// Redraws all layers, combines them onto the bufferImage, and draws the bufferImage to the window.
        /// </summary>
        public void Draw()
        {
            foreach (Layer layer in layers)
                bufferGraphics.DrawImage(layer.Draw(), Point.Empty);

            formGraphics = CreateGraphics();
            formGraphics.DrawImage(bufferImage, Point.Empty);
        }

        /// <summary>
        /// Is called every few milliseconds and triggers regular processing activities.
        /// </summary>
        private void timerTick_Tick(object sender, EventArgs e)
        {
            character.KeyboardMove();

            Draw();

            lblDebug.Text = "View Area: " + ((BackgroundLayer)layers[0]).visibleArea;
        }

        /// <summary>
        /// Resizes bufferImage and all the layers when the window is resized.
        /// </summary>
        private void Game_Resize(object sender, EventArgs e)
        {
            bufferImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            bufferGraphics = Graphics.FromImage(bufferImage);

            foreach (Layer layer in layers)
                layer.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        private void Game_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (IsKeyDown(KeyMap.Up) || IsKeyDown(KeyMap.Down) || IsKeyDown(KeyMap.Left) || IsKeyDown(KeyMap.Right))
            {
                if (!character.isMoving)
                    character.StartMove();

                e.Handled = true;
            }
        }

        private void Game_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!(IsKeyDown(KeyMap.Up) || IsKeyDown(KeyMap.Down) || IsKeyDown(KeyMap.Left) || IsKeyDown(KeyMap.Right)))
            {
                if (character.isMoving)
                    character.StopMove();

                e.Handled = true;
            }
        }
    }
}
