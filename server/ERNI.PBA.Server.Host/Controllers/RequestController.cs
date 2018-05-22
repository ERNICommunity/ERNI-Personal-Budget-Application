using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Examples;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;

namespace server.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestRepository _requestRepository;

        public RequestController(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUsersBudget(int year, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, HttpContext.User.GetId(), cancellationToken);

            var result = requests.Select(_ => new {
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date
            });

            return Ok(result);
        }

        [HttpGet("user/{userId}/year/{year}")]
        public async Task<IActionResult> GetUsersBudget(int userId, int year, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, userId, cancellationToken);

            var result = requests.Select(_ => new {
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date
            });

            return Ok(result);
        }

        [HttpGet("pending")]
        [SwaggerResponseExample(200, typeof(PendingRequestExample))]
        public async Task<IActionResult> GetPendingRequests(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}