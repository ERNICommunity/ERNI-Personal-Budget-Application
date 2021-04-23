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
        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, [FromServices] IGetTeamBudgetByYearQuery query, CancellationToken cancellationToken)
        {
            var outputModel = await query.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }
    }
}