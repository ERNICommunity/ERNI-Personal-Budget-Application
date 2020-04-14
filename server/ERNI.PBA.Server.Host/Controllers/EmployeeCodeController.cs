using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Employees;
using ERNI.PBA.Server.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeCodeController : Controller
    {
        private readonly Lazy<IGetEmployeeCodeQuery> _getEmployeeCodeQuery;

        public EmployeeCodeController(Lazy<IGetEmployeeCodeQuery> getEmployeeCodeQuery)
        {
            _getEmployeeCodeQuery = getEmployeeCodeQuery;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var outputModels = await _getEmployeeCodeQuery.Value.ExecuteAsync(new EmployeeCodeModel(), HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }
    }
}
