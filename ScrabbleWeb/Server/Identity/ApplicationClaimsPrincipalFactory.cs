using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScrabbleWeb.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScrabbleWeb.Server.Identity
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<Player>
    {
        public ApplicationClaimsPrincipalFactory(UserManager<Player> userManager, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Player user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            bool success = identity.TryRemoveClaim(identity.FindFirst("name"));
            identity.AddClaim(new Claim("name", user.Name));

            return identity;
        }
    }
}
