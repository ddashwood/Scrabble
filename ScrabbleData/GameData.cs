using System;
using System.ComponentModel.DataAnnotations;

namespace ScrabbleData
{
    public class GameData
    {
        public int GameId { get; set; }
        [Required] public string Player1Id { get; set; }
        [Required] public string Player2Id { get; set; }

        [Required] public string RemainingTiles { get; set; }
        [Required] public string Player1Tiles { get; set; }
        [Required] public string Player2Tiles { get; set; }

        [Required] public int Player1Score { get; set; }
        [Required] public int Player2Score { get; set; }
        [Required] public string Board { get; set; }

        [Required] public DateTime LastMove { get; set; }
        [Required] public Winner Winner { get; set; }
        [Required] public PlayerSelection NextPlayer { get; set; }
        public string LastMoveDescription { get; set; }

    }
}
