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

        private int _bombs;
        public int Bombs
        {
            get
            {
                if (_bombs == 0)
                {
                    _bombs = ReadIntFromMemory(new IntPtr(0x01005330));
                }
                return _bombs;
            }
        }

        private int _gridWidth;
        public int GridWidth
        {
            get
            {
                if (_gridWidth == 0)
                {
                    _gridWidth = ReadIntFromMemory(new IntPtr(0x01005334));
                }
                return _gridWidth;
            }
        }

        private int _gridHeight;
        public int GridHeight
        {
            get
            {
                if (_gridHeight == 0)
                {
                    _gridHeight = ReadIntFromMemory(new IntPtr(0x01005338));
                }
                return _gridHeight;
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

        private int ReadIntFromMemory(IntPtr addressOffset)
        {
            byte[] buffer = new byte[1];
            int bytesRead;

            var success = ReadBytesFromMemory(addressOffset, ref buffer, out bytesRead);

            return (int)buffer[0];
        }
        private bool ReadBytesFromMemory(IntPtr addressOffset, ref byte[] buffer, out int bytesRead)
        {
            var procReadHandle = OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, WinmineProcessId());

            var success = ReadProcessMemory(procReadHandle, addressOffset, buffer, buffer.Length, out bytesRead);
            CloseHandle(procReadHandle);

            return success;
        }

        public IntPtr WinmineProcessHandle()
        {
            var winmine = Process.GetProcessesByName("Winmine__XP");
            return winmine.Length == 1 ? winmine[0].MainWindowHandle : IntPtr.Zero;
        }

        public int WinmineProcessId()
        {
            var winmine = Process.GetProcessesByName("Winmine__XP");
            return winmine.Length == 1 ? winmine[0].Id : 0;
        }

        private RECT GetWindowLocation()
        {
            var gameWindow = WinmineProcessHandle();
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
            //boardImage.Save("ms.bmp");
            //Grab picture
            return boardImage;

        }

        private void UpdateBoardState(Bitmap image)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            //using (var g2 = Graphics.FromImage(image))
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
                            //else if (Filters.LostSmiley.IsMatch(image, imageWidth, imageHeight, x, y, null))
                            //{
                            //    State = BoardState.Lost;
                            //    return;
                            //}

                        }
                    }
                }
            }
        }

        private void UpdateBoardFromMemory()
        {
            // board can be no larger than 30*24
            var baseAddr = new IntPtr(0x01005360);
            var size = GridHeight * 0x20; //32 bytes per row. 
            //Read from 01005360+1

            int bytesRead;
            byte[] buffer = new byte[size];
            var success = ReadBytesFromMemory(baseAddr, ref buffer, out bytesRead);

            Cells.Clear();
            Cell.ResetCounters();

            var consoleColor = Console.ForegroundColor;
            for (var r = 0; r < GridHeight; r++)
            {
                var c = 0;
                var row = buffer.Skip((r * 0x20) + 1).Take(GridWidth);
                foreach (var f in row)
                {
                    var foundCell = new Cell((c * Cell.CELL_SIZE) + c + 14, (r * Cell.CELL_SIZE) + r + 102);
                    c++;
                    switch (f)
                    {
                        case 0x8F: //Bomb
                        case 0x0F: //Unclicked (without a bomb)
                            foundCell.State = CellState.Unclicked;
                            break;
                        case 0x40:
                            foundCell.State = CellState.Empty;
                            foundCell.Value = CellValue.None;
                            break;
                        case 0x41:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.One;
                            break;
                        case 0x42:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Two;
                            break;
                        case 0x43:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Three;
                            break;
                        case 0x44:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Four;
                            break;
                        case 0x45:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Five;
                            break;
                        case 0x46:
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Six;
                            break;
                        case 0x8E:
                            foundCell.State = CellState.Flagged;
                            foundCell.Value = CellValue.None;
                            break;
                        case 0x8A:
                            foundCell.State = CellState.Bomb;
                            foundCell.Value = CellValue.None;
                            State = BoardState.Lost;
                            break;
                        case 0xCC:
                            foundCell.State = CellState.HitBomb;
                            foundCell.Value = CellValue.None;
                            State = BoardState.Lost;
                            break;
                        default:
                            throw new NotImplementedException(f.ToString("X2"));
                    }



                    //else if (Filters.BombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                    //{
                    //    foundCell.State = CellState.Bomb;
                    //    foundCell.Value = CellValue.None;
                    //    State = BoardState.Lost;
                    //}
                    //else if (Filters.HitBombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                    //{
                    //    foundCell.State = CellState.HitBomb;
                    //    foundCell.Value = CellValue.None;
                    //    State = BoardState.Lost;
                    //}
                    //else if (Filters.WronglyFlaggedBombCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                    //{
                    //    foundCell.State = CellState.WronglyFlaggedBomb;
                    //    foundCell.Value = CellValue.None;
                    //    State = BoardState.Lost;
                    //}



                    Cells.Add(foundCell);
                }
            }

            Cells = Cells.Select(c => { c.Riskyness = c.State == CellState.Unclicked ? CalculateRiskyness(c) : 9F; return c; }).ToList();


        }

        private void UpdateBoard()
        {
            var boardImage = GetBoardImage(WindowLocation);
            //UpdateBoard(boardImage);
            UpdateBoardFromMemory();
            UpdateBoardState(boardImage);

            boardImage.Dispose();
        }

        private void UpdateBoard(Bitmap image)
        {
            throw new Exception("This method is old and shouldn't be used.");
            Cells.Clear();
            Cell.ResetCounters();

            int matchCount = 0;
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            //using (var g2 = Graphics.FromImage(image))
            //{
            Graphics g2 = null;
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
                        else if (Filters.ValueSixCell.IsMatch(image, imageWidth, imageHeight, x, y, g2))
                        {
                            foundCell.State = CellState.Value;
                            foundCell.Value = CellValue.Six;
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
            Cells = Cells.Select(c => { c.Riskyness = c.State == CellState.Unclicked ? CalculateRiskyness(c) : 9F; return c; }).ToList();
            //image.Save($"ms_matches_m-{matchCount}.bmp");
            //}

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

            return clickRes;
        }

        public bool RightClickCell(Cell cell)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            var clickRes =
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_RIGHTUP);

            cell.State = CellState.Flagged;
            cell.Value = CellValue.None;

            return clickRes;
        }

        public bool MiddleClickCell(Cell cell)
        {
            var cellPosX = WindowLocation.Left + cell.X + Cell.CELL_SIZE / 2;
            var cellPosY = WindowLocation.Top + cell.Y + Cell.CELL_SIZE / 2;

            var clickRes =
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_MIDDLEDOWN) &
                SendClick(cellPosX, cellPosY, MOUSEEVENTF_MIDDLEUP);

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
                Console.SetCursorPosition(cell.Col * 4, cell.Row);
                switch (cell.State)
                {
                    case CellState.Unclicked:
                        //Console.Write(".");
                        Console.Write(Math.Round(CalculateRiskyness(cell), 1));
                        break;
                    case CellState.Empty:
                        Console.Write(".");
                        break;
                    case CellState.Flagged:
                        Console.Write("F");
                        break;
                    case CellState.Unknown:
                        Console.Write("?");
                        break;
                    case CellState.Value:
                        Console.Write("v");
                        //Console.Write((int)cell.Value);
                        break;
                    case CellState.Bomb:
                        Console.Write("b");
                        break;
                    case CellState.HitBomb:
                        Console.Write(Math.Round(CalculateRiskyness(cell), 1));
                        //Console.Write("B");
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

        private List<Cell> GetNeighbourCells(Cell c)
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

        private float CalculateRiskyness(Cell c)
        {
            float maxRisk = 0.0F;
            var neighbours = GetNeighbourCells(c);
            foreach (var neighbour in neighbours)
            {
                var nns = GetNeighbourCells(neighbour);
                if (neighbour.State == CellState.Value && neighbour.Value > CellValue.None)
                {
                    float riskyNess = 0.0F;

                    var nnUnclicked = nns.Count(q => q.State == CellState.Unclicked);
                    var nnFlagged = nns.Count(q => q.State == CellState.Flagged);

                    if (nnUnclicked != 0)
                    {
                        riskyNess = ((float)(int)neighbour.Value - nnFlagged) / (float)nnUnclicked;
                        if (riskyNess > maxRisk)
                            maxRisk = riskyNess;
                    }

                }

            }

            if (maxRisk > 0)
                return maxRisk;
            else
                return ((float)(Bombs - Cells.Count(q => q.State == CellState.Flagged)) / (float)Cells.Count(q => q.State == CellState.Unclicked));
        }

        public string MakeNextMove()
        {
            if (WinmineProcessHandle() == IntPtr.Zero)
            {
                Console.WriteLine("Winmine isn't running. Exiting...");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }


            bool tookAction = false;
            //if (!Cells.Any(q => q.State == CellState.Empty))
            //{
            //    var unclicked = Cells.Where(q => q.State == CellState.Unclicked).ToList();
            //    var cellToClick = unclicked[_rnd.Next(0, unclicked.Count)];
            //    LeftClickCell(cellToClick);
            //    UpdateBoard();

            //    return $"No empty cells yet. Clicked a random cell at ({cellToClick.Col}, {cellToClick.Row})";
            //}

            var unknownCells = Cells.Where(q => q.State == CellState.Unknown).ToList();
            foreach (var cell in unknownCells)
            {
                RightClickCell(cell);
                UpdateBoard();
            }


            foreach (var cell in Cells.Where(q => q.State == CellState.Value))
            {
                var neighbours = GetNeighbourCells(cell);
                var flaggedNeighbours = neighbours.Where(c => c.State == CellState.Flagged).ToList();
                var unclickedNeighbours = neighbours.Where(c => c.State == CellState.Unclicked).ToList();

                // In a case where all the neighbours must be bombs
                if (unclickedNeighbours.Count + flaggedNeighbours.Count == (int)cell.Value && cell.Value > CellValue.None)
                {
                    if (unclickedNeighbours.Any())
                    {
                        foreach (var neighbour in unclickedNeighbours)
                        {
                            RightClickCell(neighbour);
                        }
                        Console.WriteLine($"Cells must contain bombs. Flagged cells at [{string.Join(", ", unclickedNeighbours.Select(n => $"({n.Col}, {n.Row})"))}]");
                        tookAction = true;
                    }
                }
            }

            foreach (var cell in Cells.Where(q => q.State == CellState.Value))
            {
                var neighbours = GetNeighbourCells(cell);
                var flaggedNeighbours = neighbours.Where(c => c.State == CellState.Flagged).ToList();
                var unclickedNeighbours = neighbours.Where(c => c.State == CellState.Unclicked).ToList();

                // In a case where all possible bombs are flagged and the rest of the sourrounding cells are free
                if ((int)cell.Value == flaggedNeighbours.Count() && cell.Value > CellValue.None && unclickedNeighbours.Count > 1)
                {
                    MiddleClickCell(cell);
                    //UpdateBoard();
                    Console.WriteLine($"Cell neighbours can't contain any more bombs. Clicking all around ({cell.Col}, {cell.Row})");
                    tookAction = true;
                }
                else if ((int)cell.Value == flaggedNeighbours.Count() && cell.Value > CellValue.None && unclickedNeighbours.Count > 0)
                {
                    var freeCell = unclickedNeighbours.First();
                    LeftClickCell(freeCell, CellState.Value);
                    Console.WriteLine($"Cell can't contain a bomb. Clicking ({freeCell.Col}, {freeCell.Row})");
                    tookAction = true;
                }
            }


            UpdateBoard();
            if (tookAction) return "Loop repeat";


            var leastRisk = Cells.Where(c => c.State == CellState.Unclicked).OrderBy(q => Guid.NewGuid()).OrderBy(q => q.Riskyness).First();
            Console.WriteLine($"Selecting the least risky cell with a risk of {leastRisk.Riskyness,2}. Clicking ({leastRisk.Col}, {leastRisk.Row})");

            LeftClickCell(leastRisk);

            UpdateBoard();
            return "Loop repeat";
        }
    }
}
