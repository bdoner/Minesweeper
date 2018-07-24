﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Minesweeper.Externals;
using static Minesweeper.Program;

namespace Minesweeper
{
    public enum BoardState
    {
        Playing,
        Won,
        Lost
    }

    [DebuggerDisplay("State={State}, Size=({GridWidth}, {GridHeight})")]
    public class Board
    {
        private Random _rnd;
        public BoardState State { get; set; }
        public RECT WindowLocation { get; set; }
        public List<Cell> Cells { get; set; } = new List<Cell>();
        public bool IsNewGame { get { return Cells.All(c => c.State == CellState.Unclicked); } }

        public int GridWidth
        {
            get
            {
                return Cells.Max(q => q.Col);
            }
        }

        public int GridHeight
        {
            get
            {
                return Cells.Max(q => q.Row);
            }
        }

        public Board()
        {
            WindowLocation = GetWindowLocation();
            UpdateBoard();
            _rnd = new Random();
            if (Cells.Count != GridWidth * GridHeight)
            {
                throw new Exception($"Parsed {Cells.Count} cells but expected {GridWidth * GridHeight} cells.");
            }
        }

        public IntPtr WinmineIsProcessHandle()
        {
            var winmine = Process.GetProcessesByName("Winmine__XP");
            return winmine.Length == 1 ? winmine[0].MainWindowHandle : IntPtr.Zero;

        }

        private RECT GetWindowLocation()
        {
            var gameWindow = WinmineIsProcessHandle();
            if (gameWindow == IntPtr.Zero)
            {
                Console.WriteLine("\"Winmine__XP.exe\" not found running. Please run a game of Minesweeper and try again. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            HandleRef gameWindowHandle = new HandleRef(default(object), gameWindow);
            //Get window

            var topSuccess = SetForegroundWindow(gameWindow);
            //SetWindowPos(gameWindow, (IntPtr)0, 0, 0, 600, 600, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreZOrder);
            //Bring to top

            var rect = new RECT();
            var rectSuccess = GetWindowRect(new HandleRef(default(object), gameWindow), out rect);
            //Get window size

            return rect;
        }

        private Bitmap GetBoardImage(RECT windowLocation)
        {
            var boardImage = new Bitmap(windowLocation.Right - windowLocation.Left, windowLocation.Bottom - windowLocation.Top);
            using (var g = Graphics.FromImage(boardImage))
            {
                g.CopyFromScreen(new Point(windowLocation.Left, windowLocation.Top), Point.Empty, new Size(windowLocation.Right - windowLocation.Left, windowLocation.Bottom - windowLocation.Top));

            }
            boardImage.Save("ms.bmp");
            //Grab picture
            return boardImage;

        }

        private void UpdateBoardState(Bitmap image)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            using (var g2 = Graphics.FromImage(image))
            {
                for (var y = 50; y < 100; y++)
                {
                    for (var x = 0; x < imageWidth; x++)
                    {
                        if (Filters.SmileyFrame.IsMatch(image, imageWidth, imageHeight, x, y, null))
                        {
                            if (Filters.PlaySmiley.IsMatch(image, imageWidth, imageHeight, x, y, null))
                            {
                                State = BoardState.Playing;
                                return;
                            }
                            else if (Filters.WinSmiley.IsMatch(image, imageWidth, imageHeight, x, y, null))
                            {
                                State = BoardState.Won;
                                return;
                            }
                            else if (Filters.LostSmiley.IsMatch(image, imageWidth, imageHeight, x, y, null))
                            {
                                State = BoardState.Lost;
                                return;
                            }

                        }
                    }
                }
            }
        }

        private void UpdateBoard()
        {
            var boardImage = GetBoardImage(WindowLocation);
            UpdateBoard(boardImage);
            UpdateBoardState(boardImage);
        }

        private void UpdateBoard(Bitmap image)
        {
            Cells.Clear();
            Cell.ResetCounters();

            int matchCount = 0;
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            using (var g2 = Graphics.FromImage(image))
            {
                for (var y = 100; y < imageHeight; y++)
                {
                    for (var x = 0; x < imageWidth; x++)
                    {
                        if (
                            Filters.Border.IsMatch(image, imageWidth, imageHeight, x, y, null) ||
                            Filters.ClickedBorder.IsMatch(image, imageWidth, imageHeight, x, y, null) ||
                            Filters.HitBombCell.IsMatch(image, imageWidth, imageHeight, x, y, null)
                            )
                        {
                            matchCount++;
                            Log($"Found border at ({x}, {y})");
                            var foundCell = new Cell(x, y);

                            if (Filters.UnclickedCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Unclicked;
                            }
                            else if (Filters.EmptyCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Empty;
                                foundCell.Value = CellValue.None;
                            }
                            else if (Filters.ValueOneCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.One;
                            }
                            else if (Filters.ValueTwoCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Two;
                            }
                            else if (Filters.ValueThreeCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Three;
                            }
                            else if (Filters.ValueFourCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Four;
                            }
                            else if (Filters.ValueFiveCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Value;
                                foundCell.Value = CellValue.Five;
                            }
                            else if (Filters.FlaggedCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Flagged;
                                foundCell.Value = CellValue.None;
                            }
                            else if (Filters.UnknownCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Unknown;
                            }
                            else if (Filters.BombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.Bomb;
                                foundCell.Value = CellValue.None;
                                State = BoardState.Lost;
                            }
                            else if (Filters.HitBombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.HitBomb;
                                foundCell.Value = CellValue.None;
                                State = BoardState.Lost;
                            }
                            else if (Filters.WronglyFlaggedBombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                            {
                                foundCell.State = CellState.WronglyFlaggedBomb;
                                foundCell.Value = CellValue.None;
                                State = BoardState.Lost;
                            }



                            Cells.Add(foundCell);

                            x += Cell.CELL_SIZE;
                        }
                    }
                }
                image.Save($"ms_matches_m-{matchCount}.bmp");
            }

            Log($"Found {matchCount} border-pixels");
        }

        public Cell GetCellAt(int gridCol, int gridRow)
        {
            return Cells.SingleOrDefault(q => q.Col == gridCol && q.Row == gridRow);
        }

        public bool LeftClickCell(Cell cell, CellState? newState = null)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            var clickRes =
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_LEFTDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_LEFTUP);

            if (newState != null)
                cell.State = newState.Value;
            else
                UpdateBoard();

            return clickRes;
        }

        public bool RightClickCell(Cell cell)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            var clickRes =
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTUP);

            //UpdateBoard();
            cell.State = CellState.Flagged;
            cell.Value = CellValue.None;

            return clickRes;
        }

        private bool SendClick(int x, int y, uint button)
        {
            SetCursorPos(x, y);
            int lParam = (y << 16) + x;

            var inputMouse = new INPUT();
            inputMouse.Type = 0; /// input type mouse
            inputMouse.Data.Mouse.Flags = button; /// left button down            

            var inputs = new INPUT[] { inputMouse };
            var result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT))) == 0;

            return result;
        }

        public void PrintLayout()
        {
            Console.Clear();
            foreach (var cell in Cells)
            {
                Console.SetCursorPosition(cell.Col * 2, cell.Row + 1);
                switch (cell.State)
                {
                    case CellState.Unclicked:
                        Console.Write(".");
                        break;
                    case CellState.Empty:
                        Console.Write(" ");
                        break;
                    case CellState.Flagged:
                        Console.Write("F");
                        break;
                    case CellState.Unknown:
                        Console.Write("?");
                        break;
                    case CellState.Value:
                        Console.Write((int)cell.Value);
                        break;
                    case CellState.Bomb:
                        Console.Write("b");
                        break;
                    case CellState.HitBomb:
                        Console.Write("B");
                        break;
                    case CellState.WronglyFlaggedBomb:
                        Console.Write("f");
                        break;
                    default:
                        break;
                }

                Console.SetCursorPosition(0, GridHeight + 1);
                Console.WriteLine();
            }
        }

        private List<Cell> GetNeighborCells(Cell c)
        {
            var neighbors = new List<Cell>
            {
                GetCellAt(c.Col - 1, c.Row - 1),
                GetCellAt(c.Col - 0, c.Row - 1),
                GetCellAt(c.Col + 1, c.Row - 1),

                GetCellAt(c.Col - 1, c.Row - 0),
                GetCellAt(c.Col + 1, c.Row - 0),

                GetCellAt(c.Col - 1, c.Row + 1),
                GetCellAt(c.Col - 0, c.Row + 1),
                GetCellAt(c.Col + 1, c.Row + 1),
            };

            return neighbors.Where(f => f != null).ToList();
        }

        public string MakeNextMove()
        {
            if (WinmineIsProcessHandle() == IntPtr.Zero)
            {
                Console.WriteLine("Winmine isn't running. Exiting...");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }

            if (IsNewGame)
            {
                var cellColToClick = _rnd.Next(1, GridWidth + 1);
                var cellRowToClick = _rnd.Next(1, GridHeight + 1);
                var cellToClick = GetCellAt(cellColToClick, cellRowToClick);

                LeftClickCell(cellToClick);

                return $"New game. Clicked a random cell at ({cellColToClick}, {cellRowToClick})";
            }
            else if (!Cells.Any(q => q.State == CellState.Empty))
            {
                var cellColToClick = _rnd.Next(1, GridWidth + 1);
                var cellRowToClick = _rnd.Next(1, GridHeight + 1);
                var cellToClick = GetCellAt(cellColToClick, cellRowToClick);

                LeftClickCell(cellToClick);

                return $"No empty cells yet. Clicked a random cell at ({cellColToClick}, {cellRowToClick})";
            }

            var unknownCells = Cells.Where(q => q.State == CellState.Unknown).ToList();
            foreach (var cell in unknownCells)
            {
                RightClickCell(cell);
                UpdateBoard();
            }


            foreach (var cell in Cells.Where(q => q.State == CellState.Value))
            {
                var neighbours = GetNeighborCells(cell);

                var unclickedAndFlaggedNeighbours = neighbours.Where(n => n.State == CellState.Unclicked || n.State == CellState.Flagged);
                if (unclickedAndFlaggedNeighbours.Count() == (int)cell.Value && cell.Value > CellValue.None)
                {
                    var unclickedNeighbours = unclickedAndFlaggedNeighbours.Where(q => q.State == CellState.Unclicked).ToList();
                    if (unclickedNeighbours.Any())
                    {
                        foreach (var neighbour in unclickedNeighbours)
                        {
                            RightClickCell(neighbour);
                        }
                        return $"Cells must contain bombs. Flagged cells at [{string.Join(", ", unclickedNeighbours.Select(n => $"({n.Col}, {n.Row})"))}]";
                    }
                }

                var flaggedNeighbours = neighbours.Where(c => c.State == CellState.Flagged).ToList();
                if ((int)cell.Value == flaggedNeighbours.Count() && cell.Value > CellValue.None)
                {
                    var unclickedNeighbours = neighbours.Where(q => q.State == CellState.Unclicked).ToList();
                    if (unclickedNeighbours.Any())
                    {
                        foreach (var neighbour in unclickedNeighbours)
                        {
                            LeftClickCell(neighbour, CellState.Value);
                        }
                        UpdateBoard();
                        return $"Cells can't contain bombs. Clicked cells at [{string.Join(", ", unclickedNeighbours.Select(n => $"({n.Col}, {n.Row})"))}]";
                    }
                }
            }

            return "Took no action.";
        }
    }
}