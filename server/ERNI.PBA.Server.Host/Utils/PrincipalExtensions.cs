using System;
using System.Security.Claims;

namespace ERNI.PBA.Server.Utils
{
    public static class PrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(c => c.Type == Claims.Id);

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
    }
}