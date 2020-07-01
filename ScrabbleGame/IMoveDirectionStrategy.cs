using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleGame
{
    interface IMoveDirectionStrategy
    {
        bool TryAdjustMinMax(TilePlacement placement);
        HashSet<(int x, int y)> GetAllSpacesInMainWord();
        IEnumerable<char> WordTiles(Func<int, int, char> tileGetter);
        IEnumerable<char> BeforeWordTiles(Func<int, int, char> tileGetter);
        IEnumerable<char> AfterWordTiles(Func<int, int, char> tileGetter);
        IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement);
    }
}
