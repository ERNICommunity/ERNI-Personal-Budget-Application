using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Host.Queries.Employees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeCodeController : Controller
    {
        private readonly IMediator _mediator;

        public EmployeeCodeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var outputModels = await _mediator.Send(new GetEmployeeCodeQuery(), cancellationToken);

            return Ok(outputModels);
        }
    }
}
