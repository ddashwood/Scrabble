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
                new TilePlacement(0, 2, 'B'),
                new TilePlacement(1, 2, 'B')
            });

            Assert.False(move.IsValidMove(out string _));
            var e = Assert.Throws<InvalidOperationException>(() => move.FindWords());
            Assert.Equal("Invalid tile placement", e.Message);
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
            List<string> actual = move.FindWords().ToStringList();

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
            List<string> actual = move.FindWords().ToStringList();

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
            List<string> actual = move.FindWords().ToStringList();

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
            List<string> actual = move.FindWords().ToStringList();

            Assert.Contains("TESTER", actual);
        }

        [Fact]
        public void GetNewWordsFindCrossWordsTest()
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
            List<string> actual = move.FindWords().ToStringList();
            List<string> expected = new List<string> { "BLOB", "LT", "OE", "BS" };

            actual.Sort();
            expected.Sort();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetNewWordsSingleTileHorizontalTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(3, 5, 'B')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords().ToStringList();
            List<string> expected = new List<string> { "SB" };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetNewWordsSingleTileVerticalTest()
        {
            Game game = GetGameWithSingleHorizontalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(3, 5, 'B')
            });

            Assert.True(move.IsValidMove(out string _));
            List<string> actual = move.FindWords().ToStringList();
            List<string> expected = new List<string> { "BE" };

            Assert.Equal(expected, actual);
        }
    }
}
