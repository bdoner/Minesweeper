using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public struct Pixel
    {
        public int RelX;
        public int RelY;
        public byte ColorVal;
    }

    public class PixelFilter
    {
        public List<Pixel> Filter { get; set; }
        public int Threshold { get; set; } = 0;
        public bool IsMatch(ref Bitmap bitmap, int sourceX, int sourceY)
        {
            foreach (var f in Filter)
            {
                var fX = sourceX + f.RelX;
                if (fX < 0) return false;
                if (fX >= bitmap.Width) return false;

                var fY = sourceY + f.RelY;
                if (fY < 0) return false;
                if (fY >= bitmap.Height) return false;

                var p = bitmap.GetPixel(fX, fY);
                var minCol = Math.Max(0, Math.Min(255, f.ColorVal - Threshold));
                var maxCol = Math.Max(0, Math.Min(255, f.ColorVal + Threshold));
                if (!(p.G > minCol && p.G < maxCol))
                    return false;
            }
            return true;
        }
    }
}
