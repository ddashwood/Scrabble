using ScrabbleWeb.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrabbleData;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;

namespace ScrabbleWeb.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<Player>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<GameData> Games { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Change default table names to singular
            builder.Entity<Player>(entity => { entity.ToTable(name: "Player"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Role"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRole"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaim"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogin"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserToken"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaim"); });

            builder.Entity<GameData>(entity =>
            {
                entity
                    .ToTable("Game")
                    .HasKey(g => g.GameId);
                entity
                    .HasOne<Player>()
                    .WithMany(p => p.GamesAsPlayer1)
                    .HasForeignKey("Player1Id")
                    .HasPrincipalKey("Id");
                entity
                    .HasOne<Player>()
                    .WithMany(p => p.GamesAsPlayer2)
                    .HasForeignKey("Player2Id")
                    .HasPrincipalKey("Id");
            });
        }
    }

}
