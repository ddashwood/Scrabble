using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    public class DragSpaceOffBoard : IDragSpace
    {
        public int Space { get; }

        public DragSpaceOffBoard(int space)
        {
            Space = space;
        }
    }
}
