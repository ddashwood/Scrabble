using ScrabbleMoveChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    public class BoardPosition : ITilePosition
    {
        private readonly Game game;
        public int X { get; }
        public int Y { get; }

        public BoardPosition(Game game, int x, int y)
        {
            this.game = game;
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && (other.X, other.Y) == (X, Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public void RemoveTile()
        {
            game.Move.RemovePlacementAtPosition(X, Y);
        }

        public void AddTile(char tile)
        {
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
    }
}
