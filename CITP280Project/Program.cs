using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITP280Project
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var gameWindow = new GameWindow();

            try
            {
                Application.Run(gameWindow);
            }
            catch (SqliteException ex)
            {
                gameWindow.TryDisposeWorld(false);

                MessageBox.Show(
                    "The program encountered a database error and must close.\n Exception: " + ex.Message,
                    "CITP 280 Project",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }
        }
    }
}
