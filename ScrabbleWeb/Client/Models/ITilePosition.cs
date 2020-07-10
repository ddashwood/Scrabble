using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Models
{
    public interface ITilePosition
    {
        void RemoveTile();
        Task AddTile(char tile);
        char GetTile();
    }
}
