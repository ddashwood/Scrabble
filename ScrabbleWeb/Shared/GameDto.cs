using System;

namespace ScrabbleWeb.Shared
{
    public class GameDto
    {
        public int GameId { get; set; }
        public string PlayerTiles { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public string Board { get; set; }
        public bool IsPlayer1 { get; set; }
        public string OtherPlayerName { get; set; }
    }
}
