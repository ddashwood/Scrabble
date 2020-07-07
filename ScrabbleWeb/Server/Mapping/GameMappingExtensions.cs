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
        public static GameDto ToDto(this Game game, string userId)
        {
            PlayerSelection thisPlayerSelection;
            if (game.Player1.Id == userId)
            {
                thisPlayerSelection = PlayerSelection.Player1;
            }
            else if (game.Player2.Id == userId)
            {
                thisPlayerSelection = PlayerSelection.Player2;
            }
            else
            {
                throw new InvalidOperationException("The User Id given is not a participant in this game");
            }

            GamePlayer thisPlayer;
            GamePlayer otherPlayer;
            (thisPlayer, otherPlayer) = thisPlayerSelection == PlayerSelection.Player1 ? (game.Player1, game.Player2) : (game.Player2, game.Player1);
            return new GameDto
            {
                GameId = game.GameId,
                MyTiles = thisPlayer.Tiles,
                MyMove = game.NextPlayer == thisPlayerSelection,
                MyScore = thisPlayer.Score,
                OtherScore = otherPlayer.Score,
                Board = game.Board,
                MyName = thisPlayer.Name,
                OtherName = otherPlayer.Name,
                LastMove = game.LastMove,
                Winner = game.Winner switch
                {
                    Winner.NotFinished => WinnerDto.NotFinished,
                    Winner.Player1 => thisPlayerSelection == PlayerSelection.Player1 ? WinnerDto.YouWon : WinnerDto.OtherPlayerWon,
                    Winner.Player2 => thisPlayerSelection == PlayerSelection.Player2 ? WinnerDto.YouWon : WinnerDto.OtherPlayerWon,
                    Winner.Draw => WinnerDto.Draw,
                    _ => throw new InvalidOperationException("Winner property invalid")
                }
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
