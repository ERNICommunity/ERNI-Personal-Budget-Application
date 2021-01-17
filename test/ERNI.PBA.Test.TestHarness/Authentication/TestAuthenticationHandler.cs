using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ERNI.PBA.Test.TestHarness.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ERNI.PBA.Test.TestHarness.Authentication
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IIdentityService _identityService;

        public TestAuthenticationHandler(
            IIdentityService identityService,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _identityService = identityService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(_identityService.AzureIdentity.ToIdentity()),
                new AuthenticationProperties(),
                Constants.TestAuthenticateScheme);

            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }
}
