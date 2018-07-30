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
            //Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            Console.Write("How many bombs does your board have?: ");
            var bombs = int.Parse(Console.ReadLine());
            Console.WriteLine();
            var board = new Board(bombs);
            
            var movesMade = 0;
            var maxMoves = board.GridHeight * board.GridWidth;
            while (board.State == BoardState.Playing && ++movesMade < maxMoves)
            {
                var actionTaken = board.MakeNextMove();
                if (actionTaken != "Took no action.") movesMade = 0;

                Console.WriteLine(actionTaken);
            }

            //if(board.State == BoardState.Lost)
                //board.PrintLayout();

            Console.WriteLine($"Game is over. You {(movesMade == maxMoves ? "ran out of moves" : board.State.ToString())}.");

            var timeTaken = DateTime.Now - startTime;
            Console.WriteLine($"Execution complete in {timeTaken.TotalSeconds}s");
            //Thread.Sleep(5000);
            Console.Write("Retry? ");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.Y)
                Main(args);

        }

    }
}
