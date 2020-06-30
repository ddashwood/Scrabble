using ScrabbleData;
using ScrabbleGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ScrabbleGameTests
{
    public class IsValidMoveTests : TestBase
    {
        [Fact]
        public void ValidMoveTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move badMove = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(1, 6, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            });
            Move goodMove = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            });

            Assert.False(badMove.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);
            Assert.True(goodMove.IsValidMove(out _));
        }

        [Fact]
        public void NewTilesMustTouchOtherTilesTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move badMove = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 8, 'P'),
                new TilePlacement(1, 8, 'A'),
                new TilePlacement(2, 8, 'S'),
                new TilePlacement(3, 8, 'T')
            });
            Move goodMove = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            });

            Assert.False(badMove.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);
            Assert.True(goodMove.IsValidMove(out _));
        }

        [Fact]
        public void TilesMustBeInALineTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 8, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T')
            });

            Assert.False(move.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);
        }

        [Fact]
        public void NoGapsInNewTilesTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 5, 'P'),
                new TilePlacement(3, 5, 'T')
            });

            Assert.False(move.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);

            move.AddPlacement(new TilePlacement(1, 5, 'A'));
            Assert.True(move.IsValidMove(out _));
        }

        [Fact]
        public void CantOverwriteExistingTileTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 5, 'P'),
                new TilePlacement(1, 5, 'A'),
                new TilePlacement(2, 5, 'S'),
                new TilePlacement(3, 5, 'T')
            });

            Assert.False(move.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);

        }

        [Fact]
        public void DuplicatesNotAllowedTest()
        {
            Game game = GetGameWithSingleVerticalWord();
            Move move = new Move(game, new List<TilePlacement>
            {
                new TilePlacement(0, 7, 'P'),
                new TilePlacement(1, 7, 'A'),
                new TilePlacement(2, 7, 'S'),
                new TilePlacement(3, 7, 'T'),
                new TilePlacement(2, 7, 'T')
            });

            Assert.False(move.IsValidMove(out string error));
            Assert.Equal("Invalid tile placement", error);
        }

    }
}
