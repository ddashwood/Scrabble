using ScrabbleGame;
using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class FirstMoveTests : TestBase
    {
        [Fact]
        public void FirstMoveTest()
        {
            var newGame = GetGameWithEmptyBoard();
            var existingGame = GetGameWithSingleVerticalWord();

            Assert.True(newGame.IsFirstMove());
            Assert.False(existingGame.IsFirstMove());
        }

        [Fact]
        public void FirstMoveOkTest()
        {
            Game game = GetGameWithEmptyBoard();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(7, 6, 'T'),
                new TilePlacement(7, 7, 'E'),
                new TilePlacement(7, 8, 'S'),
                new TilePlacement(7, 9, 'T'),
            });

            Assert.True(move.IsValidMove(out string _));
        }

        [Fact]
        public void FirstMoveNotOnCentreTest()
        {
            Game game = GetGameWithEmptyBoard();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(8, 6, 'T'),
                new TilePlacement(8, 7, 'E'),
                new TilePlacement(8, 8, 'S'),
                new TilePlacement(8, 9, 'T'),
            });

            Assert.False(move.IsValidMove(out string error));
            Assert.Equal("Not on centre square", error);
        }
    }
}