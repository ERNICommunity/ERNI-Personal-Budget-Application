using System;
using ERNI.PBA.Test.TestHarness.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace ERNI.PBA.Test.TestHarness.Extensions
{
    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(Constants.TestAuthenticateScheme, Constants.TestAuthenticateSchemeName, configureOptions);
        }
    }
}
