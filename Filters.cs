using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Filters
    {
        public static PixelFilter Border = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xffffff),

                    //X top left to top right
                    new Pixel( 1, 0, 0xffffff),
                    new Pixel( 2, 0, 0xffffff),
                    new Pixel( 3, 0, 0xffffff),
                    new Pixel( 4, 0, 0xffffff),
                    new Pixel( 5, 0, 0xffffff),
                    new Pixel( 6, 0, 0xffffff),
                    new Pixel( 7, 0, 0xffffff),
                    new Pixel( 8, 0, 0xffffff),
                    new Pixel( 9, 0, 0xffffff),
                    new Pixel(10, 0, 0xffffff),
                    new Pixel(11, 0, 0xffffff),
                    new Pixel(12, 0, 0xffffff),

                    new Pixel(13, 0, 0xc0c0c0),
                    new Pixel(14, 0, 0x808080),

                    //Y top right to bottom right
                    new Pixel(14, 1, 0x808080),
                    new Pixel(14, 2, 0x808080),
                    new Pixel(14, 3, 0x808080),
                    new Pixel(14, 4, 0x808080),
                    new Pixel(14, 5, 0x808080),
                    new Pixel(14, 6, 0x808080),
                    new Pixel(14, 7, 0x808080),
                    new Pixel(14, 8, 0x808080),



                    //Y top left to bottom left
                    new Pixel(0,  1, 0xffffff),
                    new Pixel(0,  2, 0xffffff),
                    new Pixel(0,  3, 0xffffff),
                    new Pixel(0,  4, 0xffffff),
                    new Pixel(0,  5, 0xffffff),
                    new Pixel(0,  6, 0xffffff),
                    new Pixel(0,  7, 0xffffff),
                    new Pixel(0,  8, 0xffffff),
                    new Pixel(0,  9, 0xffffff),
                    new Pixel(0, 10, 0xffffff),
                    new Pixel(0, 11, 0xffffff),
                    new Pixel(0, 12, 0xffffff),

                    new Pixel(0, 13, 0xc0c0c0),
                    new Pixel(0, 14, 0x808080),


                    //X bottom left to bottom right
                    new Pixel(1, 14, 0x808080),
                    new Pixel(2, 14, 0x808080),
                    new Pixel(3, 14, 0x808080),
                    new Pixel(4, 14, 0x808080),
                    new Pixel(5, 14, 0x808080),
                    new Pixel(6, 14, 0x808080),
                    new Pixel(7, 14, 0x808080),
                    new Pixel(8, 14, 0x808080),


                }
        };


        public static PixelFilter ClickedBorder = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),

                    //X top left to top right
                    new Pixel( 1, 0, 0xc0c0c0),
                    new Pixel( 2, 0, 0xc0c0c0),
                    new Pixel( 3, 0, 0xc0c0c0),
                    new Pixel( 4, 0, 0xc0c0c0),
                    new Pixel( 5, 0, 0xc0c0c0),
                    new Pixel( 6, 0, 0xc0c0c0),
                    new Pixel( 7, 0, 0xc0c0c0),
                    new Pixel( 8, 0, 0xc0c0c0),
                    new Pixel( 9, 0, 0xc0c0c0),
                    new Pixel(10, 0, 0xc0c0c0),
                    new Pixel(11, 0, 0xc0c0c0),
                    new Pixel(12, 0, 0xc0c0c0),

                    new Pixel(13, 0, 0xc0c0c0),
                    new Pixel(14, 0, 0xc0c0c0),

                    //Y top right to bottom right
                    new Pixel(14, 1, 0xc0c0c0),
                    new Pixel(14, 2, 0xc0c0c0),
                    new Pixel(14, 3, 0xc0c0c0),
                    new Pixel(14, 4, 0xc0c0c0),
                    new Pixel(14, 5, 0xc0c0c0),
                    new Pixel(14, 6, 0xc0c0c0),
                    new Pixel(14, 7, 0xc0c0c0),
                    new Pixel(14, 8, 0xc0c0c0),



                    //Y top left to bottom left
                    new Pixel(0,  1, 0xc0c0c0),
                    new Pixel(0,  2, 0xc0c0c0),
                    new Pixel(0,  3, 0xc0c0c0),
                    new Pixel(0,  4, 0xc0c0c0),
                    new Pixel(0,  5, 0xc0c0c0),
                    new Pixel(0,  6, 0xc0c0c0),
                    new Pixel(0,  7, 0xc0c0c0),
                    new Pixel(0,  8, 0xc0c0c0),
                    new Pixel(0,  9, 0xc0c0c0),
                    new Pixel(0, 10, 0xc0c0c0),
                    new Pixel(0, 11, 0xc0c0c0),
                    new Pixel(0, 12, 0xc0c0c0),

                    new Pixel(0, 13, 0xc0c0c0),
                    new Pixel(0, 14, 0xc0c0c0),

                    //X bottom left to bottom right
                    new Pixel(1, 14, 0xc0c0c0),
                    new Pixel(2, 14, 0xc0c0c0),
                    new Pixel(3, 14, 0xc0c0c0),
                    new Pixel(4, 14, 0xc0c0c0),
                    new Pixel(5, 14, 0xc0c0c0),
                    new Pixel(6, 14, 0xc0c0c0),
                    new Pixel(7, 14, 0xc0c0c0),
                    new Pixel(8, 14, 0xc0c0c0),


                }
        };

        public static PixelFilter UnclickedCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xffffff),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0x808080),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0x808080),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0x808080)
                }
        };

        public static PixelFilter EmptyCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0xc0c0c0),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0xc0c0c0)
                }
        };

        public static PixelFilter ValueOneCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0x0000ff),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0xc0c0c0),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0xc0c0c0)
                }
        };

        public static PixelFilter ValueTwoCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0x008000),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0xc0c0c0),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0xc0c0c0)
                }
        };

        public static PixelFilter ValueThreeCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0xff0000),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0xc0c0c0),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0xc0c0c0)
                }
        };

        public static PixelFilter ValueFourCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE / 2, Cell.CELL_SIZE / 2, 0x000080),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0xc0c0c0),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0xc0c0c0),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0xc0c0c0)
                }
        };

        public static PixelFilter FlaggedCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xffffff),
                    new Pixel( 6, 4, 0xff0000),
                    new Pixel( 7, 9, 0x000000),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0x808080),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0x808080),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0x808080)
                }
        };

        public static PixelFilter UnknownCell = new PixelFilter
        {
            Threshold = 0,
            Filter = new List<Pixel>
                {
                    new Pixel( 0, 0, 0xffffff),
                    new Pixel( 4, 3, 0x000000),
                    new Pixel( 9, 4, 0x000000),
                    new Pixel( Cell.CELL_SIZE-1, 0, 0x808080),
                    new Pixel( 0, Cell.CELL_SIZE-1, 0x808080),
                    new Pixel( Cell.CELL_SIZE-1, Cell.CELL_SIZE-1, 0x808080)
                }
        };





    }
}
