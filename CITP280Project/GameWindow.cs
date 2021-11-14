﻿using Microsoft.Data.Sqlite;
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
using Point = System.Drawing.Point;
using PointD = System.Windows.Point;

namespace CITP280Project
{
    /// <summary>
    /// The main game window. It runs a timer that triggers game events to happen every few milliseconds.
    /// It sets up the relationships between a World, Player, and WorldView object, and asks WorldView for images to draw to the window.
    /// It also responds to keypresses by telling the Player to start or stop moving.
    /// </summary>
    public partial class GameWindow : Form
    {
        private World world;
        private WorldView worldView;
        private Player player;

        public GameWindow()
        {
            InitializeComponent();

            lblLoading.Text = $"Loading {Images.COUNT} Images...";

            Images.Initialize();

            lblLoading.Text = $"Loaded {Images.Successes}/{Images.COUNT} Images.";

            if (Images.Errors == 0)
            {
                btnStart.BackColor = Color.FromArgb(0, 255, 0);
            }
            else
            {
                lblLoading.Text += $" There were {Images.Errors} errors.";
                btnStart.BackColor = Color.FromArgb(255, 255, 0);
            }

            Material.Initialize();
        }

        private void GameWindow_Layout(object sender, LayoutEventArgs e)
        {
            var margin = 8;

            lblLoading.Width = pnlMenu.Width - margin * 2;
            lblLoading.Location = new Point(
                margin,
                (pnlMenu.Height - lblLoading.Height) / 2);

            btnStart.Location = new Point(
                (pnlMenu.Width - btnStart.Width) / 2,
                pnlMenu.Bottom - margin - btnStart.Height);
        }

        private void GameWindow_Resize(object sender, EventArgs e)
        {
            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
                worldView.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            pnlMenu.Hide();
            Focus();
            lblDebug.Show();

            world = new World("test world");
            player = world.CreatePlayer("test player");
            worldView = new WorldView(world, player, ClientRectangle.Width, ClientRectangle.Height);

            timerTick.Start();
        }

        /// <summary>
        /// Triggers regular processing activities, which include
        /// drawing the result of worldView.GetImage() to the window.
        /// </summary>
        private void timerTick_Tick(object sender, EventArgs e)
        {
            world.timerTick_Tick();

            CreateGraphics().DrawImage(worldView.GetImage(), Point.Empty);

            var cursorLocation = worldView.ToWorldLocation(PointToClient(System.Windows.Forms.Cursor.Position));

            lblDebug.Text = "View Area: " + worldView.VisibleArea +
                $"\nCursor: {{ {cursorLocation.X:F}, {cursorLocation.Y:F} }}";
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

        private void GameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            TryDisposeWorld(true);
        }

        /// <summary>
        /// Tries to call Dispose() on the current world. If there is a database error and complain is true,
        /// show a MessageBox describing the error.
        /// </summary>
        /// <param name="complain">Whether to show a MessageBox if there is a database error</param>
        public void TryDisposeWorld(bool complain)
        {
            try
            {
                world?.Dispose();
            }
            catch (SqliteException ex)
            {
                if (complain)
                {
                    MessageBox.Show(
                    "The program encountered a database error while closing.\n Exception: " + ex.Message,
                    "CITP 280 Project",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
        }
    }
}
