using System;
using ERNI.PBA.Test.TestHarness.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace ERNI.PBA.Test.TestHarness.Extensions
{
    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(Constants.TestAuthenticateScheme, Constants.TestAuthenticateSchemeName, configureOptions);
        }
    }
}
