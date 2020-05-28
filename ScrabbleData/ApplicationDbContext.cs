using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrabbleData
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<PlayerData>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GameData>(entity =>
            {
                entity
                    .ToTable("Game")
                    .HasKey(g => g.GameId);
                entity
                    .HasOne(g => g.Player1)
                    .WithMany(u => u.GamesAsPlayer1);
                entity
                    .HasOne(g => g.Player2)
                    .WithMany(u => u.GamesAsPlayer2);
            });
        }
    }
}
