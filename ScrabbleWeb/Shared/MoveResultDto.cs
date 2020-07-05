using System;
using System.Collections.Generic;
using System.Text;

namespace ScrabbleWeb.Shared
{
    public class MoveResultDto
    {
        public MoveResultDto(string error)
        {
            Error = error;
        }
        public MoveResultDto(GameDto gameDto)
        {
            GameDto = gameDto;
        }
        public MoveResultDto()
        { }

        public string Error { get; set; }
        public List<string> InvalidWords { get; set; }

        public GameDto GameDto { get; set; }
    }
}
