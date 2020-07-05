using ScrabbleMoveChecker;
using ScrabbleWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleWeb.Client.Game
{
    public class Game : GameBase
    {
        public Game(GameDto dto)
            :base(dto.Board)
        {
            GameId = dto.GameId;
            PlayerTiles = dto.PlayerTiles.ToCharArray();
            Player1Score = dto.Player1Score;
            Player2Score = dto.Player2Score;
            IsPlayer1 = dto.IsPlayer1;
            OtherPlayerName = dto.OtherPlayerName;
            Move = new MoveBase(this);
        }

        public int GameId { get; set; }
        public char[] PlayerTiles { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public bool IsPlayer1 { get; set; }
        public string OtherPlayerName { get; set; }
        public MoveBase Move { get; set; }
    }
}
