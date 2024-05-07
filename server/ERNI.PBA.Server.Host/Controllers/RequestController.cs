using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Requests;
using ERNI.PBA.Server.Business.Queries.Budgets;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController(
        Lazy<IGetRequestsQuery> getRequestsQuery,
        Lazy<ISetRequestStateCommand> approveRequestCommand,
        Lazy<IAddMassRequestCommand> addMassRequestCommand,
        Lazy<IUpdateRequestCommand> updateRequestCommand,
        Lazy<IUpdateTeamRequestCommand> updateTeamRequestCommand,
        Lazy<IDeleteRequestCommand> deleteRequestCommand) : Controller
    {
        [HttpGet("{id}")]
#pragma warning disable CA1721 // Property names should not match get methods
        public async Task<IActionResult> GetRequest([FromServices] IGetRequestQuery query, int id, CancellationToken cancellationToken)
#pragma warning restore CA1721 // Property names should not match get methods
        {
            var request = await query.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok(request);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            await approveRequestCommand.Value.ExecuteAsync((id, RequestState.Approved), HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            await approveRequestCommand.Value.ExecuteAsync((id, RequestState.Rejected), HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRequest([FromBody] AddRequestCommand.PostRequestModel payload,
            [FromServices] AddRequestCommand addRequestCommand, CancellationToken cancellationToken)
        {
            var id = await addRequestCommand.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok(id);
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state.
        /// </summary>
        [HttpPost("mass")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddMassRequest([FromBody] MassRequestModel payload, CancellationToken cancellationToken)
        {
            await addMassRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/state/{requestState}/type/{budgetTypeId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<IGetRequestsQuery.RequestModel[]> GetRequests(int year, RequestState requestState, int budgetTypeId, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = [requestState],
                BudgetTypeId = budgetTypeId
            };

            return await getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await updateRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await updateTeamRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, CancellationToken cancellationToken)
        {
            await deleteRequestCommand.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("personal-budget-left")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<GetBudgetLeftQuery.UserModel[]> BudgetLeft([FromServices] GetBudgetLeftQuery query, CancellationToken cancellationToken) =>
            await query.ExecuteAsync(HttpContext.User, cancellationToken);
    }
}