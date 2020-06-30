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
    }
}
