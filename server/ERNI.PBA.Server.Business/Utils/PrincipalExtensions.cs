using System;
using System.Security.Claims;
using ERNI.PBA.Server.Domain.Security;

namespace ERNI.PBA.Server.Business.Utils
{
    public static class PrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(c => c.Type == UserClaims.Id);

            if (claim == null)
            {
                throw new InvalidOperationException("No Id claim found");
            }

            if (!int.TryParse(claim.Value, out var id))
            {
                throw new InvalidOperationException("Id claim value invalid");
            }

            return id;
        }

        public static string GetIdentifier(this ClaimsPrincipal claimsPrincipal, string identifier)
        {
            return claimsPrincipal.FindFirstValue(identifier);
        }
    }
}