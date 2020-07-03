using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleMoveChecker
{
    interface IMoveDirectionStrategy
    {
        bool TryAdjustMinMax(TilePlacement placement);
        HashSet<(int x, int y)> GetAllSpacesInMainWord();
        IEnumerable<PlayedWordLetter> WordTiles(Func<int, int, char> userTileGetter, Func<int, int, char> boardTileGetter);
        IEnumerable<PlayedWordLetter> BeforeWordTiles(Func<int, int, char> boardTileGetter);
        IEnumerable<PlayedWordLetter> AfterWordTiles(Func<int, int, char> boardTileGetter);
        IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement);
    }
}
