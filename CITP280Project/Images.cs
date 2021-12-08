using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public static class Images
    {
        private const string PATH_TO_IMAGES = "../../Resources/";
        private static Random rng = new Random();

        public static Bitmap Unknown { get; private set; }
        public static Bitmap Dirt { get; private set; }
        public static Bitmap Stone { get; private set; }
        public static Bitmap PlayerFacingLeft { get; private set; }
        public static Bitmap PlayerFacingRight { get; private set; }
        public static Bitmap StoneFlower { get; private set; }
        public static Bitmap Grass { get; private set; }
        public static Bitmap Wheat { get; private set; }

        // State information
        public static bool IsLoaded { get; private set; } = false;
        public static int Successes { get; private set; } = 0;
        public static int Errors { get; private set; } = 0;

        /// <summary>
        /// Loads all images needed by the game. The results of loading the images
        /// are in the Successes and Errors properties.
        /// </summary>
        public static void Initialize()
        {
            Unknown = TryGetBitmap("Unknown.png");
            Dirt = TryGetBitmap("Dirt.png");
            Stone = TryGetBitmap("Stone.png");
            PlayerFacingRight = TryGetBitmap("Player.png");
            StoneFlower = TryGetBitmap("StoneFlower.png");
            Grass = TryGetBitmap("Grass.png");
            Wheat = TryGetBitmap("Wheat.png");

            PlayerFacingLeft = (Bitmap)PlayerFacingRight.Clone();
            PlayerFacingLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);

            IsLoaded = true;
        }

        /// <summary>
        /// Tries to load an image with the specified name from the hardcoded directory pathToImages.
        /// On success, returns the loaded Bitmap and increments Successes.
        /// On failure, returns a new Bitmap with a solid random color and increments Errors.
        /// </summary>
        /// <remarks>name must include the file extension.</remarks>
        private static Bitmap TryGetBitmap(string name)
        {
            Bitmap bitmap;

            try
            {
                bitmap = new Bitmap(PATH_TO_IMAGES + name);

                Successes++;
            }
            catch (Exception e) when (e is ArgumentException || e is IOException)
            { // The Bitmap constructor throws ArgumentException when the file does not exist.
                Errors++;

                bitmap = new Bitmap(16, 16);
                Graphics.FromImage(bitmap).Clear(Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256)));
            }

            return bitmap;
        }
    }
}
