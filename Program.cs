﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static Minesweeper.Externals;

namespace Minesweeper
{
    public class Program
    {
        public static void Log(string msg)
        {
            //Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            var board = new Board();

            var movesMade = 0;
            var maxMoves = 10;// board.GridHeight * board.GridWidth; 
            while (board.State == BoardState.Playing && ++movesMade < maxMoves)
            {
                Console.WriteLine(board.MakeNextMove());
            }

            Console.WriteLine($"Game is over. You {(movesMade == maxMoves ? "ran out of moves" : board.State.ToString())}.");

            var timeTaken = DateTime.Now - startTime;
            Console.WriteLine($"Execution complete in {timeTaken.TotalMilliseconds}ms");
            //Thread.Sleep(5000);
            Console.ReadLine();
        }

    }
}
