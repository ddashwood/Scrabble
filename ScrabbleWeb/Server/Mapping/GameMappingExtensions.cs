using ScrabbleGame;
using ScrabbleWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Server.Mapping
{
    public static class GameMappingExtensions
    {
        public static GameDto ToDto(this Game game)
        {
            return new GameDto
            {
                Board = game.Board,
                PlayerTiles = game.Player1Tiles,
                OtherPlayerName = "Test Player"
            };
        }
    }
}
