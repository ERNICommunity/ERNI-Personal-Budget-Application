using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Employees;
using ERNI.PBA.Server.Domain.Models.Responses;
using ERNI.PBA.Server.Graph;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.Business.Queries.Employees
{
    public class GetEmployeeCodeQuery : Query<AdUserOutputModel[]>, IGetEmployeeCodeQuery
    {
        private static DateTime _timestamp;
        private static AdUserOutputModel[] _cache;

        private readonly IConfiguration _configuration;

        public GetEmployeeCodeQuery(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task<AdUserOutputModel[]> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (_cache != null && _timestamp.AddHours(4) > DateTime.Now)
            {
                return _cache;
            }

            var config = new GraphConfiguration(
                _configuration["Graph:ClientId"],
                _configuration["Graph:TenantId"],
                _configuration["Graph:ClientSecret"]);

            var f = new GraphFacade(config);

            var users = await f.GetUsers();

            var data = users
                .Where(_ => _.UserPrincipalName.Contains("@"))
                .Select(_ => new AdUserOutputModel
                {
                    LastName = _.Surname,
                    FirstName = _.GivenName,
                    DisplayName = _.DisplayName,
                    Code = _.UserPrincipalName.Split('@')[0]
                }).ToArray();

            _timestamp = DateTime.Now;
            _cache = data;

            return data;
        }
    }
}
