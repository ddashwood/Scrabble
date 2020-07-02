using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ScrabbleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleData
{
    public class Player : IdentityUser
    {
        public virtual DbSet<GameData> GamesAsPlayer1 { get; set; }
        public virtual DbSet<GameData> GamesAsPlayer2 { get; set; }
    }
}
