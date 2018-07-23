using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public enum CellState
    {
        Unclicked,
        Empty,
        Flagged,
        Unknown,
        Value
    }

    public enum CellValue
    {
        Unknown,
        One,
        Two,
        Three,
        Four,
        Five,
        Six
    }

    public class Cell
    {
        public const int CELL_SIZE = 15;

        public int X { get; set; }
        public int Y { get; set; }
        public CellState State { get; set; } = CellState.Unclicked;
        public CellValue Value { get; set; } = CellValue.Unknown;
    }
}
