using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleGame
{
    public class Move : MoveBase
    {
        // It's always safe to do this case, because it's set by the
        // strongly typed constructor
        private Game Game => (Game)game;

        public Move(Game game)
            :base(game)
        { }

        // Used for testing
        internal Move(Game game, List<TilePlacement> placements)
            :base(game, placements)
        { }

        public void Play()
        {
            if (!IsValidMove(out string error)) throw new InvalidOperationException(error);

            foreach (var placement in placements)
            {
                Game[placement.X, placement.Y] = placement.Tile;
            }
        }

        internal IEnumerable<string> InvalidWords()
        {
            var words = FindWords();
            foreach (var word in words)
            {
                var wordText = word.ToString();
                if (!Game.CheckWord(wordText))
                {
                    yield return wordText;
                }
            }
        }
    }
}
