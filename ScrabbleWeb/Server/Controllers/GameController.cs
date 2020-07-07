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
using ScrabbleMoveChecker;
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
        private readonly IWordCheckerFactory wordCheckerFactory;
        private readonly UserManager<Player> userManager;
        private readonly IMapper mapper;

        public GameController(ApplicationDbContext context,
            IWordCheckerFactory wordCheckerFactory,
            UserManager<Player> userManager,
            IMapper mapper)
        {
            this.context = context;
            this.wordCheckerFactory = wordCheckerFactory;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> Get(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            Game game = await context.Games.Where(g => g.GameId == id).ToGames(context, mapper).SingleAsync();
            if (game.Player1.Id != userId && game.Player2.Id != userId)
            {
                return Forbid();
            }

            return game.ToDto(userId);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task Get()
        {
            //var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var userId = "c6d3f1ed-4f5a-43eb-bb63-d241e37ad97a";

            var games = await context.Games
                .Where(g => g.Player1Id == userId || g.Player2Id == userId)
                .OrderByDescending(g => g.LastMove)
                .Take(3)
                .ToGames(context, mapper)
                .ToListAsync();
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

            Game game = new Game();
            game.SetupNewGame(userId, other.Id);

            GameData gameData = mapper.Map<GameData>(game);

            context.Games.Add(gameData);
            await context.SaveChangesAsync();

            return new NewGameDto { NewGameId = gameData.GameId };
        }
    }
}
