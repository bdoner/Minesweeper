using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static Minesweeper.Externals;

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var winmine = System.Diagnostics.Process.GetProcessesByName("Winmine__XP");
            if (winmine.Length == 0)
            {
                Console.WriteLine("\"Winmine__XP.exe\" not found running. Please run a game of Minesweeper and try again.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            IntPtr gameWindow = winmine[0].MainWindowHandle;
            HandleRef gameWindowHandle = new HandleRef(default(object), gameWindow);
            //Get window

            var topSuccess = SetForegroundWindow(gameWindow);
            //SetWindowPos(gameWindow, (IntPtr)0, 0, 0, 600, 600, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreZOrder);
            //Bring to top

            var rect = new RECT();
            var rectSuccess = GetWindowRect(new HandleRef(default(object), gameWindow), out rect);
            //Get window size


            var windowPicture = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (var g = Graphics.FromImage(windowPicture))
            {
                g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));

            }
            windowPicture.Save("ms.bmp");
            //Grab picture

            var cells = new List<Cell>();
            int matchCount = 0;
            int imageWidth = rect.Right - rect.Left;
            int imageHeight = rect.Bottom - rect.Top;
            using (var g2 = Graphics.FromImage(windowPicture))
            {
                for (var y = 0; y < imageHeight; y++)
                {
                    for (var x = 0; x < imageWidth; x++)
                    {
                        if (
                            Filters.BorderFilter.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2) ||
                            Filters.ClickedBorderFilter.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2)
                            )
                        {
                            matchCount++;
                            Console.WriteLine($"Found border at ({x}, {y})");
                            x += Cell.CELL_SIZE;
                            cells.Add(new Cell { X = x, Y = y });
                        }
                    }
                }
                windowPicture.Save($"ms_matches_t1-{Filters.BorderFilter.Threshold}_t2-{Filters.ClickedBorderFilter.Threshold}_m-{matchCount}.bmp");
            }

            Console.WriteLine($"Found {matchCount} border-pixels");
            //Get board state

            // - main loop
            //  picture board
            //  parse board
            //  find mines
            //  mark mines
            // - Repeat main loop

            Console.WriteLine("Done...");
            //Thread.Sleep(5000);
            //Console.ReadLine();
        }

    }
}
