using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IWordCheckerFactory wordCheckerFactory;
        public GameController(ApplicationDbContext context, IWordCheckerFactory wordCheckerFactory)
        {
            this.context = context;
            this.wordCheckerFactory = wordCheckerFactory;
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

        [HttpPost("{id}")]
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
