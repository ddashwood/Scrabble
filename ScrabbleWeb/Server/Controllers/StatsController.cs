using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrabbleWeb.Server.Data;
using ScrabbleData;
using ScrabbleGame;
using Microsoft.EntityFrameworkCore;
using ScrabbleWeb.Shared;

namespace ScrabbleWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<Player> userManager;
        private readonly IMapper mapper;

        public StatsController(ApplicationDbContext context,
            UserManager<Player> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<StatsDto> Get()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            var data = from game in context.Games
                       where (game.Player1Id == userId || game.Player2Id == userId)
                                && game.Winner != Winner.NotFinished
                       group game by 1 into games
                       select new StatsDto
                       {
                           Count = games.Count(),
                           // Count(Func<>) isn't runnable in SQL
                           Wins = games.Sum(g => (g.Player1Id == userId && g.Winner == Winner.Player1)
                                              || (g.Player2Id == userId && g.Winner == Winner.Player2) ? 1 : 0),
                           Draws = games.Sum(g => g.Winner == Winner.Draw ? 1 : 0)
                       };

            var opps = from game in context.Games
                       join player1 in context.Users on game.Player1Id equals player1.Id
                       join player2 in context.Users on game.Player2Id equals player2.Id
                       where (game.Player1Id == userId || game.Player2Id == userId)
                                && game.Winner != Winner.NotFinished
                       let opponent = (game.Player1Id == userId ? player2.Name : player1.Name)
                       group game by opponent into games
                       orderby games.Count() descending
                       select new StatsPerOpponentDto
                       {
                           Opponent = games.Key,
                           Count = games.Count(),
                           Wins = games.Sum(g => (g.Player1Id == userId && g.Winner == Winner.Player1)
                                              || (g.Player2Id == userId && g.Winner == Winner.Player2) ? 1 : 0),
                           Draws = games.Sum(g => g.Winner == Winner.Draw ? 1 : 0)
                       };

            var dtoTask = data.SingleAsync();
            var oppsTask = opps.ToListAsync();

            await Task.WhenAll(dtoTask, oppsTask);
            var dto = dtoTask.Result;
            dto.StatsPerOpponent = oppsTask.Result;

            return dto;
        }
    }
}
