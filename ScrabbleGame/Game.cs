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
        private readonly IWordChecker wordChecker;
        public int GameId { get; }
        public string Player1Id { get; private set; }
        public string Player2Id { get; private set; }
        public string RemainingTiles { get; private set; }
        public string Player1Tiles { get; private set; }
        public string Player2Tiles { get; private set; }

        public int Player1Score { get; private set; }
        public int Player2Score { get; private set; }

        public Game(IWordChecker wordChecker = null)
            : base(new string(' ', BOARD_WIDTH * BOARD_HEIGHT))
        {
            Player1Tiles = "BLE*A K";
            this.wordChecker = wordChecker;
        }

        public Game(GameData gameData, IWordChecker wordChecker = null)
            : base(gameData.Board)
        {
            if (gameData == null) throw new ArgumentNullException(nameof(gameData));
            this.wordChecker = wordChecker;

            GameId = gameData.GameId;
            Player1Id = gameData.Player1Id;
            Player2Id = gameData.Player2Id;
            RemainingTiles = gameData.RemainingTiles;
            Player1Tiles = gameData.Player1Tiles;
            Player2Tiles = gameData.Player2Tiles;

            Player1Score = gameData.Player1Score;
            Player2Score = gameData.Player2Score;
        }

        internal bool CheckWord(string word)
        {
            return wordChecker.IsWord(word);
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
            Player1Id = player1;
            Player2Id = player2;
            Board = new string(' ', BOARD_WIDTH * BOARD_HEIGHT);
            RemainingTiles = "ABC";
            Player1Tiles = "ABC";
            Player2Tiles = "ABC";
            Player1Score = 0;
            Player2Score = 0;
        }
    }
}
