using Microsoft.AspNetCore.Authorization;

namespace ERNI.PBA.Server.Host.Auth
{
    public static class ScopeRequirementExtensions
    {
        public static AuthorizationPolicyBuilder RequireScope(
            this AuthorizationPolicyBuilder authorizationPolicyBuilder,
            params string[] requiredScopes) =>
            authorizationPolicyBuilder.AddRequirements(new ScopePolicyRequirement(requiredScopes));
    }
}