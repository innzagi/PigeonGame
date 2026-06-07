using System.Drawing;
using System.Drawing.Drawing2D;

namespace PigeonGame.Helpers;

public static class BitmapHelper
{
    public static Bitmap LoadScaledBitmap(string path, int width, int height)
    {
        var raw = Image.FromFile(path);
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }
}
