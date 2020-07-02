﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleGame
{
    public class Move
    {
        private readonly Game game;
        private readonly List<TilePlacement> placements = new List<TilePlacement>();

        // Set by SetDirectionStrategy(), which is called whenever the
        // list of placements changes, so should always be correct.
        // Set to null when the tiles do not form a straight line
        private IMoveDirectionStrategy directionStrategy;

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

        public int GetScore(out string error)
        {
            if (!IsValidMove(out error))
            {
                return 0;
            }

            var words = FindWords();
            return words.Sum(w => w.Score);
        }

        public void Play()
        {
            if (!IsValidMove(out string error)) throw new InvalidOperationException(error);

            foreach (var placement in placements)
            {
                game[placement.X, placement.Y] = placement.Tile;
            }
        }

        internal bool IsValidMove(out string error)
        {
            if (placements.Count == 0)
            {
                error = "No tile placements supplied";
                return false;
            }
            if (!InAValidLine() || (!TouchesExistingTile() && !game.IsFirstMove()))
            {
                error = "Invalid tile placement";
                return false;
            }
            if (game.IsFirstMove() && !IsUsingCentreSquare())
            {
                error = "Not on centre square";
                return false;
            }
            error = string.Empty;

            return true;
        }

        internal IEnumerable<string> InvalidWords()
        {
            var words = FindWords();
            foreach (var word in words)
            {
                var wordText = word.ToString();
                if (!game.CheckWord(wordText))
                {
                    yield return wordText;
                }
            }
        }

        private bool IsUsingCentreSquare() =>
            placements.Any(p => p.X == (Game.BOARD_WIDTH - 1) / 2 &&
                                p.Y == (Game.BOARD_HEIGHT - 1) / 2);

        private void SetDirectionStrategy()
        {
            // This method should be called when placements have changed
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

        internal List<PlayedWord> FindWords()
        {
            if (!IsValidMove(out string error)) throw new InvalidOperationException(error);

            List<PlayedWord> results = new List<PlayedWord>();

            // Check the main word that's been played
            PlayedWordBuilder mainWordBuilder = new PlayedWordBuilder();
            foreach (PlayedWordLetter tile in directionStrategy.WordTiles(
                (x,y) => placements.SingleOrDefault(p => p.X == x && p.Y == y)?.Tile ?? ' ',
                (x,y) => game[x,y]))
            {
                mainWordBuilder.AddBoardTileAtEnd(tile);
            }

            // Are the existing letters before/after it?
            foreach (PlayedWordLetter tile in directionStrategy.BeforeWordTiles((x, y) => game[x, y]))
            {
                mainWordBuilder.AddBoardTileAtStart(tile);
            }
            foreach (PlayedWordLetter tile in directionStrategy.AfterWordTiles((x, y) => game[x, y]))
            {
                mainWordBuilder.AddBoardTileAtEnd(tile);
            }

            var mainWord = mainWordBuilder.GetPlayedWord();
            if (mainWord.ToString().Length > 1)
            {
                results.Add(mainWord);
            }

            // Check for any intersecting words

            foreach (var placement in placements)
            {
                var oppositeStrategy = directionStrategy.GetOppositeStrategy(placement);
                var crossWordBuilder = new PlayedWordBuilder();
                crossWordBuilder.AddBoardTileAtStart(PlayedWordLetter.CreateWithBoardMultiplier(placement.Tile, placement.X, placement.Y));

                foreach (PlayedWordLetter tile in oppositeStrategy.BeforeWordTiles((x, y) => game[x, y]))
                {
                    crossWordBuilder.AddBoardTileAtStart(tile);
                }
                foreach (PlayedWordLetter tile in oppositeStrategy.AfterWordTiles((x, y) => game[x, y]))
                {
                    crossWordBuilder.AddBoardTileAtEnd(tile);
                }

                var crossWord = crossWordBuilder.GetPlayedWord();
                if (crossWord.ToString().Length > 1)
                {
                    results.Add(crossWord);
                }
            }

            return results;
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
