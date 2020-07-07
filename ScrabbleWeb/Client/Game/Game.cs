using AutoMapper;
using ScrabbleData;
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
        public Game(GameDto dto, IMapper mapper)
            : base(dto.Board)
        {
            mapper.Map(dto, this);

            Move = new MoveBase(this);
        }

        public int GameId { get; set; }
        public char[] MyTiles { get; set; }
        public bool MyMove { get; set; }
        public int MyScore { get; set; }
        public int OtherScore { get; set; }
        public string MyName { get; set; }
        public string OtherName { get; set; }
        public DateTime LastMove { get; set; }
        public bool IsComplete { get; set; }
        public WinnerDto Winner { get; set; }
        public MoveBase Move { get; set; }
    }
}
