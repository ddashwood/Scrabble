using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ScrabbleData;
using ScrabbleGame;
using ScrabbleMoveChecker;
using ScrabbleWeb.Server.Data;
using ScrabbleWeb.Server.Hubs;
using ScrabbleWeb.Server.Mapping;
using ScrabbleWeb.Shared;

namespace ScrabbleWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class MoveController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IWordCheckerFactory wordCheckerFactory;
        private readonly IMapper mapper;
        private readonly IHubContext<MoveHub> hubContext;

        public MoveController(ApplicationDbContext context,
            IWordCheckerFactory wordCheckerFactory,
            IMapper mapper,
            IHubContext<MoveHub> hubContext)
        {
            this.context = context;
            this.wordCheckerFactory = wordCheckerFactory;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<MoveResultDto>> Post(List<TilePlacement> placements, int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            GameData gameData = await context.Games.Include(g => g.LastMoveTiles).SingleAsync(g => g.GameId == id);
            if (gameData.Player1Id != userId && gameData.Player2Id != userId)
            {
                return Forbid();
            }

            Game game = await context.Games.Where(g => g.GameId == id).ToGames(context, mapper).SingleAsync();
            game.WordChecker = wordCheckerFactory.GetWordChecker();

            var move = new Move(game, userId);
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

            mapper.Map(game, gameData);

            await context.SaveChangesAsync();
            var otherUser = gameData.Player1Id == userId ? gameData.Player2Id : gameData.Player1Id;
            await hubContext.Clients.All.SendAsync("ReceiveMove", otherUser, id);

            return Ok(new MoveResultDto(game.ToDto(userId)));
        }

    }
}
