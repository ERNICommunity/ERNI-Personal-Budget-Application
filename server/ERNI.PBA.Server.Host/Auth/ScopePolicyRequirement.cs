using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace ERNI.PBA.Server.Host.Auth
{
    public class ScopePolicyRequirement : AuthorizationHandler<ScopePolicyRequirement>, IAuthorizationRequirement
    {
        private readonly string[] _requiredScopes;

        public ScopePolicyRequirement(string[] requiredScopes)
        {
            if (requiredScopes is null || requiredScopes.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredScopes));
            }

            _requiredScopes = requiredScopes;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopePolicyRequirement requirement)
        {
            if (!context.User.Claims.Any())
            {
                context.Fail();
            }
            else
            {
                var scopeClaim = context.User.FindFirst(ClaimConstants.Scp) ?? context.User.FindFirst(ClaimConstants.Scope);
                if (scopeClaim?.Value.Split(' ').Intersect(_requiredScopes).Any() != true)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
