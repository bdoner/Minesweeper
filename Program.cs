using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using static Minesweeper.Externals;

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr gameWindow = FindWindow(default(string), "Minesweeper");
            if (gameWindow == IntPtr.Zero) Environment.Exit(1);

            HandleRef gameWindowHandle = new HandleRef(default(object), gameWindow);
            //Get window

            var rect = new RECT();
            var rectSuccess = GetWindowRect(new HandleRef(default(object), gameWindow), out rect);
            //Get window size

            var topSuccess = SetForegroundWindow(gameWindow);// SetWindowPos(gameWindow, (IntPtr)0, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.ShowWindow);
            //Bring to top

            var windowPicture = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (var g = Graphics.FromImage(windowPicture))
            {
                g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));

            }
            windowPicture.Save("ms.bmp");
            //Grab picture

            int borderCount = 0;
            var borderFilter = new PixelFilter
            {
                Threshold = 32,
                Filter = new List<Pixel>
                {
                    //Above
                    new Pixel
                    {
                        ColorVal = 0x06,
                        RelX = -1,
                        RelY = -1
                    },
                    new Pixel
                    {
                        ColorVal = 0x0f,
                        RelX = 0,
                        RelY = -1
                    },
                    new Pixel
                    {
                        ColorVal = 0x0e,
                        RelX = 1,
                        RelY = -1
                    },

                    //Same row
                    new Pixel
                    {
                        ColorVal = 0x0c,
                        RelX = -1,
                        RelY = 0
                    },
                    //new Pixel
                    //{
                    //    ColorVal = 0x64,
                    //    RelX = 0,
                    //    RelY = 0
                    //},
                    //new Pixel
                    //{
                    //    ColorVal = 0x7a,
                    //    RelX = 1,
                    //    RelY = 0
                    //},

                    //Below
                    new Pixel
                    {
                        ColorVal = 0x0e,
                        RelX = -1,
                        RelY = 1
                    },
                    //new Pixel
                    //{
                    //    ColorVal = 0x78,
                    //    RelX = 0,
                    //    RelY = 1
                    //},
                    //new Pixel
                    //{
                    //    ColorVal = 0xac,
                    //    RelX = 1,
                    //    RelY = 1
                    //},

                    //Check opposite
                    new Pixel
                    {
                        ColorVal = 0x02,
                        RelX = 30,
                        RelY = 30
                    }
                }
            };

            var g2 = Graphics.FromImage(windowPicture);
            for (var y = 0; y < windowPicture.Height; y++)
            {
                for (var x = 0; x < windowPicture.Width; x++)
                {
                    var pixel = windowPicture.GetPixel(x, y);
                    if (borderFilter.IsMatch(ref windowPicture, x, y))
                    {
                        borderCount++;
                        Console.WriteLine($"Found border at ({x}, {y})");
                        g2.DrawRectangle(Pens.Red, x, y, 29, 29);

                        x += 25;
                    }
                }
            }
            windowPicture.Save("ms_drawn.bmp");
            g2.Dispose();

            Console.WriteLine($"Found {borderCount} border-pixels");
            //Get board rect
            // - main loop
            //  picture board
            //  parse board
            //  find mines
            //  mark mines
            // - Repeat main loop

            Console.WriteLine("Done...");
            Thread.Sleep(5000);
            //Console.ReadLine();
        }

    }
}
