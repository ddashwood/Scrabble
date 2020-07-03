using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    public class DragSpaceOnBoard : IDragSpace
    {
        public int X { get; }
        public int Y { get; }

        public DragSpaceOnBoard(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
