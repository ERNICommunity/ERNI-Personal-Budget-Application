using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Queries.Budgets;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class TeamBudgetController : Controller
    {
        private readonly IMediator _mediator;

        public TeamBudgetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetId();
            var query = new GetBudgetByYearQuery
            {
                UserId = userId,
                Year = year
            };

            var outputModel = await _mediator.Send(query, cancellationToken);

            return Ok(outputModel);
        }
    }
}