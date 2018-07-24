using System;
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
            UpdateBoard();
            _rnd = new Random();
            if (Cells.Count != GridWidth * GridHeight)
            {
                throw new Exception($"Parsed {Cells.Count} cells but expected {GridWidth * GridHeight} cells.");
            }
        }

        private RECT GetWindowLocation()
        {
            var winmine = Process.GetProcessesByName("Winmine__XP");
            if (winmine.Length == 0)
            {
                Console.WriteLine("\"Winmine__XP.exe\" not found running. Please run a game of Minesweeper and try again. Press any key to exit.");
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
            WindowLocation = GetWindowLocation();
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

        public bool LeftClickCell(Cell cell)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            return
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_LEFTDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_LEFTUP);
        }

        public bool RightClickCell(Cell cell)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            return
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTUP);
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
            Thread.Sleep(100);
            return result;
        }

        public string MakeNextMove()
        {
            if (IsNewGame)
            {
                var cellColToClick = _rnd.Next(1, GridWidth + 1);
                var cellRowToClick = _rnd.Next(1, GridHeight + 1);
                var cellToClick = GetCellAt(cellColToClick, cellRowToClick);

                LeftClickCell(cellToClick);
                UpdateBoard();

                return $"New game. Clicked a random cell at ({cellColToClick}, {cellRowToClick})";
            }
            else if(!Cells.Any(q => q.State == CellState.Empty))
            {
                var cellColToClick = _rnd.Next(1, GridWidth + 1);
                var cellRowToClick = _rnd.Next(1, GridHeight + 1);
                var cellToClick = GetCellAt(cellColToClick, cellRowToClick);

                LeftClickCell(cellToClick);
                UpdateBoard();

                return $"No empty cells yet. Clicked a random cell at ({cellColToClick}, {cellRowToClick})";
            }
            else
            {

            }
            // find mine cells
            // mark mines cells
            // find safe cells
            // click safe cells

            return "Took no action.";
        }
    }
}
