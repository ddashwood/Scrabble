using Microsoft.AspNetCore.Mvc;
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

        // Set by SetDirectionStrategy(), which is called whenever the
        // list of placements changes, so should always be correct.
        // Set to null when the tiles do not form a straight line
        private IMoveDirectionStrategy directionStrategy;

        // Set by IsValidMove(), which is called explicitly when needed
        private bool? isValidMove = null;

        public Move(Game game)
        {
            this.game = game;
        }

        // Used for testing
        internal Move(Game game, List<TilePlacement> placements)
            :this(game)
        {
            this.placements = placements;
            SetDirectionStrategy();
        }

        public void AddPlacement(TilePlacement placement)
        {
            placements.Add(placement);
            SetDirectionStrategy();
        }
        public void RemovePlacementAtPosition(int x, int y)
        {
            placements.Remove(placements.Single(p => p.X == x && p.Y == y));
            SetDirectionStrategy();
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

            List<string> results = new List<string>();


            // Check the main word that's been played
            StringBuilder mainWord = new StringBuilder();
            foreach(char tile in directionStrategy.WordTiles(GetTileAt))
            {
                mainWord.Append(tile);
            }

            // Are the existing letters before/after it?
            foreach (char tile in directionStrategy.BeforeWordTiles((x, y) => game[x, y]))
            {
                mainWord.Insert(0, tile);
            }
            foreach (char tile in directionStrategy.AfterWordTiles((x, y) => game[x,y]))
            {
                mainWord.Append(tile);
            }

            if (mainWord.Length > 1)
            {
                results.Add(mainWord.ToString());
            }

            // Check for any intersecting words

            foreach (var placement in placements)
            {
                var oppositeStrategy = directionStrategy.GetOppositeStrategy(placement);
                var crossWord = new StringBuilder(placement.Tile.ToString());

                foreach (char tile in oppositeStrategy.BeforeWordTiles((x, y) => game[x,y]))
                {
                    crossWord.Insert(0, tile);
                }
                foreach (char tile in oppositeStrategy.AfterWordTiles((x, y) => game[x,y]))
                {
                    crossWord.Append(tile);
                }

                if(crossWord.Length > 1)
                {
                    results.Add(crossWord.ToString());
                }
            }

            return results;
        }

        private void SetDirectionStrategy()
        {
            // This method should only be called when placements have changed
            // Therefore, we can't know if it's valid until we explicitly check its validity
            isValidMove = null;
            directionStrategy = null;

            if (placements.Count == 1)
            {
                // Either horizontal or vertical would work here...
                directionStrategy = new MoveDirectionStrategyHorizontal(placements[0].X, placements[0].X, placements[0].Y);
                return;
            }

            var first = placements[0];
            var second = placements[1];


            if (first.X == second.X)
            {
                directionStrategy = new MoveDirectionStrategyVertical(first.X, Math.Min(first.Y, second.Y), Math.Max(first.Y, second.Y));
            }
            else if (first.Y == second.Y)
            {
                directionStrategy = new MoveDirectionStrategyHorizontal(Math.Min(first.X, second.X), Math.Max(first.X, second.X), first.Y);
            }
            else
            {
                // Can't be in a line if the first two items aren't in a line
                return;
            }

            // Check the rest of the tiles follow the same alignment as the first two
            for (int i = 2; i < placements.Count; i++)
            {
                if (!directionStrategy.TryAdjustMinMax(placements[i]))
                {
                    directionStrategy = null;
                    return;
                }
            }
        }

        private bool InAValidLine()
        {
            if (directionStrategy == null)
            {
                // This means the tiles are not in a straight line - no direction, hence no direction strategy
                return false;
            }

            // We've now established that the tiles are in a line
            // The next thing to check is whether there are any gaps in that line, or tiles
            // placed on top of other tiles

            HashSet<(int x, int y)> positions = directionStrategy.GetAllSpacesInMainWord();
            foreach (var placement in placements)
            {
                if (!positions.Contains((placement.X, placement.Y)))
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
                positions.Remove((placement.X, placement.Y));
            }
            // Anything left in positions now means the position is mid-word and has not been played in.
            // That's ok, so long as the board already has a tile in that position
            foreach (var position in positions)
            {
                if(game[position.x, position.y] == ' ')
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

        private char GetTileAt(int x, int y)
        {
            char result = game[x, y];
            if (result == ' ') // No tile on the board - see if there's a placement in this position
            {
                var placement = placements.SingleOrDefault(p => p.X == x && p.Y == y);
                if (placement != null)
                {
                    result = placement.Tile;
                }
            }

            return result;
        }
    }
}
