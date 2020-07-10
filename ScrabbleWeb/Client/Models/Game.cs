using AutoMapper;
using ScrabbleData;
using ScrabbleBase;
using ScrabbleWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Models
{
    public class Game : GameBase
    {
        public Game(GameDto dto, IMapper mapper)
            : base(dto.Board)
        {
            mapper.Map(dto, this);

            MyOriginalTiles = (char[])MyTiles.Clone();
            Move = new MoveBase(this);
        }

        public string MyUserId { get; set; }
        public int GameId { get; set; }
        public char[] MyTiles { get; set; }
        public char[] MyOriginalTiles { get; set; } // Used when recalling tiles
        public bool MyMove { get; set; }
        public int MyScore { get; set; }
        public int OtherScore { get; set; }
        public string MyName { get; set; }
        public string OtherName { get; set; }
        public DateTime LastMove { get; set; }
        public bool IsComplete { get; set; }
        public WinnerDto Winner { get; set; }
        public MoveBase Move { get; set; }
        public int TilesRemaining { get; set; }
        public string LastMoveDescription { get; set; }
        public List<LastMoveTileDto> LastMoveTiles { get; internal set; }

        internal async Task MoveTile(ITilePosition from, ITilePosition to)
        {
            var tile = from.GetTile();
            from.RemoveTile();
            await to.AddTile(tile);

            if (to.GetTile() == '#')
            {
                // The user placed a blank tile, but then did not choose a letter
                RemoveBlankToRack(to);
            }
        }

        private void RemoveBlankToRack(ITilePosition position)
        {
            position.RemoveTile();
            MyTiles[Array.IndexOf(MyTiles, ' ')] = '*';
        }

        internal void MoveTilesOnRack(RackPosition from, RackPosition to)
        {
            char beingMoved = from.GetTile();

            if (from.Space < to.Space)
            {
                // Moving tile to the right - shuffle intermmediate tiles to the left
                for (int i = from.Space; i < to.Space; i++)
                {
                    MyTiles[i] = MyTiles[i + 1];
                }
            }
            else
            {
                // Moving tile to the left - shuffle intermmediate tiles to the right
                for (int i = from.Space; i > to.Space; i--)
                {
                    MyTiles[i] = MyTiles[i - 1];
                }
            }

            MyTiles[to.Space] = beingMoved;
        }
    }
}
