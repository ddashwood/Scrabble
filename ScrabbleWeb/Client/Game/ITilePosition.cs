using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    interface ITilePosition
    {
        void RemoveTile();
        void AddTile(char tile);
        char GetTile();
    }
}
