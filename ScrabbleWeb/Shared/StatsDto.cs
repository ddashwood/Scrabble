using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleWeb.Shared
{
    public class StatsDto
    {
        public int Count { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public List<StatsPerOpponentDto> StatsPerOpponent { get; set; }
    }
}
