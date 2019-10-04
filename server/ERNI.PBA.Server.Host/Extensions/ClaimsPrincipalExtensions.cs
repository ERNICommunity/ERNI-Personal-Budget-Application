using System.Security.Claims;

namespace ERNI.PBA.Server.Host.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetIdentifier(this ClaimsPrincipal claimsPrincipal, string identifier)
        {
            return claimsPrincipal.FindFirstValue(identifier);
        }
    }
}
