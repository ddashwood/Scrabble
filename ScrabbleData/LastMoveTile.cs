using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScrabbleData
{
    public class LastMoveTile
    {
        [Required] public int GameId { get; set; }
        [Required] public int TileId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public virtual GameData Game { get; set; }
    }
}
