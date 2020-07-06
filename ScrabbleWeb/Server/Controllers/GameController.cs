using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrabbleData;
using ScrabbleGame;
using ScrabbleMoveChecker;
using ScrabbleWeb.Server.Data;
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
        public GameController(ApplicationDbContext context, 
            IWordCheckerFactory wordCheckerFactory,
            UserManager<Player> userManager)
        {
            this.context = context;
            this.wordCheckerFactory = wordCheckerFactory;
            this.userManager = userManager;
            game = game ?? new Game(wordCheckerFactory.GetWordChecker());
        }

        static Game game;

        [HttpGet("{id}")]
        public GameDto Get(int id)
        {
            string board = game.Board;
            return new GameDto
            {
                Board = board,
                PlayerTiles = game.Player1Tiles,
                OtherPlayerName = "Test Player"
            };
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
            GameData gameData = new GameData
            {
                Player1Id = game.Player1Id,
                Player2Id = game.Player2Id,
                RemainingTiles = "ABC",
                Player1Tiles = "ABC",
                Player2Tiles = "ABC",
                Player1Score = 0,
                Player2Score = 0,
                Board = game.Board,
                LastMove = DateTime.Now,
                IsComplete = false,
                Winner = Winner.NotFinished
            };

            context.Games.Add(gameData);
            await context.SaveChangesAsync();

            return new NewGameDto { NewGameId = gameData.GameId };
        }

        [HttpPost("/api/move/{id}")]
        public ActionResult<MoveResultDto> Post(List<TilePlacement> placements, int id)
        {
            var move = new Move(game);
            foreach (var placement in placements)
            {
                move.AddPlacement(placement);
            }

            int score = move.GetScore(out string error);
            if (!string.IsNullOrEmpty(error))
            {
                return UnprocessableEntity(new MoveResultDto(error));
            }

            var badWords = move.InvalidWords().ToList();

            if (badWords.Count() != 0)
            {
                var result = new MoveResultDto("The following words are not allowed: " + string.Join(", ", badWords));
                result.InvalidWords = badWords;
                return UnprocessableEntity(result);
            }

            move.Play();
            return Ok(new MoveResultDto(Get(id)));
        }
    }
}
