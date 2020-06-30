using ScrabbleData;
using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class GameTests : TestBase
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
    }
}
