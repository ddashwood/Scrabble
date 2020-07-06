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
        public GameDto Get(int id)
        {
            GameData gameData = context.Games.Single(g => g.GameId == id);
            Game game = mapper.Map<Game>(gameData);

            return game.ToDto();
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
            gameData.LastMove = DateTime.Now;
            gameData.IsComplete = false;
            gameData.Winner = Winner.NotFinished;

            context.Games.Add(gameData);
            await context.SaveChangesAsync();

            return new NewGameDto { NewGameId = gameData.GameId };
        }
    }
}
