using ScrabbleData;
using ScrabbleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("ScrabbleGameTests")]

namespace ScrabbleGame
{
    public class Game : GameBase
    {
        public const int TILES_PER_PLAYER = 7;
        public int GameId { get; set; }
        public IWordChecker WordChecker { get; set; }
        public string RemainingTiles { get; private set; }
        public GamePlayer Player1 { get; set; } = new GamePlayer();
        public GamePlayer Player2 { get; set; } = new GamePlayer();
        public DateTime LastMove { get; set; }
        public Winner Winner { get; set; }
        public PlayerSelection NextPlayer { get; set; }
        public string LastMoveDescription { get; set; }
        public List<LastMoveTile> LastMoveTiles { get; internal set; }


        private static Random random = new Random();
        private static object randLock = new object();

        public Game()
            : base(new string(' ', BOARD_WIDTH * BOARD_HEIGHT))
        { }

        internal bool CheckWord(string word)
        {
            return WordChecker.IsWord(word);
        }

        internal void ReplacePlayerTile(GamePlayer player, char tile)
        {
            char realTile = (tile >= 'A' && tile <= 'Z') ? tile : '*'; // Is it a blank tile?
            int tilePosition = player.Tiles.IndexOf(realTile);

            if (tilePosition == -1)
            {
                throw new InvalidOperationException("Attempt to play a tile that is not on the player's rack");
            }

            var rack = player.Tiles.ToCharArray();
            rack[tilePosition] = GetCharFromRemainingTiles();
            player.Tiles = new string(rack);
        }

        internal void UpdateNextPlayerAndResult()
        {
            // TO DO - The game should also be over when there are no valid moves left

            GamePlayer playerWithNoTiles = null;
            GamePlayer playerWithRemainingTiles = null;

            if (string.IsNullOrWhiteSpace(Player1.Tiles))
            {
                (playerWithNoTiles, playerWithRemainingTiles) = (Player1, Player2);
            }
            else if (string.IsNullOrWhiteSpace(Player2.Tiles))
            {
                (playerWithNoTiles, playerWithRemainingTiles) = (Player2, Player1);
            }

            if (playerWithNoTiles == null)
            {
                // The game is not over
                NextPlayer = NextPlayer == PlayerSelection.Player1 ? PlayerSelection.Player2 : PlayerSelection.Player1;
                return;
            }

            // The game is over
            NextPlayer = PlayerSelection.N_A;
            var remainingScore = playerWithRemainingTiles.Tiles.Sum(c => LetterScore(c));
            playerWithRemainingTiles.Score -= remainingScore;
            playerWithNoTiles.Score += remainingScore; // Only if the player truly has no tiles - but at the moment that's the only way to win that's supported

            if (Player1.Score == Player2.Score)
            {
                Winner = Winner.Draw;
            }
            else if (Player1.Score > Player2.Score)
            {
                Winner = Winner.Player1;
            }
            else
            {
                Winner = Winner.Player2;
            }
        }

        public void Resign(string userId)
        {
            if (userId == Player1.Id && Winner == Winner.NotFinished)
            {
                Winner = Winner.Player2;
                LastMoveDescription = $"{Player1.Name} resigned";
            }
            else if (userId == Player2.Id && Winner == Winner.NotFinished)
            {
                Winner = Winner.Player1;
                LastMoveDescription = $"{Player2.Name} resigned";
            }
            else
            {
                throw new InvalidOperationException("User supplied is not participating in this game");
            }

            NextPlayer = PlayerSelection.N_A;
            LastMove = DateTime.Now;
        }

        public void Pass(string userId)
        {
            if (Player1.Id == userId && NextPlayer == PlayerSelection.Player1)
            {
                NextPlayer = PlayerSelection.Player2;
                LastMoveDescription = $"{Player1.Name} passed";
            }
            else if (Player2.Id == userId && NextPlayer == PlayerSelection.Player2)
            {
                NextPlayer = PlayerSelection.Player1;
                LastMoveDescription = $"{Player2.Name} passed";
            }
            else
            {
                throw new InvalidOperationException("Can't pass if it's not your move");
            }

            LastMove = DateTime.Now;
        }

        public void Swap(string userId, string tiles)
        {
            if (tiles.Length > RemainingTiles.Length)
            {
                throw new InvalidOperationException("Attempt to swap more tiles than there are in the tile bag");
            }

            GamePlayer player;
            if (Player1.Id == userId && NextPlayer == PlayerSelection.Player1)
            {
                NextPlayer = PlayerSelection.Player2;
                player = Player1;
            }
            else if (Player2.Id == userId && NextPlayer == PlayerSelection.Player2)
            {
                NextPlayer = PlayerSelection.Player1;
                player = Player2;
            }
            else
            {
                throw new InvalidOperationException("Can't swap if it's not your move");
            }

            var rack = player.Tiles.ToCharArray();
            foreach (char tile in tiles)
            {
                int index = Array.IndexOf(rack, tile);
                if (index == -1)
                {
                    throw new InvalidOperationException("Attempt to swap a tile you don't hold");
                }

                rack[index] = GetCharFromRemainingTiles();
            }

            player.Tiles = new string(rack);
            RemainingTiles += tiles;

            LastMoveDescription = $"{player.Name} swapped {tiles.Length} tiles";
            LastMove = DateTime.Now;
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
            RemainingTiles = string.Join("",
                    new string('A', 9),
                    new string('B', 2),
                    new string('C', 2),
                    new string('D', 4),
                    new string('E', 12),
                    new string('F', 2),
                    new string('G', 3),
                    new string('H', 2),
                    new string('I', 9),
                    new string('J', 1),
                    new string('K', 1),
                    new string('L', 4),
                    new string('M', 2),
                    new string('N', 6),
                    new string('O', 8),
                    new string('P', 2),
                    new string('Q', 1),
                    new string('R', 6),
                    new string('S', 4),
                    new string('T', 6),
                    new string('U', 4),
                    new string('V', 2),
                    new string('W', 2),
                    new string('X', 1),
                    new string('Y', 2),
                    new string('Z', 1),
                    new string('*', 2)
                );
            StringBuilder player1tiles = new StringBuilder();
            StringBuilder player2tiles = new StringBuilder();
            for (int i = 0; i < TILES_PER_PLAYER; i++)
            {
                player1tiles.Append(GetCharFromRemainingTiles());
                player2tiles.Append(GetCharFromRemainingTiles());
            }
            Player1.Tiles = player1tiles.ToString();
            Player2.Tiles = player2tiles.ToString();
            Player1.Score = 0;
            Player2.Score = 0;
            Winner = Winner.NotFinished;
            lock (randLock) // Must lock , because random iss static, and therefore shared between threads
            {
                NextPlayer = (PlayerSelection)random.Next(2);
            }
            LastMove = DateTime.Now;
        }

        private char GetCharFromRemainingTiles()
        {
            if (RemainingTiles == "")
            {
                // No tiles left
                return ' ';
            }

            int position;
            lock (randLock) // Must lock , because random iss static, and therefore shared between threads
            {
                position = random.Next(RemainingTiles.Length);
            }

            char result = RemainingTiles[position];
            RemainingTiles = RemainingTiles.Substring(0, position) + RemainingTiles.Substring(position + 1);
            return result;
        }
    }
}
