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

        public IEnumerable<char> WordTiles(Func<int, int, char> tileGetter)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return (tileGetter(x, y));
            }
        }

        public IEnumerable<char> BeforeWordTiles(Func<int, int, char> tileGetter)
        {
            int y = minY - 1;
            bool atEdge = y < 0;
            char nextChar;

            while (!atEdge && (nextChar = tileGetter(x, y)) != ' ')
            {
                yield return nextChar;
                y--;
                atEdge = y < 0;
            }
        }

        public IEnumerable<char> AfterWordTiles(Func<int, int, char> tileGetter)
        {
            int y = maxY + 1;
            bool atEdge = y >= Game.BOARD_HEIGHT;
            char nextChar;

            while (!atEdge && (nextChar = tileGetter(x, y)) != ' ')
            {
                yield return nextChar;
                y++;
                atEdge = y >= Game.BOARD_HEIGHT;
            }
        }

        public IMoveDirectionStrategy GetOppositeStrategy(TilePlacement placement)
        {
            return new MoveDirectionStrategyHorizontal(placement.X, placement.X, placement.Y);
        }
    }
}
