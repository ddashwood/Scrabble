using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleWeb.Shared
{
    public class GameListDto
    {
        public List<GameDto> ActiveGames { get; set; }
        public List<GameDto> RecentGames { get; set; }
    }
}
