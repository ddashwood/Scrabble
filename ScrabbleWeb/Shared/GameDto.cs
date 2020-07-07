using ScrabbleData;
using System;

namespace ScrabbleWeb.Shared
{
    public class GameDto
    {
        public int GameId { get; set; }
        public string MyTiles { get; set; }
        public bool MyMove { get; set; }
        public int MyScore { get; set; }
        public int OtherScore { get; set; }
        public string Board { get; set; }
        public string MyName { get; set; }
        public string OtherName { get; set; }
        public DateTime LastMove { get; set; }
        public bool IsComplete { get; set; }
        public Winner Winner { get; set; }
    }
}
