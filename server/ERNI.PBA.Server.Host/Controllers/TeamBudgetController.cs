using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class TeamBudgetController : Controller
    {
        private readonly Lazy<IGetTeamBudgetByYearQuery> _getBudgetByYearQuery;

        public TeamBudgetController(Lazy<IGetTeamBudgetByYearQuery> getBudgetByYearQuery) => _getBudgetByYearQuery = getBudgetByYearQuery;

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var outputModel = await _getBudgetByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }
    }
}