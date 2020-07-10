using ScrabbleGame;
using ScrabbleBase;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class PlayMoveTests : TestBase
    {
        [Fact]
        public void PlayMoveTest()
        {
            Game game = GetGameWithEmptyBoard();
            Move move1 = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(6, 7, 'T'),
                new TilePlacement(7, 7, 'E'),
                new TilePlacement(8, 7, 'S'),
                new TilePlacement(9, 7, 'T')
            });
            Move move2 = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(10, 7, 'S'),
                new TilePlacement(10, 8, 'T'),
                new TilePlacement(10, 9, 'A'),
                new TilePlacement(10, 10, 'R')
            });

            Assert.False(move2.IsValidMove(out string error1));
            move1.Play();
            Assert.True(move2.IsValidMove(out string error2));
        }
    }
}
