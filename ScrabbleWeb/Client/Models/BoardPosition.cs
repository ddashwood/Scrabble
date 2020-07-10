using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Models
{
    public class BoardPosition : ITilePosition
    {
        private readonly Game game;
        private readonly Func<Task<char>> getBlankTile;
        public int X { get; }
        public int Y { get; }

        public BoardPosition(Game game, int x, int y, Func<Task<char>> blankTileGetter)
        {
            this.game = game;
            getBlankTile = blankTileGetter;
            X = x;
            Y = y;
        }

        public void RemoveTile()
        {
            game.Move.RemovePlacementAtPosition(X, Y);
        }

        public async Task AddTile(char tile)
        {
            // If the tile is not a capital letter, that means that
            // a blank tile is being removed from the board (or moved around the rack)
            if (tile < 'A' || tile > 'Z')
            {
                tile = await getBlankTile();
            }
            game.Move.AddPlacement(new TilePlacement(X, Y, tile));
        }

        public char GetTile()
        {
            char tile = game[X, Y];
            if (tile == ' ')
            {
                tile = game.Move.Placements.SingleOrDefault(p => (p.X, p.Y) == (X, Y))?.Tile ?? ' ';
            }

            return tile;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && (other.X, other.Y) == (X, Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
