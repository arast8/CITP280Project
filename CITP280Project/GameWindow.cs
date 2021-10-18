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
    public partial class GameWindow : Form
    {
        private World world;
        private WorldView worldView;
        private Player player;

        public GameWindow()
        {
            InitializeComponent();
            Images.Initialize(out var e);
            Material.Initialize();

            world = new World();
            player = world.CreatePlayer("test character");
            worldView = new WorldView(world, player, ClientRectangle.Width, ClientRectangle.Height);

            timerTick.Start();
        }

        /// <summary>
        /// Triggers regular processing activities, which include:
        /// Starting or stopping character movement
        /// Drawign the result of worldView.GetImage() to the window.
        /// </summary>
        private void timerTick_Tick(object sender, EventArgs e)
        {
            world.timerTick_Tick();

            CreateGraphics().DrawImage(worldView.GetImage(), Point.Empty);

            //lblDebug.Text = "View Area: " + ((BackgroundLayer)layers[0]).visibleArea;
        }

        /// <summary>
        /// Resizes the WorldView.
        /// </summary>
        private void GameWindow_Resize(object sender, EventArgs e)
        {
            worldView.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        private void GameWindow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (IsKeyDown(KeyMap.Up) || IsKeyDown(KeyMap.Down) || IsKeyDown(KeyMap.Left) || IsKeyDown(KeyMap.Right))
            {
                player.StartMove();

                e.Handled = true;
            }
        }

        private void GameWindow_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!(IsKeyDown(KeyMap.Up) || IsKeyDown(KeyMap.Down) || IsKeyDown(KeyMap.Left) || IsKeyDown(KeyMap.Right)))
            {
                player.StopMove();

                e.Handled = true;
            }
        }
    }
}
