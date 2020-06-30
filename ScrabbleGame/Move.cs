using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleGame
{
    public class Move
    {
        private enum Direction
        {
            Horizontal,
            Vertical,
            SingleTile,
            NotALine = 99
        }

        private readonly Game game;
        private readonly List<TilePlacement> placements = new List<TilePlacement>();

        private bool? isValidMove = null;

        // This group of 4 variables is set by the SetMoveData() method every time the
        // list of tile placements changes
        private Direction moveDirection = Direction.NotALine;
        private int rowOrColNumber; // Row if horizontal, Col if vertical, otherwise invalid
        private int minColOrRow;  // First column if horizontal, first row if vertical, otherwise invalid
        private int maxColOrRow;  // Last column if horizontal, last row if vertical, otherwise invalid

        public Move(Game game)
        {
            this.game = game;
        }

        // Used for testing
        internal Move(Game game, List<TilePlacement> placements)
            :this(game)
        {
            this.placements = placements;
            SetMoveData();
        }

        public void AddPlacement(TilePlacement placement)
        {
            placements.Add(placement);
            SetMoveData();
        }
        public void RemovePlacementAtPosition(int x, int y)
        {
            placements.Remove(placements.Single(p => p.X == x && p.Y == y));
            SetMoveData();
        }

        internal bool IsValidMove(out string error)
        {
            isValidMove = false;
            if (placements.Count == 0)
            {
                error = "No tile placements supplied";
                return false;
            }
            if (!TouchesExistingTile() || !InAValidLine())
            {
                error = "Invalid tile placement";
                return false;
            }
            error = string.Empty;

            isValidMove = true;
            return true;
        }

        internal List<string> FindWords()
        {
            if (isValidMove == null) throw new InvalidOperationException("Move validity has not been checked");
            if (isValidMove == false) throw new InvalidOperationException("Not a valid move");

            throw new NotImplementedException();
        }

        private void SetMoveData()
        {
            if (placements.Count == 1)
            {
                moveDirection = Direction.SingleTile;
                return;
            }

            var first = placements[0];
            var second = placements[1];


            if (first.X == second.X)
            {
                moveDirection = Direction.Vertical;
                rowOrColNumber = first.X;
                minColOrRow = Math.Min(first.Y, second.Y);
                maxColOrRow = Math.Max(first.Y, second.Y);
            }
            else if (first.Y == second.Y)
            {
                moveDirection = Direction.Horizontal;
                rowOrColNumber = first.Y;
                minColOrRow = Math.Min(first.X, second.X);
                maxColOrRow = Math.Max(first.X, second.X);
            }
            else
            {
                moveDirection = Direction.NotALine;  // Can't be in a line if the first two items aren't in a line
                return;
            }


            for (int i = 2; i < placements.Count; i++)
            {
                if (rowOrColNumber != (moveDirection == Direction.Horizontal ? placements[i].Y : placements[i].X))
                {
                    moveDirection = Direction.NotALine;
                    return;
                }

                int newVal = (moveDirection == Direction.Horizontal ? placements[i].X : placements[i].Y);
                if (newVal < minColOrRow) minColOrRow = newVal;
                if (newVal > maxColOrRow) maxColOrRow = newVal;
            }
        }

        private bool InAValidLine()
        {
            if (moveDirection == Direction.NotALine)
            {
                return false;
            }

            // We've now established that the tiles are in a line
            // The next thing to check is whether there are any gaps in that line, or tiles
            // placed on top of other tiles

            HashSet<int> positions = new HashSet<int>(Enumerable.Range(minColOrRow, maxColOrRow - minColOrRow + 1));
            foreach (var placement in placements)
            {
                int val = (moveDirection == Direction.Horizontal ? placement.X : placement.Y);
                if (!positions.Contains(val))
                {
                    // positions was initialised with a range going from the minimum to the
                    // maximum. So if it doesn't contain the current value, the reason would
                    // be because that value has already been removed, i.e. there are duplicates
                    return false;
                }
                if (game[placement.X, placement.Y] != ' ')
                {
                    // Trying to play where there's already a tile
                    return false;
                }
                positions.Remove(val);
            }

            // Anything left in positions now means the position is not specified. That's ok,
            // so long as the board already has a tile in that position
            foreach (int val in positions)
            {
                char currentTile = moveDirection == Direction.Horizontal ? game[val, rowOrColNumber] : game[rowOrColNumber, val];
                if (currentTile == ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private bool TouchesExistingTile()
        {
            foreach (var placement in placements)
            {
                // Check left
                if (placement.X > 0 && game[placement.X - 1, placement.Y] != ' ')
                    return true;

                // Check right
                if (placement.X < Game.BOARD_WIDTH - 1 && game[placement.X + 1, placement.Y] != ' ')
                    return true;

                // Check up
                if (placement.Y > 0 && game[placement.X, placement.Y - 1] != ' ')
                    return true;

                // Check down
                if (placement.Y < Game.BOARD_HEIGHT - 1 && game[placement.X, placement.Y + 1] != ' ')
                    return true;
            }

            return false;
        }
    }
}
