using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrabbleData;
using ScrabbleGame;
using ScrabbleBase;
using ScrabbleWeb.Server.Data;
using ScrabbleWeb.Server.Mapping;
using ScrabbleWeb.Shared;

namespace ScrabbleWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<Player> userManager;
        private readonly IMapper mapper;

        public GameController(ApplicationDbContext context,
            UserManager<Player> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> Get(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            Game game = await context.Games.Include(g => g.LastMoveTiles).Where(g => g.GameId == id).ToGames(context, mapper).SingleAsync();
            if (game.Player1.Id != userId && game.Player2.Id != userId)
            {
                return Forbid();
            }

            return game.ToDto(userId);
        }

        [HttpGet]
        public async Task<GameListDto> Get()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

            var activeGames = await context.Games
                .Where(g => (g.Player1Id == userId || g.Player2Id == userId)
                            && g.Winner == Winner.NotFinished)
                .OrderByDescending(g => g.LastMove)
                .ToGames(context, mapper)
                .ToListAsync();

            var recentGames = await context.Games
                .Where(g => (g.Player1Id == userId || g.Player2Id == userId)
                            && g.Winner != Winner.NotFinished)
                .OrderByDescending(g => g.LastMove)
                .Take(5)
                .ToGames(context, mapper)
                .ToListAsync();

            return new GameListDto
            {
                MyUserId = userId,
                ActiveGames = activeGames.Select(g => g.ToDto(userId)),
                RecentGames = recentGames.Select(g => g.ToDto(userId))
            };
        }

        [HttpPost]
        public async Task<ActionResult<NewGameDto>> Post([FromQuery]int rematch)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            GameData game = await context.Games.SingleAsync(g => g.GameId == rematch);
            if (game.Player1Id != userId && game.Player2Id != userId)
            {
                return Forbid();
            }

            return await CreateGameAsync(game.Player1Id, game.Player2Id);
        }

        [HttpPost("{email}")]
        public async Task<ActionResult<NewGameDto>> Post(string email)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            //var user = await userManager.FindByIdAsync(userId);
            if (userId == null)
            {
                Response.StatusCode = 500; // Since this should never happen
                return new NewGameDto { Error = "Could not find your user ID" };
            }

            var other = await userManager.FindByEmailAsync(email);
            if (other == null || other.Id == null)
            {
                return UnprocessableEntity(new NewGameDto { Error = "Could not find a user with that e-mail address" });
            }

            return await CreateGameAsync(userId, other.Id);
        }

        private async Task<ActionResult<NewGameDto>> CreateGameAsync(string player1Id, string player2Id)
        {
            Game game = new Game();
            game.SetupNewGame(player1Id, player2Id);

            GameData gameData = mapper.Map<GameData>(game);

            context.Games.Add(gameData);
            await context.SaveChangesAsync();

            return new NewGameDto { NewGameId = gameData.GameId };
        }
    }
}
