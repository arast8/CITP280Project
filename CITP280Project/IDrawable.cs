using System.Drawing;

namespace CITP280Project
{
    /// <summary>
    /// IDrawable is implemented by things that should have a Bitmap image associated with them.
    /// </summary>
    internal interface IDrawable
    {
        Bitmap CurrentImage { get; }
    }
}
