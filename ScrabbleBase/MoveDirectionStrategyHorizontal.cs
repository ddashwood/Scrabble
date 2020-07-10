using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleBase
{
    class MoveDirectionStrategyHorizontal : IMoveDirectionStrategy
    {
        private int minX;
        private int maxX;
        private int y;

        public MoveDirectionStrategyHorizontal(int minX, int maxX, int y)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.y = y;
        }

        public bool TryAdjustMinMax(TilePlacement placement)
        {
            if (placement.Y != y)
            {
                return false;
            }

            if (placement.X < minX) minX = placement.X;
            if (placement.X > maxX) maxX = placement.X;
            return true;
        }

        public HashSet<(int x, int y)> GetAllSpacesInMainWord()
        {
            var spaces = Enumerable.Range(minX, maxX - minX + 1).Select(x => (x, y));
            return new HashSet<(int x, int y)>(spaces);
        }

        public IEnumerable<PlayedWordLetter> WordTiles(Func<int, int, char> userTileGetter, Func<int, int, char> boardTileGetter)
        {
            for (int x = minX; x <= maxX; x++)
            {
                char tile = boardTileGetter(x, y);
                if (tile == ' ')
                {
                    // The board has no tile here, so the user must be playing
                    // in this space. Include the multiplier from the board
                    yield return PlayedWordLetter.CreateWithBoardMultiplier(userTileGetter(x, y), x, y);
                }
                else
                {
                    // The board has a tile here already, so multipliers aren't used
                    yield return PlayedWordLetter.Create(tile);
                }
            }
        }

        public IEnumerable<PlayedWordLetter> BeforeWordTiles(Func<int, int, char> boardTileGetter)
        {
            int x = minX - 1;
            bool atEdge = x < 0;
            char nextChar;

            while (!atEdge && (nextChar = boardTileGetter(x, y)) != ' ')
            {
                // Don't include multipliers on letters before/after what the user played
                yield return PlayedWordLetter.Create(nextChar);
                x--;
                atEdge = x < 0;
            }
        }

        public IEnumerable<PlayedWordLetter> AfterWordTiles(Func<int, int, char> boardTileGetter)
        {
            int x = maxX + 1;
            bool atEdge = x >= GameBase.BOARD_WIDTH;
            char nextChar;

            while (!atEdge && (nextChar = boardTileGetter(x, y)) != ' ')
            {
                // Don't include multipliers on letters before/after what the user played
                yield return PlayedWordLetter.Create(nextChar);
                x++;
                atEdge = x >= GameBase.BOARD_WIDTH;
            }
        }

        public IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement)
        {
            return new MoveDirectionStrategyVertical(placement.X, placement.Y, placement.Y);
        }
    }
}
