using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Text;
using ScrabbleData;

namespace ScrabbleGame
{
    public class Move : MoveBase
    {
        // It's always safe to do this cast, because it's set by the
        // strongly typed constructor
        private Game Game => (Game)game;
        private GamePlayer thisPlayer;
        private bool thisPlayersMove;

        public Move(Game game, string playerId)
            : base(game)
        {
            if (playerId == game.Player1.Id)
            {
                thisPlayer = game.Player1;
                thisPlayersMove = game.NextPlayer == PlayerSelection.Player1;
            }
            else if (playerId == game.Player2.Id)
            {
                thisPlayer = game.Player2;
                thisPlayersMove = game.NextPlayer == PlayerSelection.Player2;
            }
            else
            {
                throw new InvalidOperationException("The Player Id supplied is not participating in this game");
            }
        }

        // Used for testing
        internal Move(Game game, List<TilePlacement> placements)
            : base(game, placements)
        { }

        public void Play()
        {
            if (!thisPlayersMove) throw new InvalidOperationException("Attempt to play when it's not your move");

            int score = GetScore(out string error);
            if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException(error);

            foreach (var placement in placements)
            {
                Game.ReplacePlayerTile(thisPlayer, placement.Tile);
                Game[placement.X, placement.Y] = placement.Tile;
            }

            thisPlayer.Score += score;
            Game.UpdateNextPlayerAndResult();
        }

        public IEnumerable<string> InvalidWords()
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
