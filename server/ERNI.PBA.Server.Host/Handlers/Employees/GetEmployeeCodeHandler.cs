using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Graph;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.Employees;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.Host.Handlers.Employees
{
    public class GetEmployeeCodeHandler : IRequestHandler<GetEmployeeCodeQuery, AdUserOutputModel[]>
    {
        private static DateTime _timestamp;
        private static AdUserOutputModel[] _cache;

        private readonly IConfiguration _configuration;

        public GetEmployeeCodeHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AdUserOutputModel[]> Handle(GetEmployeeCodeQuery request, CancellationToken cancellationToken)
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
