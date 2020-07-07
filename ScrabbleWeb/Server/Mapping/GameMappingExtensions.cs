using AutoMapper;
using ScrabbleData;
using ScrabbleGame;
using ScrabbleWeb.Server.Data;
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
                MyTiles = game.Player1.Tiles,
                OtherName = "Test Player"
            };
        }

        public static IQueryable<Game> ToGames(this IQueryable<GameData> dtos, ApplicationDbContext dbContext, IMapper mapper)
        {
            return from game in dtos
                   join player1 in dbContext.Users on game.Player1Id equals player1.Id
                   join player2 in dbContext.Users on game.Player2Id equals player2.Id
                   let gameBeforePlayerNames = mapper.Map<Game>(game)
                   let playerNames = new GamePlayers { Player1Name = player1.Name, Player2Name = player2.Name }
                   select mapper.Map(playerNames, gameBeforePlayerNames);
        }
    }
}
