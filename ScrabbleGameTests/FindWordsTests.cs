using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class FindWordsTests : TestBase
    {
        [Fact]
        public void CantFindWordsWithInvalidMoveTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 2, 'B')
            });

            var e = Assert.Throws<InvalidOperationException>(() => move.FindWords());
            Assert.Equal("Move validity has not been checked", e.Message);
            Assert.False(move.IsValidMove(out string _));
            e = Assert.Throws<InvalidOperationException>(() => move.FindWords());
            Assert.Equal("Not a valid move", e.Message);
        }

        [Fact]
        public void GetNewWordsFindSimpleWordTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 2, 'B'),
                new TilePlacement(1, 3, 'L'),
                new TilePlacement(1, 4, 'O'),
                new TilePlacement(1, 5, 'B')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords();

            Assert.Contains("BLOB", actual);
        }

        [Fact]
        public void GetNewWordsBeforeExistingTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(2, 1, 'R'),
                new TilePlacement(2, 2, 'E')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords();

            Assert.Contains("RETEST", actual);
        }

        [Fact]
        public void GetNewWordsBeforeExistingOnEdgeTest()
        {
            Game game = GetGameWithSingleVerticalWordAtBottom();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(2, 9, 'R'),
                new TilePlacement(2, 10, 'E')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords();

            Assert.Contains("RETEST", actual);
        }

        [Fact]
        public void GetNewWordsAfterExistingOnEdgeTest()
        {
            Game game = GetGameWithSingleVerticalWordAtTop();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(2, 4, 'E'),
                new TilePlacement(2, 5, 'R')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords();

            Assert.Contains("TESTER", actual);
        }


        // TO DO NEXT

        // Intersecting words (will probably want to factor out code to extend left/right/up/down
        // Single letter placement
    }
}
