using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace ScrabbleGame
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
            return Enumerable.Range(minX, maxX - minX + 1).Select(x => (x, y)).ToHashSet();
        }

        public IEnumerable<char> WordTiles(Func<int, int, char> tileGetter)
        {
            for (int x = minX; x <= maxX; x++)
            {
                yield return (tileGetter(x, y));
            }
        }

        public IEnumerable<char> BeforeWordTiles(Func<int, int, char> tileGetter)
        {
            int x = minX - 1;
            bool atEdge = x < 0;
            char nextChar;

            while (!atEdge && (nextChar = tileGetter(x, y)) != ' ')
            {
                yield return nextChar;
                x--;
                atEdge = x < 0;
            }
        }

        public IEnumerable<char> AfterWordTiles(Func<int, int, char> tileGetter)
        {
            int x = maxX + 1;
            bool atEdge = x >= Game.BOARD_WIDTH;
            char nextChar;

            while (!atEdge && (nextChar = tileGetter(x, y)) != ' ')
            {
                yield return nextChar;
                x++;
                atEdge = x >= Game.BOARD_WIDTH;
            }
        }

        public IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement)
        {
            return new MoveDirectionStrategyVertical(placement.X, placement.Y, placement.Y);
        }
    }
}
