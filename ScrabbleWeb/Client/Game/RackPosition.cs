using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    public class RackPosition : ITilePosition
    {
        public int Space { get; }

        public RackPosition(int space)
        {
            Space = space;
        }
    }
}
