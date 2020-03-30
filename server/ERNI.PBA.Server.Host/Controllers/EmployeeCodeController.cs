using ERNI.PBA.Server.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeCodeController : Controller
    {
        private readonly IConfiguration _configuration;

        private static DateTime _timestamp;
        private static User[] _cache;

        public EmployeeCodeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            if (_cache != null && _timestamp.AddHours(4) > DateTime.Now)
            {
                return Ok(_cache);
            }

            var config = new GraphConfiguration(_configuration["Graph:ClientId"],
                _configuration["Graph:TenantId"],
                _configuration["Graph:ClientSecret"]);

            var f = new GraphFacade(config);

            var users = await f.GetUsers();

            var data = users
                .Where(_ => _.UserPrincipalName.Contains("@"))
                .Select(_ => new User
            {
                LastName = _.Surname,
                FirstName = _.GivenName,
                DisplayName = _.DisplayName,
                Code = _.UserPrincipalName.Split('@')[0]
            }).ToArray();

            _timestamp = DateTime.Now;
            _cache = data;

            return Ok(data);
        }

        private class User
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string DisplayName { get; set; }
            public string Code { get; set; }
        }
    }
}
