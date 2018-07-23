using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    [DebuggerDisplay("Point=({RelX}, {RelY}), Color=({R}, {G}, {B})")]
    public struct Pixel
    {
        public int RelX;
        public int RelY;
        public byte R;
        public byte G;
        public byte B;

        public Pixel(int relX, int relY, int hexArgbColor)
        {
            var col = Color.FromArgb(hexArgbColor);

            R = col.R;
            G = col.G;
            B = col.B;

            RelX = relX;
            RelY = relY;
        }
    }

    public class PixelFilter
    {
        public List<Pixel> Filter { get; set; }
        public int Threshold { get; set; } = 0;
        public bool IsMatch(Bitmap bitmap, int imageWidth, int imageHeight, int sourceX, int sourceY, Graphics g = null)
        {
            foreach (var f in Filter)
            {
                var fX = sourceX + f.RelX;
                if (fX < 0) return false;
                if (fX >= imageWidth) return false;

                var fY = sourceY + f.RelY;
                if (fY < 0) return false;
                if (fY >= imageHeight) return false;

                var p = bitmap.GetPixel(fX, fY);
                var minR = Math.Max(0, Math.Min(255, f.R - Threshold));
                var maxR = Math.Max(0, Math.Min(255, f.R + Threshold));
                if (!(p.R >= minR && p.R <= maxR))
                    return false;

                var minG = Math.Max(0, Math.Min(255, f.G - Threshold));
                var maxG = Math.Max(0, Math.Min(255, f.G + Threshold));
                if (!(p.G >= minG && p.G <= maxG))
                    return false;

                var minB = Math.Max(0, Math.Min(255, f.B - Threshold));
                var maxB = Math.Max(0, Math.Min(255, f.B + Threshold));
                if (!(p.B >= minB && p.B <= maxB))
                    return false;
            }

            if (g != null)
            {

                foreach (var f in Filter)
                {
                    var fX = sourceX + f.RelX;
                    var fY = sourceY + f.RelY;

                    g.FillRectangle(Brushes.Orange, fX, fY, 1, 1);
                }
            }

            return true;
        }
    }
}
