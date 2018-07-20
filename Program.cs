using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Minesweeper.Externals;

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr gameWindow = FindWindow(default(string), "Minesweeper");
            HandleRef gameWindowHandle = new HandleRef(default(object), gameWindow);
            //Get window

            var rect = new RECT();
            var rectSuccess = GetWindowRect(new HandleRef(default(object), gameWindow), out rect);

            //var placement = new WINDOWPLACEMENT();
            //var placementSuccess = GetWindowPlacement(gameWindow, ref placement);

            var windowPicture = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (var g = Graphics.FromImage(windowPicture))
            {
                g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));

            }
            //Grab picture


            //Get board rect
            // - main loop
            //  picture board
            //  parse board
            //  find mines
            //  mark mines
            // - Repeat main loop
        }
        
    }
}
