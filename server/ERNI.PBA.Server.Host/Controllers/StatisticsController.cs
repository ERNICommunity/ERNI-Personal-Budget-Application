using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Queries;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class StatisticsController : Controller
    {
        [HttpGet("{year}")]
        [Authorize]
        public async Task<IActionResult> GetStatistics(int year, [FromServices] GetStatisticsQuery query, CancellationToken cancellationToken)
        {
            var outputModels = await query.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }
    }
}
