using System.Collections.Generic;
using System.Security.Claims;
using ERNI.PBA.Server.Domain.Security;
using ERNI.PBA.Test.TestHarness.Authentication;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public class AzureIdentity
    {
        public static AzureIdentity Admin => new AzureIdentity
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
            var claims = new List<Claim> { new Claim(UserClaims.Id, Id.ToString()) };

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                claims.Add(new Claim(UserClaims.FirstName, FirstName));
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                claims.Add(new Claim(UserClaims.LastName, LastName));
            }

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                claims.Add(new Claim(UserClaims.UserName, UserName));
            }

            if (!string.IsNullOrWhiteSpace(UniqueIndetifier))
            {
                claims.Add(new Claim(UserClaims.UniqueIndetifier, UniqueIndetifier));
            }

            if (!string.IsNullOrWhiteSpace(Role))
            {
                claims.Add(new Claim(UserClaims.Role, Role));
                claims.Add(new Claim(ClaimTypes.Role, Role));
            }

            return new ClaimsIdentity(claims, Constants.TestAuthenticateScheme);
        }
    }
}
