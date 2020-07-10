using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Models
{
    public class RackPosition : ITilePosition
    {
        private readonly Game game;
        public int Space { get; }

        public RackPosition(Game game, int space)
        {
            this.game = game;
            Space = space;
        }

        public void RemoveTile()
        {
            game.MyTiles[Space] = ' ';
        }

        public Task AddTile(char tile)
        {
            // If the tile is not a capital letter, that means that
            // a blank tile is being removed from the board (or moved around the rack)
            if (tile < 'A' || tile > 'Z')
            {
                tile = '*';
            }

            game.MyTiles[Space] = tile;
            return Task.CompletedTask;
        }

        public char GetTile()
        {
            return game.MyTiles[Space];
        }

        public override bool Equals(object obj)
        {
            return obj is RackPosition other && other.Space == Space;
        }

        public override int GetHashCode()
        {
            return Space.GetHashCode();
        }
    }
}
