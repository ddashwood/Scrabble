using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrabbleData;
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

        [HttpGet("{id}")]
        public GameDto Get(int id)
        {
            string board = string.Concat(Enumerable.Repeat("               ", 3)) +
               "  T            " + // TEST starts at position 2, 3
               "  E            " +
               "  m            " +
               "  P            " + // and extends to position 2, 6
               string.Concat(Enumerable.Repeat("               ", 8));
            return new GameDto { Board = board };
        }
    }
}
