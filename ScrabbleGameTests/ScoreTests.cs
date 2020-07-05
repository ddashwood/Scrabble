using ScrabbleGame;
using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class ScoreTests : TestBase
    {
        [Fact]
        public void BasicScoreTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 4, 'C'),
                new TilePlacement(1, 4, 'L'),
                new TilePlacement(3, 4, 'F')
            });

            int actual = move.GetScore(out string _);
            int expected = 9; // CLEF

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AdjacentScoreTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 2, 'L'),
                new TilePlacement(1, 3, 'A'),
                new TilePlacement(1, 4, 'P')
            });

            int actual = move.GetScore(out string _);
            int expected = 5  // LAP
                         + 2  // AT
                         + 4; // PE

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WordMultiplerTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 1, 'F'),
                new TilePlacement(1, 2, 'L'),
                new TilePlacement(1, 3, 'A'),
                new TilePlacement(1, 4, 'P')
            });

            int actual = move.GetScore(out string _);
            int expected = 18 // FLAP with double word under the F
                         + 2  // AT
                         + 4; // PE

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LetterMultiplerTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 1, 'F'),
                new TilePlacement(1, 2, 'L'),
                new TilePlacement(1, 3, 'A'),
                new TilePlacement(1, 4, 'P'),
                new TilePlacement(1, 5, 'S')
            });

            int actual = move.GetScore(out string _);
            int expected = 24 // FLAPS with double word under the F, tripple letter under the S
                         + 2  // AT
                         + 4  // PE
                         + 4; // SS with tripple letter under the first S

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WordMultiplierMultipleWordsTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 2, 'L'),
                new TilePlacement(2, 2, 'A'),
                new TilePlacement(3, 2, 'P')
            });

            int actual = move.GetScore(out string _);
            int expected = 10  // LAPS with double word under the A
                         + 10; // ATEST with double word under the A

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BlankTileScoreTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(1, 1, 'F'),
                new TilePlacement(1, 2, 'L'),
                new TilePlacement(1, 3, 'a'),
                new TilePlacement(1, 4, 'P'),
                new TilePlacement(1, 5, 'S')
            });

            int actual = move.GetScore(out string _);
            int expected = 22 // FLaPS with double word under the F, tripple letter under the S
                         + 1  // AT
                         + 4  // PE
                         + 4; // SS with tripple letter under the first S

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FirstTileScoreTest()
        {
            Game game = GetGameWithEmptyBoard();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(7, 7, 'B'),
            });

            int actual = move.GetScore(out string _);

            Assert.Equal(3, actual);
        }

        [Fact]
        public void BingoBonusTest()
        {
            Game game = GetGameWithEmptyBoard();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(7, 7, 'A'),
                new TilePlacement(7, 8, 'A'),
                new TilePlacement(7, 9, 'A'),
                new TilePlacement(7, 10, 'A'),
                new TilePlacement(7, 11, 'A'),
                new TilePlacement(7, 12, 'A'),
                new TilePlacement(7, 13, 'A'),
            });

            int actual = move.GetScore(out string _);

            // Includes 50 point bonus for "Bingo"
            // Also, one of the A's is on a double-letter
            Assert.Equal(58, actual);
        }

        [Fact]
        public void FindInvalidWordsTest()
        {
            string words = $"FLAPS{Environment.NewLine}" +
                $"AT{Environment.NewLine}" +
                $"TEST";
            using (Stream wordsStream = GenerateStreamFromString(words))
            {
                var wordChecker = new FileWordChecker(wordsStream);

                Game game = GetGameWithSingleVerticalWord(wordChecker);
                Move move = new Move(game, new List<TilePlacement>
                {
                    new TilePlacement(1, 1, 'F'),
                    new TilePlacement(1, 2, 'L'),
                    new TilePlacement(1, 3, 'a'),
                    new TilePlacement(1, 4, 'P'),
                    new TilePlacement(1, 5, 'S')
                });

                var actual = move.InvalidWords().ToList();
                var expected = new List<string> { "PE", "SS" };

                actual.Sort();
                expected.Sort();
                Assert.Equal(expected, actual);
            }
        }
    }
}