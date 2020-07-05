using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
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

        public override bool Equals(object obj)
        {
            return obj is RackPosition other && other.Space == Space;
        }

        public override int GetHashCode()
        {
            return Space.GetHashCode();
        }

        public void RemoveTile()
        {
            game.PlayerTiles[Space] = '-';
        }

        public void AddTile(char tile)
        {
            game.PlayerTiles[Space] = tile;
        }

        public char GetTile()
        {
            return game.PlayerTiles[Space];
        }
    }
}
