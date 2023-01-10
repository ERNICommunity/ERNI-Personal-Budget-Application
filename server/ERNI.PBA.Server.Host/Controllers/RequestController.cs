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
    public class RequestController : Controller
    {
        private readonly Lazy<IGetRequestQuery> _getRequestQuery;
        private readonly Lazy<IGetRequestsQuery> _getRequestsQuery;
        private readonly Lazy<ISetRequestStateCommand> _setRequestStateCommand;
        private readonly Lazy<IAddMassRequestCommand> _addMassRequestCommand;
        private readonly Lazy<IUpdateRequestCommand> _updateRequestCommand;
        private readonly Lazy<IUpdateTeamRequestCommand> _updateTeamRequestCommand;
        private readonly Lazy<IDeleteRequestCommand> _deleteRequestCommand;

        public RequestController(
            Lazy<IGetRequestQuery> getRequestQuery,
            Lazy<IGetRequestsQuery> getRequestsQuery,
            Lazy<ISetRequestStateCommand> approveRequestCommand,
            Lazy<IAddMassRequestCommand> addMassRequestCommand,
            Lazy<IUpdateRequestCommand> updateRequestCommand,
            Lazy<IUpdateTeamRequestCommand> updateTeamRequestCommand,
            Lazy<IDeleteRequestCommand> deleteRequestCommand)
        {
            _getRequestQuery = getRequestQuery;
            _getRequestsQuery = getRequestsQuery;
            _setRequestStateCommand = approveRequestCommand;
            _addMassRequestCommand = addMassRequestCommand;
            _updateRequestCommand = updateRequestCommand;
            _updateTeamRequestCommand = updateTeamRequestCommand;
            _deleteRequestCommand = deleteRequestCommand;
        }

        [HttpGet("{id}")]
#pragma warning disable CA1721 // Property names should not match get methods
        public async Task<IActionResult> GetRequest(int id, CancellationToken cancellationToken)
#pragma warning restore CA1721 // Property names should not match get methods
        {
            var request = await _getRequestQuery.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok(request);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            await _setRequestStateCommand.Value.ExecuteAsync((id, RequestState.Approved), HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            await _setRequestStateCommand.Value.ExecuteAsync((id, RequestState.Rejected), HttpContext.User, cancellationToken);

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
        public async Task<IActionResult> AddMassRequest([FromBody] RequestMassModel payload, CancellationToken cancellationToken)
        {
            await _addMassRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/state/{requestState}/type/{budgetTypeId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<IGetRequestsQuery.RequestModel[]> GetRequests(int year, RequestState requestState, int budgetTypeId, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = new[] { requestState },
                BudgetTypeId = budgetTypeId
            };

            return await _getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await _updateRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await _updateTeamRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, CancellationToken cancellationToken)
        {
            await _deleteRequestCommand.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("personal-budget-left")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<GetBudgetLeftQuery.UserModel[]> BudgetLeft([FromServices] GetBudgetLeftQuery query, CancellationToken cancellationToken) =>
            await query.ExecuteAsync(HttpContext.User, cancellationToken);
    }
}