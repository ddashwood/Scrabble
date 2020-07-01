using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleGame
{
    class MoveDirectionStrategyVertical : IMoveDirectionStrategy
    {
        private int x;
        private int minY;
        private int maxY;

        public MoveDirectionStrategyVertical(int x, int minY, int maxY)
        {
            this.x = x;
            this.minY = minY;
            this.maxY = maxY;
        }

        public bool TryAdjustMinMax(TilePlacement placement)
        {
            if (placement.X != x)
            {
                return false;
            }

            if (placement.Y < minY) minY = placement.Y;
            if (placement.Y > maxY) maxY = placement.Y;
            return true;
        }

        public HashSet<(int x, int y)> GetAllSpacesInMainWord()
        {
            return Enumerable.Range(minY, maxY - minY + 1).Select(y => (x, y)).ToHashSet();
        }

        public IEnumerable<PlayedWordLetter> WordTiles(Func<int, int, char> userTileGetter, Func<int, int, char> boardTileGetter)
        {
            for (int y = minY; y <= maxY; y++)
            {
                char tile = boardTileGetter(x, y);
                if (tile == ' ')
                {
                    // The board has no tile here, so the user must be playing
                    // in this space. Include the position in order to find
                    // modifiers at that position
                    yield return new PlayedWordLetter(userTileGetter(x, y), x, y);
                }
                else
                {
                    // The board has a tile here already, so modifiers aren't
                    // used - therefore don't include the position when creating
                    // the played letter object
                    yield return new PlayedWordLetter(tile);
                }
            }
        }

        public IEnumerable<PlayedWordLetter> BeforeWordTiles(Func<int, int, char> boardTileGetter)
        {
            int y = minY - 1;
            bool atEdge = y < 0;
            char nextChar;

            while (!atEdge && (nextChar = boardTileGetter(x, y)) != ' ')
            {
                // Don't include modifiers on letters before/after what the user played
                yield return new PlayedWordLetter(nextChar);
                y--;
                atEdge = y < 0;
            }
        }

        public IEnumerable<PlayedWordLetter> AfterWordTiles(Func<int, int, char> boardTileGetter)
        {
            int y = maxY + 1;
            bool atEdge = y >= Game.BOARD_WIDTH;
            char nextChar;

            while (!atEdge && (nextChar = boardTileGetter(x, y)) != ' ')
            {
                // Don't include modifiers on letters before/after what the user played
                yield return new PlayedWordLetter(nextChar);
                y++;
                atEdge = y >= Game.BOARD_WIDTH;
            }
        }
        public IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement)
        {
            return new MoveDirectionStrategyHorizontal(placement.X, placement.X, placement.Y);
        }
    }
}
