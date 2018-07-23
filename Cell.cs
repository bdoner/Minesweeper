﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six
    }

    [DebuggerDisplay("Point=({X}, {Y}), Grid=({Col}, {Row}), State={State}, Value={Value}")]
    public class Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;

            if (Y != _lastY)
            {
                Row = _lastRow + 1;
                _lastRow = Row;
                _lastY = Y;
                _lastCol = 0;
            }
            Col = _lastCol + 1;
            Row = _lastRow;
            _lastCol = Col;
        }

        private static int _lastY = 0;
        private static int _lastRow = 0;
        private static int _lastCol = 0;
        public const int CELL_SIZE = 15;

        public int Row { get; internal set; }
        public int Col { get; internal set; }
        public int X { get; set; }
        public int Y { get; set; }
        public CellState State { get; set; } = CellState.Unclicked;
        public CellValue Value { get; set; } = CellValue.Unknown;
    }
}
