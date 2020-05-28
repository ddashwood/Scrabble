using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ScrabbleData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:InternalsVisibleTo("ScrabbleGameTests")]

namespace ScrabbleGame
{
    /// <summary>
    /// Represents a Scrabble game. When referring to a square on the board,
    /// ' ' means the square is empty, lower case means a blank tile has been
    /// used, upper case means a regular tile has been used. X/Y coordinates are
    /// zero-based, with (0,0) being to the top-left corner of the board.
    /// </summary>
    public class Game
    {
        public const int BOARD_WIDTH = 15;
        public const int BOARD_HEIGHT = 15;

        private readonly GameData gameData;
        public Game(GameData gameData)
        {
            this.gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));
        }

        public char this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                if (y < 0 || y >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);

                int index = y * BOARD_WIDTH + x;
                return gameData.Board[index];
            }

            internal set
            {
                if (x < 0 || x >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                if (y < 0 || y >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);

                char[] spaces = gameData.Board.ToCharArray();
                int index = y * BOARD_WIDTH + x;
                spaces[index] = value;
                gameData.Board = new string(spaces);
            }
        }

        public bool TryPlay(List<TilePlacement> tilePlacements, out string error)
        {
            if (tilePlacements == null || tilePlacements.Count == 0)
            {
                error = "No tile placements supplied";
                return false;
            }
            if (!TouchesExistingTile(tilePlacements) || !InALine(tilePlacements))
            {
                error = "Invalid tile placement";
                return false;
            }
            error = string.Empty;

            return true;
        }

        private bool InALine(List<TilePlacement> tilePlacements)
        {
            if (tilePlacements.Count == 1) return true;

            var first = tilePlacements[0];
            var second = tilePlacements[1];

            bool horizontal;
            int lineNumber;
            int min;
            int max;
            if (first.X == second.X)
            {
                horizontal = false;
                lineNumber = first.X;
                min = Math.Min(first.Y, second.Y);
                max = Math.Max(first.Y, second.Y);
            }
            else if (first.Y == second.Y)
            {
                horizontal = true;
                lineNumber = first.Y;
                min = Math.Min(first.X, second.X);
                max = Math.Max(first.X, second.X);
            }
            else
            {
                return false; // Can't be in a line if the first two items aren't in a line
            }

            for (int i = 2; i < tilePlacements.Count; i++)
            {
                if (lineNumber != (horizontal ? tilePlacements[i].Y : tilePlacements[i].X))
                {
                    return false;
                }

                int newVal = (horizontal ? tilePlacements[i].X : tilePlacements[i].Y);
                if (newVal < min) min = newVal;
                if (newVal > max) max = newVal;
            }

            // We've now established that the tiles are in a line
            // The next thing to check is whether there are any gaps in that line, or tiles
            // placed on top of other tiles

            HashSet<int> positions = new HashSet<int>(Enumerable.Range(min, max - min + 1));
            foreach (var placement in tilePlacements)
            {
                int val = (horizontal ? placement.X : placement.Y);
                if (!positions.Contains(val))
                {
                    // positions was initialised with a range going from the minimum to the
                    // maximum. So if it doesn't contain the current value, the reason would
                    // be because that value has already been removed, i.e. there are duplicates
                    return false;
                }
                if (this[placement.X, placement.Y] != ' ')
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
                char currentTile = horizontal ? this[val, first.Y] : this[first.X, val];
                if (currentTile == ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private bool TouchesExistingTile(List<TilePlacement> tilePlacements)
        {
            foreach (var placement in tilePlacements)
            {
                // Check left
                if (placement.X > 0 && this[placement.X - 1, placement.Y] != ' ')
                    return true;

                // Check right
                if (placement.X < BOARD_WIDTH - 1 && this[placement.X + 1, placement.Y] != ' ')
                    return true;

                // Check up
                if (placement.Y > 0 && this[placement.X, placement.Y - 1] != ' ')
                    return true;

                // Check down
                if (placement.Y < BOARD_HEIGHT - 1 && this[placement.X, placement.Y + 1] != ' ')
                    return true;
            }

            return false;
        }
    }
}
