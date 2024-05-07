using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorizationController : Controller
    {
        [HttpGet("userInfo")]
        public IActionResult GetUserInfo() => Ok(new
        {
            Name = HttpContext.User.GetIdentifier(UserClaims.Name),
            Roles = GetRoles().ToArray()
        });

        private IEnumerable<string> GetRoles()
        {
            var roles = new[] { Roles.Admin, Roles.Finance, Roles.Superior, Roles.CommunityLeader };

            return roles.Where(HttpContext.User.IsInRole);
        }
    }
}
