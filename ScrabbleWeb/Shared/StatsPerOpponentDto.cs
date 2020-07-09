using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleWeb.Shared
{
    public class StatsPerOpponentDto
    {
        public string Opponent { get; set; }
        public int Count { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
    }
}
