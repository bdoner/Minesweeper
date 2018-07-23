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
                            var foundCell = new Cell(x, y);

                            if (Filters.UnclickedCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Unclicked;
                            }
                            else if (Filters.EmptyCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Empty;
                                foundCell.Value = CellValue.None;
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
                            else if (Filters.ValueFiveCell.IsMatch(windowPicture, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Five;
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
                windowPicture.Save($"ms_matches_m-{matchCount}.bmp");
            }

            Console.WriteLine($"Found {matchCount} border-pixels");

            foreach (var cell in cells)
            {
                var cellPosX = rect.Left + cell.X + Cell.CELL_SIZE / 2;
                var cellPosY = rect.Top + cell.Y + Cell.CELL_SIZE / 2;

                SetCursorPos(cellPosX, cellPosY);

                int lParam = (cellPosY << 16) + cellPosX;

                var inputMouseDown = new INPUT();
                inputMouseDown.Type = 0; /// input type mouse
                inputMouseDown.Data.Mouse.Flags = MOUSEEVENTF_RIGHTDOWN; /// left button down

                var inputMouseUp = new INPUT();
                inputMouseUp.Type = 0; /// input type mouse
                inputMouseUp.Data.Mouse.Flags = MOUSEEVENTF_RIGHTUP; /// left button up

                var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
                SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
                
                Thread.Sleep(100);
            }

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
