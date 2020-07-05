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

        public override bool Equals(object obj)
        {
            return obj is RackPosition other && other.Space == Space;
        }

        public override int GetHashCode()
        {
            return Space.GetHashCode();
        }
    }
}
