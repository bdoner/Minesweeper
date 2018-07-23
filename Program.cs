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
            var startTime = DateTime.Now;
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
                for (var y = 100; y < imageHeight; y++)
                {
                    for (var x = 0; x < imageWidth; x++)
                    {
                        if (
                            Filters.Border.IsMatch(windowPicture, imageWidth, imageHeight, x, y, null) ||
                            Filters.ClickedBorder.IsMatch(windowPicture, imageWidth, imageHeight, x, y, null)
                            )
                        {
                            matchCount++;
                            Console.WriteLine($"Found border at ({x}, {y})");
                            var foundCell = new Cell { X = x, Y = y };

                            if (Filters.UnclickedCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Unclicked;
                            }
                            else if (Filters.EmptyCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Empty;
                            }
                            else if (Filters.ValueOneCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.One;
                            }
                            else if (Filters.ValueTwoCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Two;
                            }
                            else if (Filters.ValueThreeCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Three;
                            }
                            else if (Filters.ValueFourCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Four;
                            }
                            else if (Filters.FlaggedCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Flagged;
                            }
                            else if (Filters.UnknownCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Unknown;
                            }

                            cells.Add(foundCell);

                            x += Cell.CELL_SIZE;
                        }
                    }
                }
                windowPicture.Save($"ms_matches_t1-{Filters.Border.Threshold}_t2-{Filters.ClickedBorder.Threshold}_m-{matchCount}.bmp");
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
            var timeTaken = DateTime.Now - startTime;
            Console.WriteLine($"Execution complete in {timeTaken.TotalMilliseconds}ms");
            Thread.Sleep(5000);
            //Console.ReadLine();
        }

    }
}
