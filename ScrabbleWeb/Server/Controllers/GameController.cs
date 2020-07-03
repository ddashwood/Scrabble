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
        public GameController(ApplicationDbContext context)
        {
            this.context = context;
        }

        static Game game = new Game();

        [HttpGet("{id}")]
        public GameDto Get(int id)
        {
            string board = game.Board;
            // string.Concat(Enumerable.Repeat("               ", 3)) +
            //"  T            " + // TEST starts at position 2, 3
            //"  E            " +
            //"  m            " +
            //"  P            " + // and extends to position 2, 6
            //string.Concat(Enumerable.Repeat("               ", 8));
            return new GameDto
            {
                Board = board,
                PlayerTiles = game.Player1Tiles,
                OtherPlayerName = "Test Player"
            };
        }

        [HttpPost]
        public IEnumerable<string> Post(List<TilePlacement> placements)
        {
            var move = new Move(game);
            foreach (var placement in placements)
            {
                move.AddPlacement(placement);
            }

            var badWords = move.InvalidWords().ToList();

            if (badWords.Count() == 0)
            {
                move.Play();
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            }

            return badWords;
        }
    }
}
