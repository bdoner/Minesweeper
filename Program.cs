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
    public class Program
    {
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;

            var board = new Board();
            //board.RightClickCell(board.GetCellAt(board.GridWidth / 2 + 1, board.GridHeight / 2 + 1));


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
