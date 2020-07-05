using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    interface ITilePosition
    {
        void RemoveTile();
        Task AddTile(char tile);
        char GetTile();
    }
}
