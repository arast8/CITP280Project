using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITP280Project
{
    public static class Images
    {
        private static string pathToImages = "../../Resources";
        public static Bitmap Unknown { get; private set; }
        public static Bitmap Dirt { get; private set; }
        public static Bitmap Stone { get; private set; }
        public static Bitmap PlayerFacingLeft { get; private set; }
        public static Bitmap PlayerFacingRight { get; private set; }

        public static bool Initialize(out Exception e)
        {
            try
            {
                Unknown = new Bitmap(1, 1);
                Dirt = Properties.Resources.Dirt;
                PlayerFacingRight = Properties.Resources.Character;
                PlayerFacingLeft = (Bitmap)PlayerFacingRight.Clone();
                PlayerFacingLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Stone = new Bitmap(1, 1);

                e = null;
                return true;
            }
            catch (Exception ex)
            {
                e = ex;
                return false;
            }
        }
    }
}
