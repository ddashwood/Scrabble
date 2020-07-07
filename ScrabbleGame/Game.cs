using ScrabbleData;
using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("ScrabbleGameTests")]

namespace ScrabbleGame
{
    public class Game : GameBase
    {
        public int GameId { get; set; }
        public IWordChecker WordChecker { get; set; }
        public string RemainingTiles { get; private set; }
        public GamePlayer Player1 { get; set; } = new GamePlayer();
        public GamePlayer Player2 { get; set; } = new GamePlayer();
        public DateTime LastMove { get; set; }
        public Winner Winner { get; set; }
        public PlayerSelection NextPlayer { get; set; }

        public Game()
            : base(new string(' ', BOARD_WIDTH * BOARD_HEIGHT))
        { }

        internal bool CheckWord(string word)
        {
            return WordChecker.IsWord(word);
        }

        // We need to "hide" the indexer from the base class in order to
        // leave the getter unchanged (pass through to base), but add
        // a new setter where there wasn't one at all in the base class
        public new char this[int x, int y]
        {
            get
            {
                return base[x, y];
            }

            set
            {
                if (x < 0 || x >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                if (y < 0 || y >= BOARD_WIDTH)
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);

                char[] spaces = Board.ToCharArray();
                int index = y * BOARD_WIDTH + x;
                spaces[index] = value;
                Board = new string(spaces);
            }
        }

        public void SetupNewGame(string player1, string player2)
        {
            Player1.Id = player1;
            Player2.Id = player2;
            Board = new string(' ', BOARD_WIDTH * BOARD_HEIGHT);
            RemainingTiles = "ABC";
            Player1.Tiles = "BLA*E K";
            Player2.Tiles = "BLA*E K";
            Player1.Score = 0;
            Player2.Score = 0;
            Winner = Winner.NotFinished;
            NextPlayer = PlayerSelection.Player1;
            LastMove = DateTime.Now;
        }
    }
}
