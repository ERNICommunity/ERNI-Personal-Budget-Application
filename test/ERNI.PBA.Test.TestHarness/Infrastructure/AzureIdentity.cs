using System.Security.Claims;
using ERNI.PBA.Server.Domain.Security;
using ERNI.PBA.Test.TestHarness.Authentication;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public class AzureIdentity
    {
        public static AzureIdentity Default => new AzureIdentity
        {
            FirstName = "Joe",
            LastName = "Doe",
            UserName = "dow@erni.sk",
            UniqueIndetifier = "blqmvmhilzkvdleaxssdjuvzpwiusunbwnyfwlwayit",
            Role = Roles.Admin,
            Id = 1
        };

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string UniqueIndetifier { get; set; }

        public string Role { get; set; }

        public int Id { get; set; }

        public ClaimsIdentity ToIdentity()
        {
            return new ClaimsIdentity(
                new[]
                {
                    new Claim(Claims.FirstName, FirstName),
                    new Claim(Claims.LastName, LastName),
                    new Claim(Claims.UserName, UserName),
                    new Claim(Claims.UniqueIndetifier, UniqueIndetifier),
                    new Claim(Claims.Role, Role),
                    new Claim(Claims.Id, Id.ToString())
                }, Constants.TestAuthenticateScheme);
        }
    }
}
