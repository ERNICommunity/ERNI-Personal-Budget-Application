using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Queries.TeamBudgets;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Superior + "," + Roles.Admin)]
    public class TeamBudgetController : Controller
    {
        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, [FromServices] IGetTeamBudgetByYearQuery query, CancellationToken cancellationToken)
        {
            var outputModel = await query.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }

        [HttpGet("default-team/{year}")]
        public async Task<IActionResult> GetDefaultTeamBudgetByYear(int year, [FromServices] GetDefaultTeamBudgetsQuery query, CancellationToken cancellationToken)
        {
            var outputModel = await query.ExecuteAsync((year, limitToOwnTeam: true), HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }

        [HttpGet("all-employees/{year}")]
        public async Task<IActionResult> GetAllTeamBudgetsByYear(int year, [FromServices] GetDefaultTeamBudgetsQuery query, CancellationToken cancellationToken)
        {
            var outputModel = await query.ExecuteAsync((year, limitToOwnTeam: false), HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }

        [HttpGet("requests/{year}")]
        public async Task<IActionResult> GetTeamBudgetRequests(int year, [FromServices] GetTeamBudgetRequestsQuery query, CancellationToken cancellationToken)
        {
            var outputModel = await query.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }

        [HttpPost("requests")]
        public async Task<IActionResult> CreateTeamBudgetRequest([FromBody] CreateTeamRequestCommand.NewTeamRequestModel payload, [FromServices] CreateTeamRequestCommand query, CancellationToken cancellationToken)
        {
            await query.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }
    }
}