using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Requests;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Output;
using ERNI.PBA.Server.Domain.Output.PendingRequests;
using ERNI.PBA.Server.Domain.Queries.Budgets;
using ERNI.PBA.Server.Domain.Queries.Requests;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IMediator _mediator;

        public RequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequest(int id, CancellationToken cancellationToken)
        {
            var requestQuery = new GetRequestQuery
            {
                Principal = HttpContext.User,
                RequestId = id
            };

            var request = await _mediator.Send(requestQuery, cancellationToken);

            return Ok(request);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            var approveRequestCommand = new ApproveRequestCommand { RequestId = id };
            await _mediator.Send(approveRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            var rejectRequestCommand = new RejectRequestCommand { RequestId = id };
            await _mediator.Send(rejectRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest([FromBody]PostRequestModel payload, CancellationToken cancellationToken)
        {
            var addRequestCommand = new AddRequestCommand
            {
                BudgetId = payload.BudgetId,
                UserId = User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date
            };

            await _mediator.Send(addRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("team")]
        public async Task<IActionResult> AddTeamRequest([FromBody] PostRequestModel payload, CancellationToken cancellationToken)
        {
            var addTeamRequestCommand = new AddTeamRequestCommand
            {
                BudgetId = payload.BudgetId,
                UserId = User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date
            };

            await _mediator.Send(addTeamRequestCommand, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state.
        /// </summary>
        [HttpPost("mass")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddRequestMass([FromBody] RequestMassModel payload, CancellationToken cancellationToken)
        {
            var addMassRequestCommand = new AddMassRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                State = payload.State,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date,
                Users = payload.Users
            };

            await _mediator.Send(addMassRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/pending")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetPendingRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Pending }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/approved")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetApprovedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Approved }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/approvedBySuperior")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetApprovedBySuperiorRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.ApprovedBySuperior }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/rejected")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetRejectedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Rejected }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var updateRequestCommand = new UpdateRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                RequestId = payload.Id,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date
            };

            await _mediator.Send(updateRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var updateTeamRequestCommand = new UpdateTeamRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                RequestId = payload.Id,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date
            };

            await _mediator.Send(updateTeamRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, CancellationToken cancellationToken)
        {
            var deleteRequestCommand = new DeleteRequestCommand
            {
                Principal = HttpContext.User,
                RequestId = id
            };

            await _mediator.Send(deleteRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpGet("budget-left/{amount}/{year}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<UserModel[]> BudgetLeft(decimal amount, int year, CancellationToken cancellationToken)
        {
            var getBudgetLeftQuery = new GetBudgetLeftQuery
            {
                Amount = amount,
                Year = year
            };

            return await _mediator.Send(getBudgetLeftQuery, cancellationToken);
        }
    }
}