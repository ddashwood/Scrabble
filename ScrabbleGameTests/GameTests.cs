using ScrabbleData;
using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class GameTests
    {
        [Fact]
        public void BoardGetTest()
        {
            string board = string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNO", 4))
                        + "ABC EFGHIJKLMNO" +
                        string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNO", 10));
            Assert.Equal(Game.BOARD_HEIGHT * Game.BOARD_WIDTH, board.Length);

            GameData data = new GameData { Board = board };
            Game game = new Game(data);

            Assert.Equal(' ', game[3, 4]);
        }

        [Fact]
        public void BoardSetTest()
        {
            Game game = GetGameWithEmptyBoard();

            game[3, 4] = 'X';
            Assert.Equal('X', game[3, 4]);
            Assert.Equal(' ', game[4, 3]);
        }

        [Fact]
        public void BoardGetOutOfRangeTest()
        {
            Game game = GetGameWithEmptyBoard();
            Assert.Throws<ArgumentOutOfRangeException>(() => game[0, Game.BOARD_HEIGHT]);
            Assert.Throws<ArgumentOutOfRangeException>(() => game[Game.BOARD_WIDTH, 0]);

            // Assert does not throw
            var _1 = game[0, Game.BOARD_HEIGHT - 1];
            var _2 = game[Game.BOARD_WIDTH - 1, 0];
        }

        [Fact]
        public void BoardSetOutOfRangeTest()
        {
            Game game = GetGameWithEmptyBoard();
            Assert.Throws<ArgumentOutOfRangeException>(() => game[0, Game.BOARD_HEIGHT] = ' ');
            Assert.Throws<ArgumentOutOfRangeException>(() => game[Game.BOARD_WIDTH, 0] = ' ');

            // Assert does not throw
            game[0, Game.BOARD_HEIGHT - 1] = ' ';
            game[Game.BOARD_WIDTH - 1, 0] = ' ';
        }

        [Fact]
        public void ValidMoveTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> badMove = new List<TilePlacement>
            {
                new TilePlacement(0, 8, 'P'),
                new TilePlacement(1, 8, 'A'),
                new TilePlacement(2, 8, 'S'),
                new TilePlacement(3, 8, 'T')
            };
            List<TilePlacement> goodMove = new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            };

            Assert.False(game.TryPlay(badMove, out string error));
            Assert.Equal("Invalid tile placement", error);
            Assert.True(game.TryPlay(goodMove, out _));
        }

        [Fact]
        public void NewTilesMustTouchOtherTilesTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> badMove = new List<TilePlacement>
            {
                new TilePlacement(0, 8, 'P'),
                new TilePlacement(1, 8, 'A'),
                new TilePlacement(2, 8, 'S'),
                new TilePlacement(3, 8, 'T')
            };
            List<TilePlacement> goodMove = new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            };

            Assert.False(game.TryPlay(badMove, out string error));
            Assert.Equal("Invalid tile placement", error);
            Assert.True(game.TryPlay(goodMove, out _));
        }

        [Fact]
        public void TilesMustBeInALineTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> move = new List<TilePlacement>
            {
                new TilePlacement(0, 8, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            };

            Assert.False(game.TryPlay(move, out string error));
            Assert.Equal("Invalid tile placement", error);
        }

        [Fact]
        public void NoGapsInNewTilesTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> move = new List<TilePlacement>
            {
                new TilePlacement(0, 5, 'P'),
                new TilePlacement(3, 5, 'T')
            };

            Assert.False(game.TryPlay(move, out string error));
            Assert.Equal("Invalid tile placement", error);

            move.Add(new TilePlacement(1, 5, 'A'));
            Assert.True(game.TryPlay(move, out _));
        }

        [Fact]
        public void CantOverwriteExistingTileTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> move = new List<TilePlacement>
            {
                new TilePlacement(0, 5, 'P'),
                new TilePlacement(1, 5, 'A'),
                new TilePlacement(2, 5, 'S'),
                new TilePlacement(3, 5, 'T')
            };

            Assert.False(game.TryPlay(move, out string error));
            Assert.Equal("Invalid tile placement", error);

        }

        [Fact]
        public void DuplicatesNotAllowedTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            List<TilePlacement> move = new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T'),
                new TilePlacement(2, 7, 'T')
            };

            Assert.False(game.TryPlay(move, out string error));
            Assert.Equal("Invalid tile placement", error);
        }



        private Game GetGameWithEmptyBoard()
        {
            string board = new string(' ', Game.BOARD_HEIGHT * Game.BOARD_WIDTH);
            GameData data = new GameData { Board = board };
            return new Game(data);
        }


        private Game GetGameWithSingleVerticalWord()
        {
            string board = string.Concat(Enumerable.Repeat("               ", 3)) + 
                           "  T            " + // TEST starts at position 2, 3
                           "  E            " +
                           "  S            " +
                           "  T            " + // and extends to position 2, 6
                           string.Concat(Enumerable.Repeat("               ", 8));
            GameData data = new GameData { Board = board };
            return new Game(data);
        }
    }
}
