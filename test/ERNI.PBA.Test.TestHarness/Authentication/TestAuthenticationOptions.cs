using System;
using System.Security.Claims;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authentication;

namespace ERNI.PBA.Test.TestHarness.Authentication
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(
            new[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString()),
                new Claim(Claims.Id, 1000.ToString()),
                new Claim(Claims.Role, Roles.Admin)
            }, Constants.TestAuthenticateScheme);
    }
}
