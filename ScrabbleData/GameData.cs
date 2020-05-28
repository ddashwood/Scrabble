using System;
using System.ComponentModel.DataAnnotations;

namespace ScrabbleData
{
    public class GameData
    {
        public int GameId { get; set; }

        public string RemainingTiles { get; set; }
        public string Player1Tiles { get; set; }
        public string Player2Tiles { get; set; }

        [Required] public int Player1Score { get; set; }
        [Required] public int Player2Score { get; set; }
        [Required] public string Board { get; set; }

        [Required] public PlayerData Player1 { get; set; }
        [Required] public PlayerData Player2 { get; set; }
    }
}
