using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleMoveChecker
{
    public class TilePlacement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Tile { get; set; }

        public TilePlacement(int x, int y, char tile)
        {
            (X, Y, Tile) = (x, y, tile);
        }
    }
}
